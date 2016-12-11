using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class OrchestraLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    private string difficulty_level;

    public GameObject _PlayButton;


    public Instrument instrument_1;
    public Instrument instrument_2;
    public Instrument instrument_3;
    public Instrument instrument_4;

    public AudioSource audioSource;
    public bool essai;
    public Camera mainCamera;

    public GameObject violonLight;
    public GameObject marimbaLight;
    public GameObject trompetteLight;
    public GameObject pianoLight;
    // Use this for initialization




    // Use this for initialization
    void Start()
    {

       /* setDifficulty(DIFFICULTY_EASY);

        instrument_1 = new Marimba();
        instrument_2 = new Trompette();
        instrument_3 = new Violon();
        instrument_4 = new Piano();

        placeInstrumentFarLeft(instrument_4);
        placeInstrumentMiddleLeft(instrument_2);
        placeInstrumentMiddleRight(instrument_3);
        placeInstrumentFarRight(instrument_1);*/


        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{ // if left button pressed...
        //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.transform.name == "violonarchet")
        //        {
        //            activateObject(violonLight);

        //            marimbaLight.SetActive(false);
        //            trompetteLight.SetActive(false);
        //            pianoLight.SetActive(false);
        //            //violonLight.SetActive(true);

        //        }
        //        if (hit.transform.name == "marimba")
        //        {
        //            activateObject(marimbaLight);
        //            violonLight.SetActive(false);
        //            trompetteLight.SetActive(false);
        //            pianoLight.SetActive(false);
        //            // myLights[1].enabled = true; //!myLights[1].enabled;
        //        }
        //        if (hit.transform.name == "trompette")
        //        {
        //            activateObject(trompetteLight);
        //            marimbaLight.SetActive(false);
        //            pianoLight.SetActive(false);
        //            violonLight.SetActive(false);

        //            // myLights[2].enabled = !myLights[2].enabled;                    
        //        }
        //        if (hit.transform.name == "piano")
        //        {
        //            activateObject(pianoLight);
        //            marimbaLight.SetActive(false);
        //            trompetteLight.SetActive(false);
        //            violonLight.SetActive(false);

        //            // myLights[3].enabled = !myLights[3].enabled;
        //        }
        //    }
        //}
    }




    void setDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case DIFFICULTY_EASY:
            case DIFFICULTY_MEDIUM:
            case DIFFICULTY_HARD:
                this.difficulty_level = difficulty;
                break;
            default:
                System.Console.WriteLine(difficulty + " is not a valid difficulty input");
                break;
        }
    }

    public void OnplayClicked()
    {

        audioSource.clip = Resources.Load<AudioClip>("Soundtracks/jazzcomedy_extrait1");

        audioSource.Play();
        _PlayButton.SetActive(false);
    }

    void placeInstrumentFarLeft(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getFarLeftVector(), instrument.Model.transform.rotation);
    }

    void placeInstrumentFarRight(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getFarRightVector(), instrument.Model.transform.rotation);
    }

    void placeInstrumentMiddleLeft(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getMiddleLeftVector(), instrument.Model.transform.rotation);
    }

    void placeInstrumentMiddleRight(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getMiddleRightVector(), instrument.Model.transform.rotation);
    }

    public void activateObject(GameObject instrument)
    {
        if (instrument.activeInHierarchy == false)
        {
          
            instrument.SetActive(true);
        }
        else { instrument.SetActive(false); }
    }
}
