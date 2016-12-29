using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TamtamLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    const int MAX_INSTRUMENTS = 4;
    const int QUESTION_READING_TIME = 3;

    #region Attributs assignés à travers l'Inspector

       
    public Camera _mainCamera;                  // Camera de la scène

    public Light farLeftLight;                  // Spot en bas à gauche
    public Light farRightLight;                 // Spot en bas à droite
    public Light middleLeftLight;               // Spot en haut à gauche
    public Light middleRightLight;              // Spot en haut à droite
    
    #endregion

    #region Attributs privées

    string difficulty_level;                    // Niveau de difficulté

    List<TamtamInstrument> _instruments;
    TamtamQuestion _question;

    AudioSource _extractAudioSource;            // Source audio pour l'extrait
    AudioSource _sfxAudioSource;                // Source audio pour les sound effects

    AudioClip _clipSpotlight;                   // Clip du projecteur qui s'allume / s'éteint
    AudioClip _clipApplause;                    // Clip des gens qui applaudissent
    AudioClip _clipBoo;                         // Clip des gens qui huent


    #endregion


    // Use this for initialization
    void Start () {
        SetDifficulty(DIFFICULTY_EASY);

        _sfxAudioSource = GetComponent<AudioSource>();

        _clipSpotlight = Resources.Load<AudioClip>("SFX/spotlight_on");
        _clipApplause = Resources.Load<AudioClip>("SFX/crowd_applause");
        _clipBoo = Resources.Load<AudioClip>("SFX/crowd_boo");

        _question = new TamtamQuestion();
       

        PlaceInstruments();
    }
	
	// Update is called once per frame
	void Update () {
        // Détection d'un click sur Android ou PC.
        bool clicked;
        bool particle = true;
        Vector3 clickedPosition = new Vector3();
        if (Application.platform == RuntimePlatform.Android)
        {
            clicked = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            if (clicked)
                clickedPosition = Input.GetTouch(0).position;
        }
        else
        {
            clicked = Input.GetMouseButtonDown(0);
            if (clicked)
                clickedPosition = Input.mousePosition;
        }

        // Si un clic a été réalisé
        if (clicked)
        {
            Ray ray = _mainCamera.ScreenPointToRay(clickedPosition);
            RaycastHit hit;

            // On cherche quel instrument a été cliqué
            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < _instruments.Count ; i++)
                {
                    _instruments[i].Instrument.EnableParticles(false);

                    if (hit.transform.name == _instruments[i].Instrument.Name )
                    {
                        _instruments[i].Instrument.EnableParticles(true);
                        
                        // Et on toggle le spotlight correspondant (on ou off en fonction de ce qu'il était avant)
                        _instruments[i].ToggleLight();
                        _sfxAudioSource.PlayOneShot(_clipSpotlight, 0.5f);
                    }
                }
            }
        }
    }

    public void Validate()
    {
        bool success = true;
        foreach (TamtamInstrument instrument in _instruments)
        {
            if (!instrument.ToggleLightAnswerFor(true))
            {
                success = false;
            }
        }

        if (success)
        {
            _sfxAudioSource.PlayOneShot(_clipApplause, 0.8f);
        }
        else
        {
            _sfxAudioSource.PlayOneShot(_clipBoo, 0.8f);
        }

    }

    void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case DIFFICULTY_EASY:
            case DIFFICULTY_MEDIUM:
            case DIFFICULTY_HARD:
                this.difficulty_level = difficulty;
                break;
            default:
               // Console.WriteLine(difficulty + " is not a valid difficulty input");
                break;
        }
    }


    void PlaceInstruments()
    {
        _instruments = new List<TamtamInstrument>();
        _instruments.Add(new TamtamInstrument("tamtam", false, farLeftLight));
        _instruments.Add(new TamtamInstrument("tamtam", false, farRightLight));
        _instruments.Add(new TamtamInstrument("tamtam", true, middleLeftLight));
        _instruments.Add(new TamtamInstrument("tamtam", false, middleRightLight));


        // Far left
        _instruments[0].Instrument.PutFarLeft(null);
        _instruments[0].SpotLight = farLeftLight;
        _instruments[0].Instrument.Name = "tamtam1";


        // Far right
        _instruments[1].Instrument.PutFarRight(null);
        _instruments[1].SpotLight = farRightLight;
        _instruments[0].Instrument.Name = "tamtam2";


        // Middle Left
        _instruments[2].Instrument.PutMiddleLeft(null);
        _instruments[2].SpotLight = middleLeftLight;
        _instruments[0].Instrument.Name = "tamtam3";


        // Middle Right
        _instruments[3].Instrument.PutMiddleRight(null);
        _instruments[3].SpotLight = middleRightLight;
        _instruments[0].Instrument.Name = "tamtam4";

    }

}
