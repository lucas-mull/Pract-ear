﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class OrchestraLoad : MonoBehaviour {

    const int MAX_INSTRUMENTS = 4;
    const int QUESTION_READING_TIME = 3;

    const string PATH_TO_BLINDTEST = "Soundtracks/Blindtest/";

    #region Attributs assignés à travers l'Inspector

    public Button _playButton;                  // Bouton 'Play' pour jouer / arrêter l'extrait
    public Button _validateButton;              // Bouton 'Valider' pour valider ses choix
    public Slider _soundtrackSlider;            // Slider pour se déplacer facilement dans le morceau
    public Canvas _igInterface;                 // Canvas contenant l'interface du jeu
    public Canvas _igMenu;                      // Canvas contenant le menu de pause (appui sur le bouton 'menu')
    public Canvas _igSuccess;                   // Canvas affiché quand on réussit
    public Canvas _igGameOver;                  // Canvas affiché quand on rate
    public Sprite _playSprite;                  // Sprite 'Play' du bouton play
    public Sprite _pauseSprite;                 // Sprite 'Pause' du bouton play
    public Camera _mainCamera;                  // Camera de la scène
    public Text   _questionText;                // Texte qui affiche la question actuelle
    public Image _logoDefeat;                   // Image du logo quand on perd

    // Tooltips
    public Text _farLeftText;                   
    public Text _farRightText;
    public Text _middleLeftText;
    public Text _middleRightText;

    public Light _farLeftLight;                 // Spot en bas à gauche
    public Light _farRightLight;                // Spot en bas à droite
    public Light _middleLeftLight;              // Spot en haut à gauche
    public Light _middleRightLight;             // Spot en haut à droite

    #endregion

    #region Attributs privées

    string _difficultyLevel;                    // Niveau de difficulté

    List<BlindTestInstrument> _instruments;     // Liste des instruments présents sur la scène
    BlindTestQuestion _question;                // Question du blindtest

    AudioSource _extractAudioSource;            // Source audio pour l'extrait
    AudioSource _sfxAudioSource;                // Source audio pour les sound effects

    AudioClip _clipSpotlight;                   // Clip du projecteur qui s'allume / s'éteint
    AudioClip _clipApplause;                    // Clip des gens qui applaudissent
    AudioClip _clipBoo;                         // Clip des gens qui huent

    Extract _extract;                           // Extrait joué pendant la partie

    bool _isExtractPaused = false;              // Indique si l'extrait est actuellement en pause ou pas
    bool _isReadingQuestion = true;             // Indique si on est encore dans le temps de lecture de la question
    bool _isGamePaused = false;                 // Indique si le jeu est en pause (appui sur le bouton 'menu')

    int _selectedInstrumentsCount = 0;          // Nombre d'instruments sélectionnés

    #endregion

    #region Callbacks automatiques

    // Use this for initialization
    void Start()
    {
        _extractAudioSource = _igInterface.GetComponent<AudioSource>();
        _sfxAudioSource = GetComponent<AudioSource>();

        _clipSpotlight = Resources.Load<AudioClip>("SFX/spotlight_on");
        _clipApplause = Resources.Load<AudioClip>("SFX/crowd_applause");
        _clipBoo = Resources.Load<AudioClip>("SFX/crowd_boo");

        Init();

        EnableInstrumentsColliders(false);
        _logoDefeat.GetComponent<Animator>().SetTrigger("lost");
    }

    void Init()
    {
        SetDifficulty();
        _extractAudioSource.clip = _extract.Clip;

        _question = new BlindTestQuestion(MAX_INSTRUMENTS);
        _question.InstrumentsInExtract = _extract.InstrumentsNames;
        _questionText.text = _question.Question;

        _instruments = _question.GenerateInstrumentListForQuestion();

        _selectedInstrumentsCount = 0;
        _validateButton.interactable = false;
        SetValidateButtonColor(Color.white);
        PlaceInstrumentsRandomly();        
    }

    public void Restart()
    {
        foreach (BlindTestInstrument instrument in _instruments)
        {
            instrument.ToggleLight(false);
            instrument.Destroy();
        }

        Init();
        _igSuccess.enabled = false;
        _igGameOver.enabled = false;
        _isReadingQuestion = true;        

        _questionText.transform.parent.GetComponent<Animator>().SetBool("start", false);
    }

    public void NextLevel()
    {
        DifficultyManager.IncrementDifficulty();
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        // Si on vient de démarrer, on lance l'attente pour lire la question
        if (_isReadingQuestion)
        {
            _isReadingQuestion = false;
            StartCoroutine(WaitForReadingTimeCoroutine());
        }

        // Si l'audio a été démarrée, on lance la co-routine qui bouge le slider avec la musique.
        if (_extractAudioSource.isPlaying)
        {
            StartCoroutine(PlayExtractCoroutine());
        }
        // Si l'audio ne joue pas et qu'elle n'est pas en pause, cela veut dire qu'on est arrivé à la fin du morceau.
        else if (!_isExtractPaused)
        {
            // Dans ce cas on réinitialise le lecteur en haut de l'interface.
            ResetExtractPlayback();
        }

        // Si un clic a été réalisé
        if (Utils.Clicked())
        {
            Ray ray = _mainCamera.ScreenPointToRay(Utils.GetClickedPosition());
            RaycastHit hit;

            // On cherche quel instrument a été cliqué
            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < _instruments.Count; i++)
                {
                    if (hit.transform.name == _instruments[i].Instrument.Name)
                    {                        
                        // Et on toggle le spotlight correspondant (on ou off en fonction de ce qu'il était avant)
                        _instruments[i].ToggleLight();
                        if (_instruments[i].isLit)
                        {
                            _selectedInstrumentsCount++;
                            if (_selectedInstrumentsCount == 1)
                            {
                                EnableValidationButton(true);
                            }
                        }
                        else
                        {
                            _selectedInstrumentsCount--;
                            if (_selectedInstrumentsCount == 0)
                            {
                                EnableValidationButton(false);
                            }
                        }

                        _sfxAudioSource.PlayOneShot(_clipSpotlight, 0.5f);
                    }
                }
            }
        }
    }

    #endregion

    #region Listeners

    /// <summary>
    /// Appelé quand on commence à appuyer sur le slider ('DOWN').
    /// </summary>
    public void OnSliderDown()
    {
        if (_extractAudioSource.isPlaying)
        {
            _isExtractPaused = true;
            _extractAudioSource.Pause();
        }        
    }

    /// <summary>
    /// Appelé quand on a fini d'appuyer sur le slider (changement du time de l'audio).
    /// </summary>
    public void OnSliderUp()
    {
        _extractAudioSource.time = (int)(_soundtrackSlider.value * _extractAudioSource.clip.length / _soundtrackSlider.maxValue);
        if (_isExtractPaused)
        {
            _extractAudioSource.UnPause();
            _isExtractPaused = false;
        }        
    }

    /// <summary>
    /// Listener du bouton 'play'
    /// Change l'état du bouton et de l'audio en fonction de l'état actuel.
    /// </summary>
    public void PlayOrPauseExtract()
    {
        if (_extractAudioSource.isPlaying)
        {
            PauseExtract();
        }
        else
        {
            PlayExtract();
        }
    }

    /// <summary>
    /// Met en pause ou reprend le jeu
    /// Utilisation du hack du timeScale à 0 pour "stopper" l'exécution du jeu.
    /// Note : les sons qui ont commencé se terminent
    /// </summary>
    public void PauseOrResumeGame()
    {
        if (!_isGamePaused)
        {
            EnableInstrumentsColliders(false);
            Time.timeScale = 0;
            _isGamePaused = true;
            _igMenu.enabled = true;            
            if (!_isExtractPaused)
            {
                PauseExtract();
            }
        }
        else
        {            
            _isGamePaused = false;
            _igMenu.enabled = false;
            Time.timeScale = 1;
            EnableInstrumentsColliders(true);
            PlayExtract();
        }
    }

    /// <summary>
    /// Retour au menu principal
    /// </summary>
    public void Back()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Listener du bouton 'Valider'
    /// Toggle toutes les lumières des réponses. Si tout est bon, feedback des applaudissement.
    /// Sinon, feedback des huées.
    /// </summary>
    public void Validate()
    {
        if (!_validateButton.IsInteractable())
            return;

        EnableInstrumentsColliders(false);

        bool success = true;
        foreach(BlindTestInstrument instrument in _instruments)
        {
            if (!instrument.ToggleLightAnswerFor(_question.affirmative))
            {
                success = false;
            }
        }

        if (success)
        {
            _sfxAudioSource.PlayOneShot(_clipApplause, 0.8f);

            SetValidateButtonColor(Colors.GREEN);

            _igSuccess.enabled = true;
            DifficultyManager.UnlockNextDifficulty(DifficultyManager.BLINDTEST);
        }
        else
        {
            SetValidateButtonColor(Colors.RED);

            _sfxAudioSource.PlayOneShot(_clipBoo, 0.8f);
            _igGameOver.enabled = true;
        }

        ResetExtractPlayback();
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Coroutine qui attend simplement QUESTION_READING_TIME secondes avant de démarrer l'écoute.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator WaitForReadingTimeCoroutine()
    {
        yield return new WaitForSeconds(QUESTION_READING_TIME);

        // Une fois le temps écoulé on déclenche l'animation qui déplace la question en haut de l'écran.
        _questionText.transform.parent.GetComponent<Animator>().SetBool("start", true);

        // Et si l'audio n'était pas déjà en train de jouer, on la lance.
        if (!_extractAudioSource.isPlaying)
        {
            this.PlayExtract();
        }

        EnableInstrumentsColliders(true);
    }

    /// <summary>
    /// Co routine qui déplace la valeur du slider en concordance avec l'avancée du temps du morceau.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator PlayExtractCoroutine()
    {
        // Simple produit en croix
        _soundtrackSlider.value = (int)(_extractAudioSource.time * _soundtrackSlider.maxValue / _extractAudioSource.clip.length);
        yield return null;
    }

    #endregion

    #region Méthodes privées

    void SetDifficulty()
    {
        if (DifficultyManager.PICKED_DIFFICULTY == null)
            DifficultyManager.PICKED_DIFFICULTY = DifficultyManager.EASY;

        string folder = DifficultyManager.PICKED_DIFFICULTY + "/json/";
        _extract = Extract.PickRandomFrom(PATH_TO_BLINDTEST + folder);
    }

    /// <summary>
    /// Pause l'extrait
    /// </summary>
    public void PauseExtract()
    {
        _playButton.image.sprite = _playSprite;
        _extractAudioSource.Pause();
        _isExtractPaused = true;
        AnimateInstruments(false);
    }

    /// <summary>
    /// Unpause / Démarre l'extrait
    /// </summary>
    void PlayExtract()
    {
        if (_isExtractPaused)
        {
            _playButton.image.sprite = _pauseSprite;
            _extractAudioSource.UnPause();
            _isExtractPaused = false;
            AnimateInstruments(true);
        }
        else
        {
            _playButton.image.sprite = _pauseSprite;
            _extractAudioSource.Play();
            AnimateInstruments(true);
        }
    }

    /// <summary>
    /// Place les instruments au hasard sur la scène
    /// </summary>
    void PlaceInstrumentsRandomly()
    {
        /*  
        Utilisation d'une copie temporaire de la liste des instruments.
        Les instruments étant des pointeurs je peux me permettre de travailler
        sur cette copie et de voir les changements appliqués sur l'originale.
        Le principe est d'enlevé un instrument de la liste une fois qu'il a été placé. 
        */
        List<BlindTestInstrument> temp = new List<BlindTestInstrument>();
        temp.AddRange(_instruments);

        // Far left
        int index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutFarLeft(_farLeftText);
        temp[index].SpotLight = _farLeftLight;
        temp.RemoveAt(index);

        // Far right
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutFarRight(_farRightText);
        temp[index].SpotLight = _farRightLight;
        temp.RemoveAt(index);

        // Middle Left
        index = UnityEngine.Random.Range(0, temp.Count);
        temp[index].Instrument.PutMiddleLeft(_middleLeftText);
        temp[index].SpotLight = _middleLeftLight;
        temp.RemoveAt(index);

        // Middle Right
        temp[0].Instrument.PutMiddleRight(_middleRightText);
        temp[0].SpotLight = _middleRightLight;
        temp.RemoveAt(0);
    }

    /// <summary>
    /// Réinitialise le lecteur pour le remettre au début de l'extrait.
    /// </summary>
    void ResetExtractPlayback()
    {
        if (_extractAudioSource.isPlaying)
            _extractAudioSource.Stop();

        _soundtrackSlider.value = 0;
        _playButton.image.sprite = _playSprite;
        _extractAudioSource.time = 0;

        AnimateInstruments(false);   
    }

    /// <summary>
    /// Démarre ou arrête l'animation des instruments sur la scène
    /// </summary>
    /// <param name="animate">true pour animer les instruments, false pour arrêter l'animation</param>
    void AnimateInstruments(bool animate)
    {
        for (int i = 0; i < _instruments.Count; i++)
        {
            if (animate)
            {
                _instruments[i].Instrument.StartAnimation(true);
            }
            else
            {
                _instruments[i].Instrument.StopAnimation();
            }
        }
    }

    /// <summary>
    /// Active ou désactive les colliders des instruments dans le but d'empêcher le joueur de cliquer quand il ne doit pas
    /// </summary>
    /// <param name="enabled">true pour activer, false pour désactiver</param>
    void EnableInstrumentsColliders(bool enabled)
    {
        List<BlindTestInstrument> instruments = _instruments;
        foreach (BlindTestInstrument blindTestInstrument in _instruments)
        {
            blindTestInstrument.Instrument.Collider.enabled = enabled;
        }
    }

    void EnableValidationButton(bool enable)
    {
        _validateButton.interactable = enable;
        Image image = _validateButton.transform.GetChild(1).GetComponent<Image>();
        if (enable)
        {
            image.color = Colors.YELLOW;
        }
        else
        {
            image.color = Colors.WHITE;
        }
    }

    void SetValidateButtonColor(Color color)
    {
        _validateButton.GetComponent<Image>().color = color;
    }

    #endregion
}
