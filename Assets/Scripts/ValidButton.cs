using UnityEngine;
using System.Collections;

public class ValidButton : MonoBehaviour
{

    //public GameObject ValidButon;
    public Light ViolonLight;
    public Light TrompetteLight;
    public Light PianoLight;
    public Light MarimbaLight;

    public Camera mainCamera;

    public AudioSource audioSource;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int score = 0;
        //if (hit.transform.name == ValidButon.name)
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space pressed");
            if (ViolonLight.isActiveAndEnabled)
            {
                ViolonLight.color = Color.green;
                score++;
            }
            else
            {
                ViolonLight.color = Color.red;
                ViolonLight.enabled = !ViolonLight.enabled;
            }

            if (PianoLight.isActiveAndEnabled)
            {
                PianoLight.color = Color.green;
                score++;
            }
            else
            {
                PianoLight.color = Color.red;
                PianoLight.enabled = !PianoLight.enabled;
            }

            if (MarimbaLight.isActiveAndEnabled)
            {
                MarimbaLight.color = Color.red;
            }
            else
            {
                MarimbaLight.color = Color.green;
                MarimbaLight.enabled = !MarimbaLight.enabled;
                score++;
            }

            if (TrompetteLight.isActiveAndEnabled)
            {
                TrompetteLight.color = Color.red;
            }
            else
            {
                TrompetteLight.color = Color.green;
                TrompetteLight.enabled = !TrompetteLight.enabled;
                score++;
            }
            if (score==4)
            {
                audioSource.clip = Resources.Load<AudioClip>("/jazzcomedy_extrait1");
                audioSource.Play();
            }
        }
    }
}