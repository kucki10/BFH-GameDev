using TMPro;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderRL;
    public WheelCollider wheelColliderRR;

    public Transform centerOfMass;

    public float maxTorque = 500;

    public float maxSteerAngleHighSpeed = 5;
    public float maxSteerAngleLowSpeed = 45;

    public float sidewaysStiffness = 1.5f;
    public float forewardStiffness = 1.5f;

    public RectTransform speedPointerTransform;
    public TMP_Text speedText;
    public TMP_Text gearText;

    public AudioClip engineSingleRPMSoundClip;

    public ParticleSystem smokeL;
    public ParticleSystem smokeR;

    public ParticleSystem dustFL;
    public ParticleSystem dustFR;
    public ParticleSystem dustBL;
    public ParticleSystem dustBR;

    public bool thrustEnabled;

    public MeshRenderer frontLightsMeshRenderer;
    public Light[] frontLights = new Light[2];

    public float fullBrakeTorque = 5000;
    public AudioClip brakeAudioClip;

    public WheelBehaviour[] wheelBehaviours = new WheelBehaviour[4];


    private Rigidbody _rigidbody;

    private float _currentSpeedKmh;
    private float _maxSpeedKmh = 140.0f;
    private float _maxSpeedBackwardKmh= 30.0f;

    private int _currentGearNum = 1;

    private AudioSource _engineAudioSource;

    private ParticleSystem.EmissionModule _smokeLEmission;
    private ParticleSystem.EmissionModule _smokeREmission;

    private ParticleSystem.EmissionModule _dustEmissionFL;
    private ParticleSystem.EmissionModule _dustEmissionFR;
    private ParticleSystem.EmissionModule _dustEmissionBL;
    private ParticleSystem.EmissionModule _dustEmissionBR;

    private string _groundTagFL;
    private string _groundTagFR;

    private int _groundTextureFL;
    private int _groundTextureFR;

    private bool _carIsOnDrySand;

    private bool _doSkidmarking;
    private bool _carIsNotOnSand;
    private AudioSource _brakeAudioSource;

    void Start()
    {
        GetAndApplyUserPrefs();

        this._rigidbody = gameObject.GetComponent<Rigidbody>();
        this._rigidbody.centerOfMass = new Vector3(centerOfMass.localPosition.x,
            centerOfMass.localPosition.y,
            centerOfMass.localPosition.z);

        SetWheelFrictionStiffness(forewardStiffness, sidewaysStiffness);

        // Configure AudioSource component by program
        _engineAudioSource = gameObject.AddComponent<AudioSource>();
        _engineAudioSource.clip = engineSingleRPMSoundClip;
        _engineAudioSource.loop = true;
        _engineAudioSource.volume = 0.7f;
        _engineAudioSource.playOnAwake = true;
        _engineAudioSource.enabled = false; // Bugfix
        _engineAudioSource.enabled = true; // Bugfix

        // Configure brake audiosource component by program
        _brakeAudioSource = (AudioSource)gameObject.AddComponent<AudioSource>();
        _brakeAudioSource.clip = brakeAudioClip;
        _brakeAudioSource.loop = true;
        _brakeAudioSource.volume = 0.7f;
        _brakeAudioSource.playOnAwake = false;
        _brakeAudioSource.enabled = false;
        _brakeAudioSource.enabled = true;

        _smokeLEmission = smokeL.emission;
        _smokeLEmission.enabled = true;
        _smokeREmission = smokeR.emission;
        _smokeREmission.enabled = true;


        _dustEmissionFL = dustFL.emission;
        _dustEmissionFL.enabled = true;
        _dustEmissionFR = dustFR.emission;
        _dustEmissionFR.enabled = true;
        _dustEmissionBL = dustBL.emission;
        _dustEmissionBL.enabled = true;
        _dustEmissionBR = dustBR.emission;
        _dustEmissionBR.enabled = true;

    }
    
    void FixedUpdate()
    {
        this._currentSpeedKmh = this._rigidbody.velocity.magnitude * 3.6f;

        // Evaluate ground under front wheels
        WheelHit hitFL = GetGroundInfos(ref wheelColliderFL, ref _groundTagFL, ref _groundTextureFL);
        WheelHit hitFR = GetGroundInfos(ref wheelColliderFR, ref _groundTagFR, ref _groundTextureFR);
        _carIsOnDrySand = _groundTagFL.CompareTo("Terrain") == 0 && _groundTextureFL == 0; //0 is sand
        _carIsNotOnSand = _groundTagFL.CompareTo("Terrain") == 0 && _groundTextureFL > 0; //0 is sand

        SetMotorTorque(hitFL, hitFR);
        SetSteerAngle();

        int gearNum = 0;
        float engineRPM = kmh2rpm(_currentSpeedKmh, out gearNum);

        _currentGearNum = gearNum;

        SetEngineSound(engineRPM);

        SetParticleSystems(engineRPM);
    }

    void OnGUI()
    {
        // Speedpointer rotation
        // -34 deg =   0 km/h 
        //  34 deg = 140 km/h
        
        // Get Angle of one km/h and scale that with current speed

        // We need to subtract the to sides (34 deg) from total circle, because the speed meter is not a full circle (140 is the max speed on the speed meter)
        float angleOfOneKmh = -(360 - (2 * 34)) / 140.0f;
        
        // Add initial -34 degree to the rotation, so it starts at 0 km/h
        float degAroundZ = -34 + angleOfOneKmh * this._currentSpeedKmh;

        speedPointerTransform.rotation = Quaternion.Euler(0, 0, degAroundZ);
        // SpeedText show current KMH
        gearText.text = $"Gear: {this._currentGearNum}";
        speedText.text = this._currentSpeedKmh.ToString("0") + " km/h";
    }


    private void GetAndApplyUserPrefs()
    {
        //Load settings
        var prefs = new Prefs();
        prefs.Load();
        
        //Apply settings
        prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
       
        var meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        prefs.SetBuggyColor(ref meshRenderer);

        var self = this;
        prefs.SetFriction(ref self);

        forewardStiffness = prefs.frictionForwards;
        sidewaysStiffness = prefs.frictionSidewards;

        for (var i = 0; i < frontLights.Length; i++)
        {
            prefs.SetFrontLightsEnabled(ref frontLights[i], ref frontLightsMeshRenderer);
            prefs.SetFrontLightsColor(ref frontLights[i], ref frontLightsMeshRenderer);
        }
        

    }


    WheelHit GetGroundInfos(ref WheelCollider wheelCol,
        ref string groundTag,
        ref int groundTextureIndex)
    { // Default values
        groundTag = "InTheAir";
        groundTextureIndex = -1;
        // Query ground by ray shoot on the front left wheel collider
        WheelHit wheelHit;
        wheelCol.GetGroundHit(out wheelHit);
        // If not in the air query collider
        if (wheelHit.collider)
        {
            groundTag = wheelHit.collider.tag;
            if (wheelHit.collider.CompareTag("Terrain"))
                groundTextureIndex = TerrainSurface.GetMainTexture(transform.position);
        }

        //Debug.Log($"GroundInfo: tag:{groundTag} index: {groundTextureIndex}");
        return wheelHit;
    }


    void SetParticleSystems(float engineRPM)
    {
        //Set Engine smoke (exhausts)
        float smokeRate = engineRPM / 10.0f;
        _smokeLEmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);
        _smokeREmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);

        // Set wheels dust
        float dustRate = 0;
        if (_currentSpeedKmh > 10.0f && _carIsOnDrySand)
            dustRate = _currentSpeedKmh;
        //Debug.Log(dustRate);

        _dustEmissionFL.rateOverTime = new ParticleSystem.MinMaxCurve(dustRate);
        _dustEmissionFR.rateOverTime = new ParticleSystem.MinMaxCurve(dustRate);
        _dustEmissionBL.rateOverTime = new ParticleSystem.MinMaxCurve(dustRate);
        _dustEmissionBR.rateOverTime = new ParticleSystem.MinMaxCurve(dustRate);
    }

    void SetEngineSound(float engineRPM)
    {
        if (_engineAudioSource == null) return;
        float minRPM = 800;
        float maxRPM = 8000;
        float minPitch = 0.3f;
        float maxPitch = 3.0f;


        var range = maxRPM - minRPM;
        var scale = 1.0f / range * (engineRPM - minRPM);

        float pitch = Mathf.Lerp(minPitch, maxPitch, scale);

        //Debug.Log($"engineRPM {engineRPM} pitch {pitch}");


        _engineAudioSource.pitch = pitch;
    }

    void SetSteerAngle()
    {
        var speedFactor = 1 - this._currentSpeedKmh / _maxSpeedKmh;
        var currentMaxTurnAngle = maxSteerAngleHighSpeed + ((maxSteerAngleLowSpeed - maxSteerAngleHighSpeed) * speedFactor);
        var steerAngle = currentMaxTurnAngle * Input.GetAxis("Horizontal");

        //Debug.Log($"Steering {steerAngle} (Max: {currentMaxTurnAngle}) with speed: {_currentSpeedKmh} km/h");

        wheelColliderFL.steerAngle = steerAngle;
        wheelColliderFR.steerAngle = steerAngle;
    }
    
    void SetMotorTorque(WheelHit hitFL, WheelHit hitFR)
    {
        if (!thrustEnabled)
        {
            return;
        }

        var velocityIsForward = Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f;

        // Determine if the cursor key input means braking
        var doBraking = _currentSpeedKmh > 0.5f &&
                         (Input.GetAxis("Vertical") < 0 && velocityIsForward ||
                          Input.GetAxis("Vertical") > 0 && !velocityIsForward);
        bool doFullBrake = Input.GetKey("space");
        _doSkidmarking = _carIsNotOnSand && (doFullBrake || hitFL.sidewaysSlip < -0.45f || hitFR.sidewaysSlip > 0.45f) && _currentSpeedKmh > 20.0f;


        SetBrakeSound(_doSkidmarking);
        SetSkidmarking(_doSkidmarking);

        if (doBraking || doFullBrake)
        {
            float brakeTorque = doFullBrake ? fullBrakeTorque : maxTorque;

            wheelColliderFL.brakeTorque = brakeTorque;
            wheelColliderFR.brakeTorque = brakeTorque;
            wheelColliderRL.brakeTorque = brakeTorque;
            wheelColliderRR.brakeTorque = brakeTorque;
            wheelColliderFL.motorTorque = 0;
            wheelColliderFR.motorTorque = 0;
        }
        else
        {
            wheelColliderFL.brakeTorque = 0;
            wheelColliderFR.brakeTorque = 0;
            wheelColliderRL.brakeTorque = 0;
            wheelColliderRR.brakeTorque = 0;

            var torque = maxTorque * Input.GetAxis("Vertical");

            //var direction = this._isMovingForward ? "Forward" : "Backwards";
            //Debug.Log($"Moving {direction} with speed: {_currentSpeedKmh} km/h");

            if ((velocityIsForward && _currentSpeedKmh >= this._maxSpeedKmh) ||
                (!velocityIsForward && _currentSpeedKmh >= this._maxSpeedBackwardKmh))
            {
                //No further acceleration if top speeds are reached
                torque = 0;
            }

            wheelColliderFL.motorTorque = torque;
            wheelColliderFR.motorTorque = torque;
        }
    }

    void SetBrakeSound(bool doBrakeSound)
    {
        if (doBrakeSound)
        {
            _brakeAudioSource.volume = _currentSpeedKmh / 100.0f;
            _brakeAudioSource.Play();
        }
        else
            _brakeAudioSource.Stop();
    }

    // Turns skidmarking on or off on all wheels
    void SetSkidmarking(bool doSkidmarking)
    {
        foreach (var wheel in wheelBehaviours)
            wheel.DoSkidmarking(doSkidmarking);
    }

    void SetWheelFrictionStiffness(float forewardStiffness, float sidewaysStiffness)
    {
        WheelFrictionCurve f_fwWFC = wheelColliderFL.forwardFriction;
        WheelFrictionCurve f_swWFC = wheelColliderFL.sidewaysFriction;

        f_fwWFC.stiffness = forewardStiffness;
        f_swWFC.stiffness = sidewaysStiffness;

        wheelColliderFL.forwardFriction = f_fwWFC;
        wheelColliderFL.sidewaysFriction = f_swWFC;
        wheelColliderFR.forwardFriction = f_fwWFC;
        wheelColliderFR.sidewaysFriction = f_swWFC;

        wheelColliderRL.forwardFriction = f_fwWFC;
        wheelColliderRL.sidewaysFriction = f_swWFC;
        wheelColliderRR.forwardFriction = f_fwWFC;
        wheelColliderRR.sidewaysFriction = f_swWFC;
    }

    float kmh2rpm(float kmh, out int gearNum)
    {
        Gear[] gears =
        { new Gear( 1, 900, 12, 1400),
            new Gear( 12, 900, 25, 2000),
            new Gear( 25, 1350, 45, 2500),
            new Gear( 45, 1950, 70, 3500),
            new Gear( 70, 2500, 112, 4000),
            new Gear(112, 3100, 180, 5000)
        };
        for (int i = 0; i < gears.Length; ++i)
        {
            if (gears[i].speedFits(kmh))
            {
                gearNum = i + 1;
                return gears[i].interpolate(kmh);
            }
        }
        gearNum = 1;
        return 800;
    }
    
}

class Gear
{
    public Gear(float minKMH, float minRPM, float maxKMH, float maxRPM)
    {
        _minRPM = minRPM;
        _minKMH = minKMH;
        _maxRPM = maxRPM;
        _maxKMH = maxKMH;
    }
    private float _minRPM;
    private float _minKMH;
    private float _maxRPM;
    private float _maxKMH;
    public bool speedFits(float kmh)
    {
        return kmh >= _minKMH && kmh <= _maxKMH;
    }
    public float interpolate(float kmh)
    {
        var range = _maxKMH -_minKMH;
        var scale = 1.0f / range * (kmh - _minKMH);

        //Debug.Log($"kmh {kmh} min {_minKMH} max {_maxKMH}--> scale {scale}");

        return Mathf.Lerp(_minRPM, _maxRPM, scale);
    }
}