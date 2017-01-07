using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class TamtamLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    const int MAX_INSTRUMENTS = 4;
    const int QUESTION_READING_TIME = 3;

    #region Attributs assignés à travers l'Inspector

       
    public Camera _mainCamera;                  // Camera de la scène

    public Text _questionText;                // Texte qui affiche la question actuelle

    public Button farLeftButton;
    public Button farRightButton;
    public Button middleLeftButton;
    public Button middleRightButton;

    public Sprite _playSprite;                  // Sprite 'Play' du bouton play
    public Sprite _pauseSprite;                 // Sprite 'Pause' du bouton play


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

    EventSystem eventsystem;

    #endregion


    // Use this for initialization
    void Start () {
        SetDifficulty(DIFFICULTY_EASY);

        _extractAudioSource = GetComponent<AudioSource>();
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

                    if (hit.transform.name == _instruments[i].Instrument.Name )
                    {
                        
                        // Et on toggle le spotlight correspondant (on ou off en fonction de ce qu'il était avant)
                        _instruments[i].ToggleLight();
                        _sfxAudioSource.PlayOneShot(_clipSpotlight, 0.5f);
                    }
                    
                }
            }
            RaycastHit2D hit2 = Physics2D.Raycast(clickedPosition, Vector2.zero);
            for(int i =0; i<_instruments.Count; i++)
            {
                if(hit2.collider != null)
                {
                    if (hit2.collider.name == _instruments[i].play.name)
                    {
                        if (!_instruments[i].isPlaying)
                        {
                            for (int j = 0; j < _instruments.Count; j++)
                            {
                                if (_instruments[j].isPlaying && j!=i)
                                    _instruments[j].PlayExtract();
                            }

                        }
                        else
                        {

                        }
                        

                        //_sfxAudioSource.PlayOneShot(_clipSpotlight, 0.5f);
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
        temp[index].play = farLeftButton;
        temp[index].source = _sfxAudioSource;
        temp.RemoveAt(index);

        // Far right
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutFarRight(null);
        temp[index].SpotLight = farRightLight;
        temp[index].source = _sfxAudioSource;
        temp[index].play = farRightButton;
        temp.RemoveAt(index);

        // Middle Left
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutMiddleLeft(null);
        temp[index].SpotLight = middleLeftLight;
        temp[index].source = _sfxAudioSource;
        temp[index].play = middleLeftButton;
        temp.RemoveAt(index);

        // Middle Right
        temp[0].Instrument.PutMiddleRight(null);
        temp[0].SpotLight = middleRightLight;
        temp[0].source = _sfxAudioSource;
        temp[0].play = middleRightButton;
        temp.RemoveAt(0);
    }
    
}
