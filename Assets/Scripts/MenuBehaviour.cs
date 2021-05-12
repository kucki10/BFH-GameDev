using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{

    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderRL;
    public WheelCollider wheelColliderRR;

    public MeshRenderer buggyMeshRenderer;

    public CarBehaviour carBehaviour;

    public MeshRenderer frontLightsMeshRenderer;
    public Light[] frontLights = new Light[2];


    public Slider sliderSuspensionDistance;
    public Text suspensionDistanceValue;
    public Slider sliderSuspensionSpring;
    public Text suspensionSpringValue;
    public Slider sliderSuspensionDamper;
    public Text suspensionDamperValue;

    public Slider sliderBuggyColorHue;
    public Text buggyColorHueValue;
    public Slider sliderBuggyColorSat;
    public Text buggyColorSatValue;
    public Slider sliderBuggyColorVal;
    public Text buggyColorValValue;

    public Slider sliderFrictionForwards;
    public Text frictionForwardsValue;
    public Slider sliderFrictionSidewards;
    public Text frictionSidewardsValue;

    public Toggle toggleLightsEnabled;
    public Slider sliderFrontLightsColorHue;
    public Text frontLightsColorHueValue;
    public Slider sliderFrontLightsColorSat;
    public Text frontLightsColorSatValue;
    public Slider sliderFrontLightsColorVal;
    public Text frontLightsColorValValue;

    private Prefs _prefs;
    

    void Start()
    {
        _prefs = new Prefs();

        _prefs.Load();
        
        _prefs.SetAll(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);

        sliderSuspensionDistance.value = _prefs.suspensionDistance;
        suspensionDistanceValue.text = sliderSuspensionDistance.value.ToString("0.00");
        sliderSuspensionSpring.value = _prefs.suspensionSpring;
        suspensionSpringValue.text = sliderSuspensionSpring.value.ToString("0");
        sliderSuspensionDamper.value = _prefs.suspensionDamper;
        suspensionDamperValue.text = sliderSuspensionDamper.value.ToString("0");


        sliderBuggyColorHue.value = _prefs.buggyColorHue;
        buggyColorHueValue.text = sliderBuggyColorHue.value.ToString("0");
        sliderBuggyColorSat.value = _prefs.buggyColorSat;
        buggyColorSatValue.text = sliderBuggyColorSat.value.ToString("0");
        sliderBuggyColorVal.value = _prefs.buggyColorSat;
        buggyColorValValue.text = sliderBuggyColorVal.value.ToString("0");


        sliderFrictionForwards.value = _prefs.frictionForwards;
        frictionForwardsValue.text = sliderFrictionForwards.value.ToString("0.00");
        sliderFrictionSidewards.value = _prefs.frictionSidewards;
        frictionSidewardsValue.text = sliderFrictionSidewards.value.ToString("0.00");


        toggleLightsEnabled.isOn = _prefs.frontLightsEnabled;
        sliderFrontLightsColorHue.value = _prefs.frontLightsColorHue;
        frontLightsColorHueValue.text = sliderFrontLightsColorHue.value.ToString("0");
        sliderFrontLightsColorSat.value = _prefs.frontLightsColorSat;
        frontLightsColorSatValue.text = sliderFrontLightsColorSat.value.ToString("0");
        sliderFrontLightsColorVal.value = _prefs.frontLightsColorVal;
        frontLightsColorValValue.text = sliderFrontLightsColorVal.value.ToString("0");

    }

    public void OnSliderChangedSuspensionDistance(float _)
    {
        suspensionDistanceValue.text = sliderSuspensionDistance.value.ToString("0.00");
        _prefs.suspensionDistance = sliderSuspensionDistance.value;

        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedSuspensionSpring(float _)
    {
        suspensionSpringValue.text = sliderSuspensionSpring.value.ToString("0");
        _prefs.suspensionSpring = sliderSuspensionSpring.value;

        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedSuspensionDamper(float _)
    {
        suspensionDamperValue.text = sliderSuspensionDamper.value.ToString("0");
        _prefs.suspensionDamper = sliderSuspensionDamper.value;

        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedBuggyColorHue(float _)
    {
        buggyColorHueValue.text = sliderBuggyColorHue.value.ToString("0");
        _prefs.buggyColorHue = (int)sliderBuggyColorHue.value;

        _prefs.SetBuggyColor(ref buggyMeshRenderer);
    }

    public void OnSliderChangedBuggyColorSat(float _)
    {
        buggyColorSatValue.text = sliderBuggyColorSat.value.ToString("0");
        _prefs.buggyColorSat = (int)sliderBuggyColorSat.value;

        _prefs.SetBuggyColor(ref buggyMeshRenderer);
    }

    public void OnSliderChangedBuggyColorVal(float _)
    {
        buggyColorValValue.text = sliderBuggyColorVal.value.ToString("0");
        _prefs.buggyColorVal = (int)sliderBuggyColorVal.value;

        _prefs.SetBuggyColor(ref buggyMeshRenderer);
    }


    public void OnSliderChangedFrictionForwards(float _)
    {
        frictionForwardsValue.text = sliderFrictionForwards.value.ToString("0.00");
        _prefs.frictionForwards = sliderFrictionForwards.value;

        _prefs.SetFriction(ref carBehaviour);
    }

    public void OnSliderChangedFrictionSidewards(float _)
    {
        frictionSidewardsValue.text = sliderFrictionSidewards.value.ToString("0.00");
        _prefs.frictionSidewards = sliderFrictionSidewards.value;

        _prefs.SetFriction(ref carBehaviour);
    }


    public void OnToggleChangedFrontLightsEnabled(bool _)
    {
        _prefs.frontLightsEnabled = toggleLightsEnabled.isOn;

        for (var i = 0; i < frontLights.Length; i++)
        {
            _prefs.SetFrontLightsEnabled(ref frontLights[i], ref frontLightsMeshRenderer);
        }
    }

    public void OnSliderChangedFrontLightsColorHue(float _)
    {
        frontLightsColorHueValue.text = sliderFrontLightsColorHue.value.ToString("0");
        _prefs.frontLightsColorHue = (int)sliderFrontLightsColorHue.value;
        
        ApplyColorForFrontLights();
    }

    public void OnSliderChangedFrontLightsColorSat(float _)
    {
        frontLightsColorSatValue.text = sliderFrontLightsColorSat.value.ToString("0");
        _prefs.frontLightsColorSat = (int)sliderFrontLightsColorSat.value;

        ApplyColorForFrontLights();
    }

    public void OnSliderChangedFrontLightsColorVal(float _)
    {
        frontLightsColorValValue.text = sliderFrontLightsColorVal.value.ToString("0");
        _prefs.frontLightsColorVal = (int)sliderFrontLightsColorVal.value;

        ApplyColorForFrontLights();
    }

    private void ApplyColorForFrontLights()
    {
        for (var i = 0; i < frontLights.Length; i++)
        {
            _prefs.SetFrontLightsColor(ref frontLights[i], ref frontLightsMeshRenderer);
        }
    }


    public void OnStartClick()
    {
        SceneManager.LoadScene("SampleScene");
        _prefs.Save();
    }

    void OnApplicationQuit()
    {
        _prefs.Save();
    }
}