using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimonLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    string[] EASY_CATEGORIES = new string[]{ Category.VENTS, Category.CORDES, Category.CLAVIERS, Category.PERCUSSIONS };

    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";
    const int MAX_ERRORS_ALLOWED = 3;

    #region Objets assignés à travers l'Inspector

    public Camera _mainCamera;                              // Camera principale de la scène

    public Canvas _igMenu;                                  // Canvas contenant l'UI du menu
    public Canvas _igInterface;                             // Canvas contenant l'interface in game
    public Canvas _igGameOver;                              // Canvas du Game Over
    public Canvas _igSuccess;                               // Canvas du Success (quand on réussit un niveau)
    public GameObject _actionsPanel;                        // Panel des actions dans le menu (reprendre / retour au menu)
    public Image _menuBackground;                           // Image background du menu pause

    public Text _farLeftText;                               // Texte pour le nom d'instrument en bas à gauche
    public Text _middleLeftText;                            // Texte pour le nom d'instrument en haut à gauche
    public Text _middleRightText;                           // Texte pour le nom d'instrument en bas à droite
    public Text _farRightText;                              // Texte pour le nom d'instrument en bas à droite
    public Text _countdownText;                             // Texte contenant le compte à rebours de départ
    public Text _sequenceCountText;                         // Texte contenant le score actuel (longueur de la séquence)

    public Image[] _lifeSprites;                            // Sprites pour les "vies"

    public AudioSource _audioSource;                        // AudioSource principale de notre scène

    #endregion

    #region Variables privées

    string _difficulty_level;                               // Niveau de difficulté (pas encore implémenté)

    List<Instrument> _instruments = new List<Instrument>(); // Instruments présents sur la scène
    List<Instrument> _sequence;                             // Séquence actuelle des instruments
    Instrument _chosenInstrument;                           // Instrument choisi pour la prochaine note
    Partition _partition;                                   // Partition jouée

    // Différents booléens qui servent à réguler le jeu
    bool _playerTurn, _sequenceIsPlaying, 
        _hasStarted, _success, _gameOver, _isGamePaused;

    int _currentIndexInSequence;                            // Index actuelle dans la liste d'instruments
    int _errorCount;                                        // Nombre d'erreurs effectuées par l'utilisateur
    int _sequenceCount;                                     // Nombre de notes enchaînées correctement par le joueur

    #endregion

    #region Callbacks automatiques

    // Use this for initialization
    void Start () {
        Init();

        SetDifficulty(DIFFICULTY_EASY);

        _partition = Partition.LoadPartitionFromJson("clair_de_la_lune");

        _instruments[0].PutFarLeft(_farLeftText);
        _instruments[1].PutFarRight(_farRightText);
        _instruments[2].PutMiddleLeft(_middleLeftText);
        _instruments[3].PutMiddleRight(_middleRightText);

        EnableInstrumentsColliders(false);

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameOver)
        {
            // do nothing
        }
        else if (!_hasStarted)
        {
            StartCoroutine(StartingRoutine());
            for (int i = 0; i < _instruments.Count; i++)
            {
                _instruments[i].Tooltip.transform.parent.localScale = new Vector3(0, 0, 0);
            }
            _hasStarted = true;
        }
        else if (!_playerTurn && !_sequenceIsPlaying)
        {
            StartCoroutine(PlayCurrentSequence());
            _sequenceIsPlaying = true;
        }
        else if (_playerTurn)
        {
            StartCoroutine(PlayerCoRoutine());
        }
    }

    #endregion

    #region Fonctions d'aide

    /// <summary>
    /// Initialisation de tous les booléens et de la liste d'instruments
    /// </summary>
    void Init()
    {
        _currentIndexInSequence = 0;

        _igGameOver.enabled = false;
        _igSuccess.enabled = false;        

        _playerTurn = false;
        _sequenceIsPlaying = true;
        _hasStarted = false;
        _success = true;
        _gameOver = false;
        _isGamePaused = false;

        _errorCount = 0;
        _sequenceCount = 0;

        _sequence = new List<Instrument>();
    }

    /// <summary>
    /// Reset les booléens et la liste pour pouvoir recommencer le jeu depuis le début
    /// </summary>
    public void Restart()
    {
        Init();
        _sequenceIsPlaying = false;
        _hasStarted = true;

        _igInterface.enabled = true;
        foreach (Image life in _lifeSprites)
        {
            life.GetComponent<Animator>().SetBool("died", false);
        }
    }
    
    /// <summary>
    /// Met en pause ou reprend le jeu
    /// Utilisation du hack du timeScale à 0 pour "stopper" l'exécution du jeu.
    /// Note : les sons qui ont commencé se terminent
    /// </summary>
    public void PauseOrResume()
    {
        if (!_isGamePaused)
        {
            EnableInstrumentsColliders(false);
            Time.timeScale = 0;
            _isGamePaused = true;
            _countdownText.text = string.Empty;
            _igMenu.enabled = true;
        }
        else
        {
            _isGamePaused = false;
            _igMenu.enabled = false;
            Time.timeScale = 1;
            EnableInstrumentsColliders(true);
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
    /// Génère tout ce qui dépend de la difficulté
    /// </summary>
    /// <param name="difficulty"></param>
    void SetDifficulty(string difficulty)
    {
        switch(difficulty)
        {
            case DIFFICULTY_EASY:
                // On génère un instrument par catégorie
                foreach (string category in EASY_CATEGORIES)
                {
                    _instruments.Add(Category.GetRandomInstrumentInCategory(category));
                }
                break;
            case DIFFICULTY_MEDIUM:
                break;
            case DIFFICULTY_HARD:                
                break;
            default:
                System.Console.WriteLine(difficulty + " is not a valid difficulty input");
                break;
        }
    }

    /// <summary>
    /// Choisi un instrument au hasard parmi les 4 présents.
    /// Ne tombe jamais deux fois de suite sur le même pour éviter la répétition.
    /// </summary>
    /// <returns>L'instrument choisi au hasard</returns>
    Instrument PickRandomInstrument()
    {
        int currentIndex = _instruments.IndexOf(_chosenInstrument);
        int newIndex = Random.Range(0, _instruments.Count);
        while (newIndex == currentIndex)
        {
            newIndex = Random.Range(0, _instruments.Count);
        }

        return _instruments[newIndex];
    }

    /// <summary>
    /// Active ou désactive les colliders des instruments dans le but d'empêcher le joueur de cliquer quand il ne doit pas
    /// </summary>
    /// <param name="enabled">true pour activer, false pour désactiver</param>
    void EnableInstrumentsColliders(bool enabled)
    {
        foreach (Instrument instrument in _instruments)
        {
            instrument.Collider.enabled = enabled;
        }
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Co-routine pour jouer la séquence de démonstration qui rajoute une note supplémentaire à chaque fois
    /// </summary>
    /// <returns>L'enum de cette co-routine</returns>
    IEnumerator PlayCurrentSequence()
    {
        // Si success on incrémente la séquence d'une note
        if (_success)
        {
            if (_sequence.Count == _partition.NotesCount)
            {
                _igInterface.enabled = false;
                _igSuccess.enabled = true;
                yield break;
            }
            _chosenInstrument = PickRandomInstrument();
            _sequence.Add(_chosenInstrument);
        }
        // Sinon on agit en fonction du nombre d'erreurs du joueur
        else
        {
            // Erreurs autorisées
            if (_errorCount < MAX_ERRORS_ALLOWED + 1)
            {
                _lifeSprites[_errorCount - 1].GetComponent<Animator>().SetBool("died", true);
            }
            // Nombre d'erreurs autorisées dépassé --> GAME OVER
            else
            {
                // On cache l'interface et on affiche le game over
                _gameOver = true;
                _igInterface.enabled = false;
                _igGameOver.enabled = true;

                // On interrompt la co-routine à cet endroit - plus besoin de jouer de séquence.
                yield break;
            }            
        }

        // On anime tous les instruments pendant la séquence de démonstration
        for (int i = 0; i < _instruments.Count; i++)
            _instruments[i].StartAnimation(true);

        // On assigne le dernier instrument ajouté à la séquence
        _chosenInstrument = _sequence[0];

        // On arrête d'éventuels applaudissements / booos
        if (_audioSource.isPlaying)
            _audioSource.Stop();

        // On rejoue la séquence
        for (int i = 0; i < _sequence.Count; i++)
        {
            Note note = _partition.GetNoteAt(i);
            _audioSource.clip = Resources.Load<AudioClip>(note.GetFileNameFor(_sequence[i]));
            _audioSource.Play();
            yield return new WaitForSeconds(note.GetLengthInSeconds());
        }

        // On signale que c'est au joueur de répéter la séquenc et on réinitialise les booléens.
        _currentIndexInSequence = 0;
        _playerTurn = true;
        _sequenceIsPlaying = false;
        _success = false;

        // On arrête l'animation des instruments
        for (int i = 0; i < _instruments.Count; i++)
            _instruments[i].StopAnimation();

        EnableInstrumentsColliders(true);
    }

    /// <summary>
    /// Co-routine utilisée quand c'est au joueur de jouer.
    /// Sauvegarde les input de l'utilisateur pour garder la séquence en mémoire.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerCoRoutine()
    {
        bool clicked;
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
        if (clicked)
        {
            Ray ray = _mainCamera.ScreenPointToRay(clickedPosition);
            RaycastHit hit;
            Instrument selected;

            if (Physics.Raycast(ray, out hit))
            {
                if (_audioSource.isPlaying)
                    _audioSource.Stop();

                selected = _instruments[0];
                for (int i = 0; i < _instruments.Count; i++)
                {
                    if (hit.transform.name == _instruments[i].Name)
                        selected = _instruments[i];
                }                

                if (selected == _chosenInstrument)
                {
                    bool finished = false;
                    Note toPlay = _partition.GetNoteAt(_currentIndexInSequence);
                    _audioSource.clip = Resources.Load<AudioClip>(toPlay.GetFileNameFor(selected));
                    _audioSource.Play();

                    selected.StartAnimation(true);

                    if (_currentIndexInSequence != _sequence.Count - 1)
                    {
                        _currentIndexInSequence++;
                        _chosenInstrument = _sequence[_currentIndexInSequence];
                    }
                    else
                    {
                        finished = true;
                        EnableInstrumentsColliders(false);
                    }

                    yield return new WaitForSeconds(toPlay.GetLengthInSeconds());

                    if (finished)
                    {
                        _success = true;

                        // MAJ de la séquence max du joueur
                        _sequenceCount++;
                        _sequenceCountText.text = _sequenceCount.ToString();

                        _audioSource.Stop();
                        _audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_applause");
                        _audioSource.Play();
                        yield return new WaitForSeconds(3);
                        _playerTurn = false;
                    }

                    selected.StopAnimation();
                }
                else
                {
                    EnableInstrumentsColliders(false);
                    _audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_boo");
                    _audioSource.Play();                    
                    yield return new WaitForSeconds(_audioSource.clip.length);
                    _errorCount++;                    
                    _playerTurn = false;
                }
            }
        }
    }

    /// <summary>
    /// Co-routine lancée au démarrage - Countdown / bulles avec les noms des instruments
    /// </summary>
    /// <returns></returns>
    IEnumerator StartingRoutine()
    {
        _countdownText.text = "";
        for (int i = 0; i < 5; i++)
        {
            _countdownText.text = (5 - i).ToString();
            yield return new WaitForSeconds(1);
        }
        this._igMenu.enabled = false;
        _sequenceIsPlaying = false;
        _actionsPanel.SetActive(true);
        Color color = _menuBackground.color;
        color.a = 0.5f;
        _menuBackground.color = color;
    }

    #endregion
}
