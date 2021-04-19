using System.Collections;
using TMPro;
using UnityEngine;

public class TimingBehaviour : MonoBehaviour
{
    public int countMax = 3;
    public TMP_Text timeText;
    public AudioClip countdownClip;

    private int _countDown;
    private CarBehaviour _carScript;
    private AudioSource _gateAudioSource;

    private float _pastTime = 0;
    private bool _isFinished = false;
    private bool _isStarted = false;

    // Use this for initialization
    void Start()
    {
        _carScript = GameObject.Find("MainBuggy").GetComponent<CarBehaviour>();
        _carScript.thrustEnabled = false;


        // Configure AudioSource component by program
        _gateAudioSource = gameObject.AddComponent<AudioSource>();
        _gateAudioSource.clip = countdownClip;
        _gateAudioSource.loop = false;
        _gateAudioSource.volume = 1.0f;
        _gateAudioSource.enabled = false; // Bugfix
        _gateAudioSource.enabled = true; // Bugfix

        print("Begin Start:" + Time.time);
        StartCoroutine(GameStart());
        print("End Start:" + Time.time);
    }

    // GameStart CoRoutine
    IEnumerator GameStart()
    {
        print(" Begin GameStart:" + Time.time);
        for (_countDown = countMax; _countDown > 0; _countDown--)
        {
            yield return new WaitForSeconds(1);
            print(" WaitForSeconds:" + Time.time);
            _gateAudioSource.Play();

        }
        print(" End GameStart:" + Time.time);

        _gateAudioSource.pitch = 2.0f;
        _gateAudioSource.Play();
        
        _carScript.thrustEnabled = true;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
        {
            if (!_isStarted)
                _isStarted = true;
                
            else 
                _isFinished = true;
        }
    }

    void OnGUI()
    {
        if (_carScript.thrustEnabled)
        {
            if (_isStarted && !_isFinished)
                _pastTime += Time.deltaTime;
            timeText.text = _pastTime.ToString("0.0") + " sec.";
        }
        else
            timeText.text = _countDown.ToString("0.0") + " sec.";
    }
}
