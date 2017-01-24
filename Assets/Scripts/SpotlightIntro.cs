using UnityEngine;
using System.Collections;

public class SpotlightIntro : MonoBehaviour {

    public Light[] _spotLights;

    AudioSource _audioSource;
    AudioClip _spotlightClip;
    bool _started = true;

	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
        _spotlightClip = Resources.Load<AudioClip>("SFX/spotlight_on");
        _audioSource.clip = _spotlightClip;

        foreach(Light light in _spotLights)
        {
            light.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if (_started)
        {
            _started = false;
            StartCoroutine(TurnLightsOn());
        }
	}

    IEnumerator TurnLightsOn()
    {
        foreach(Light light in _spotLights)
        {
            yield return new WaitForSeconds(1);
            light.enabled = true;
            _audioSource.Play();            
        }
    }
}
