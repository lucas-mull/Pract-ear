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

    public Text _questionText;                // Texte qui affiche la question actuelle

    public Light farLeftLight;                  // Spot en bas à gauche
    public Light farRightLight;                 // Spot en bas à droite
    public Light middleLeftLight;               // Spot en haut à gauche
    public Light middleRightLight;              // Spot en haut à droite
    
    #endregion

    #region Attributs privées

    string difficulty_level;                    // Niveau de difficulté

    List<TamtamInstrument> _instruments;        //Liste des instruments présents sur la scene
    TamtamQuestion _question;                   //Question du tamtam intru

    AudioSource _extractAudioSource;            // Source audio pour l'extrait
    AudioSource _sfxAudioSource;                // Source audio pour les sound effects

    AudioClip _clipSpotlight;                   // Clip du projecteur qui s'allume / s'éteint
    AudioClip _clipApplause;                    // Clip des gens qui applaudissent
    AudioClip _clipBoo;                         // Clip des gens qui huent

    List<Extract> _extracts;                    //Liste des extraits joués par les Tamtams

    bool _isExtractPaused = false;              // Indique si l'extrait est actuellement en pause ou pas
    bool _isReadingQuestion = true;             // Indique si on est encore dans le temps de lecture de la question
    bool _isGamePaused = false;                 // Indique si le jeu est en pause (appui sur le bouton 'menu')


    #endregion


    // Use this for initialization
    void Start () {
        SetDifficulty(DIFFICULTY_EASY);

        
        _sfxAudioSource = GetComponent<AudioSource>();

        _clipSpotlight = Resources.Load<AudioClip>("SFX/spotlight_on");
        _clipApplause = Resources.Load<AudioClip>("SFX/crowd_applause");
        _clipBoo = Resources.Load<AudioClip>("SFX/crowd_boo");

        _question = new TamtamQuestion();
        _questionText.text = _question.Question;

        _instruments = _question.GenerateInstrumentListForQuestion();

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

        List<TamtamInstrument> temp = new List<TamtamInstrument>();
        temp.AddRange(_instruments);

        int index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutFarLeft(null);
        temp[index].SpotLight = farLeftLight;
        temp.RemoveAt(index);

        // Far right
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutFarRight(null);
        temp[index].SpotLight = farRightLight;
        temp.RemoveAt(index);

        // Middle Left
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutMiddleLeft(null);
        temp[index].SpotLight = middleLeftLight;
        temp.RemoveAt(index);

        // Middle Right
        temp[0].Instrument.PutMiddleRight(null);
        temp[0].SpotLight = middleRightLight;
        temp.RemoveAt(0);

        //_instruments = new List<TamtamInstrument>();
        //_instruments.Add(new TamtamInstrument("tamtam", false, farLeftLight));
        //_instruments.Add(new TamtamInstrument("tamtam", false, farRightLight));
        //_instruments.Add(new TamtamInstrument("tamtam", false, middleLeftLight));
        //_instruments.Add(new TamtamInstrument("tamtam", false, middleRightLight));


        // Far left
        /*int index = UnityEngine.Random.Range(0, temp.Count);
        _instruments[index].Instrument.PutFarLeft(null);
        _instruments[index].SpotLight = farLeftLight;
        _instruments[index].Instrument.Name = "tamtam1";
        _instruments[index].extrait = _question.GetExtractsforQuestion()[index];
        temp.RemoveAt(index);

        // Far right
        index = UnityEngine.Random.Range(0, 4);
        _instruments[1].Instrument.PutFarRight(null);
        _instruments[1].SpotLight = farRightLight;
        _instruments[0].Instrument.Name = "tamtam2";
        _instruments[index].extrait = _question.GetExtractsforQuestion()[index];
        temp.RemoveAt(index);

        // Middle Left
        index = UnityEngine.Random.Range(0, 4);
        _instruments[2].Instrument.PutMiddleLeft(null);
        _instruments[2].SpotLight = middleLeftLight;
        _instruments[0].Instrument.Name = "tamtam3";
        _instruments[index].extrait = _question.GetExtractsforQuestion()[index];

        // Middle Right
        index = UnityEngine.Random.Range(0, 4);
        _instruments[3].Instrument.PutMiddleRight(null);
        _instruments[3].SpotLight = middleRightLight;
        _instruments[0].Instrument.Name = "tamtam4";
        _instruments[index].extrait = _question.GetExtractsforQuestion()[index];*/


    }

}
