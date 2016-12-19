using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimonLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";
    const int MAX_ERRORS_ALLOWED = 3;

    #region Objets assignés à travers l'Inspector

    public Camera mainCamera;

    public Canvas igMenu;
    public Canvas igInterface;
    public Canvas igGameOver;
    public Canvas igSuccess;

    public Text farLeftText;
    public Text middleLeftText;
    public Text middleRightText;
    public Text farRightText;
    public Text countdownText;
    public Text sequenceCountText;

    public Image error1;
    public Image error2;
    public Image error3;

    public AudioSource audioSource;

    #endregion

    #region Variables privées

    private string difficulty_level;

    private Instrument[] instruments = new Instrument[4];           // Instruments présents sur la scène
    private List<Instrument> sequence;                              // Séquence actuelle des instruments

    private Instrument chosenInstrument;                            // Instrument choisi pour la prochaine note

    private Partition partition;                                    // Partition jouée

    // Différents booléens qui servent à réguler le jeu
    bool playerTurn, sequenceIsPlaying, hasStarted, success, gameOver;

    Sprite errorSprite;

    int currentIndexInSequence;                                     // Index actuelle dans la liste d'instruments
    int errorCount;                                                 // Nombre d'erreurs effectuées par l'utilisateur
    int sequenceCount;                                              // Nombre de notes enchaînées correctement par le joueur

    #endregion

    // Use this for initialization
    void Start () {
        SetDifficulty(DIFFICULTY_EASY);

        errorSprite = Resources.Load<Sprite>("Sprites/error");

        Init();

        instruments[0] = new Marimba();
        instruments[1] = new Trompette();
        instruments[2] = new Violon();
        instruments[3] = new Piano();

        partition = Partition.LoadPartitionFromJson("clair_de_la_lune");

        instruments[0].PutFarLeft(farLeftText);
        instruments[1].PutFarRight(farRightText);
        instruments[2].PutMiddleLeft(middleLeftText);
        instruments[3].PutMiddleRight(middleRightText);

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Initialisation de tous les booléens et de la liste d'instruments
    /// </summary>
    void Init()
    {
        currentIndexInSequence = 0;

        igGameOver.enabled = false;
        igSuccess.enabled = false;

        playerTurn = false;
        sequenceIsPlaying = true;
        hasStarted = false;
        success = true;
        gameOver = false;

        errorCount = 0;
        sequenceCount = 0;

        sequence = new List<Instrument>();
    }

    /// <summary>
    /// Reset les booléens et la liste pour pouvoir recommencer le jeu depuis le début
    /// </summary>
    public void Restart()
    {
        Init();
        sequenceIsPlaying = false;
        hasStarted = true;

        igInterface.enabled = true;
    }    

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            // do nothing
        }
        else if (!hasStarted)
        {
            StartCoroutine(StartingRoutine());
            for (int i = 0; i < instruments.Length; i++)
            {
                instruments[i].Tooltip.transform.parent.localScale = new Vector3(0, 0, 0);
            }            
            hasStarted = true;
        }
        else if (!playerTurn && !sequenceIsPlaying)
        {
            StartCoroutine(PlayCurrentSequence());
            sequenceIsPlaying = true;
        }
        else if (playerTurn)
        {
            StartCoroutine(PlayerCoRoutine());
        }
    }

    void SetDifficulty(string difficulty)
    {
        switch(difficulty)
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

    /// <summary>
    /// Choisi un instrument au hasard parmi les 4 présents.
    /// Ne tombe jamais deux fois de suite sur le même pour éviter la répétition.
    /// </summary>
    /// <returns>L'instrument choisi au hasard</returns>
    Instrument PickRandomInstrument()
    {
        int index = Random.Range(0, 4);
        Instrument picked = instruments[index];
        while (picked == chosenInstrument)
        {
            index = Random.Range(0, 4);
            picked = instruments[index];
        }

        return picked;
    }

    void GenerateInstrumentSequence()
    {

    }

    /// <summary>
    /// Co-routine pour jouer la séquence de démonstration qui rajoute une note supplémentaire à chaque fois
    /// </summary>
    /// <returns>L'enum de cette co-routine</returns>
    IEnumerator PlayCurrentSequence()
    {
        // Si success on incrémente la séquence d'une note
        if (success)
        {
            if (sequence.Count == partition.NotesCount)
            {
                igInterface.enabled = false;
                igSuccess.enabled = true;
                yield break;
            }
            chosenInstrument = PickRandomInstrument();
            sequence.Add(chosenInstrument);
        }
        // Sinon on agit en fonction du nombre d'erreurs du joueur
        else
        {
            // Erreurs autorisées
            if (errorCount < MAX_ERRORS_ALLOWED + 1)
            {                
                Image target;
                switch (errorCount)
                {
                    case 1:
                        target = error1;
                        break;
                    case 2:
                        target = error2;
                        break;
                    default:
                        target = error3;
                        break;
                }

                target.sprite = errorSprite;
            }
            // Nombre d'erreurs autorisées dépassé --> GAME OVER
            else
            {
                // On cache l'interface et on affiche le game over
                gameOver = true;
                igInterface.enabled = false;
                igGameOver.enabled = true;

                // On interrompt la co-routine à cet endroit - plus besoin de jouer de séquence.
                yield break;
            }            
        }

        // On anime tous les instruments pendant la séquence de démonstration
        for (int i = 0; i < instruments.Length; i++)
            instruments[i].StartAnimation(true);

        // On assigne le dernier instrument ajouté à la séquence
        chosenInstrument = sequence[0];

        // On arrête d'éventuels applaudissements / booos
        if (audioSource.isPlaying)
            audioSource.Stop();

        // On rejoue la séquence
        for (int i = 0; i < sequence.Count; i++)
        {
            Note note = partition.GetNoteAt(i);
            audioSource.clip = Resources.Load<AudioClip>(note.GetFileNameFor(sequence[i]));
            audioSource.Play();
            yield return new WaitForSeconds(note.GetLengthInSeconds());
        }

        // On signale que c'est au joueur de répéter la séquenc et on réinitialise les booléens.
        currentIndexInSequence = 0;
        playerTurn = true;
        sequenceIsPlaying = false;
        success = false;

        // On arrête l'animation des instruments
        for (int i = 0; i < instruments.Length; i++)
            instruments[i].StopAnimation();
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
            Ray ray = mainCamera.ScreenPointToRay(clickedPosition);
            RaycastHit hit;
            Instrument selected;

            if (Physics.Raycast(ray, out hit))
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                selected = instruments[0];
                for (int i = 0; i < instruments.Length; i++)
                {
                    if (hit.transform.name == instruments[i].Name)
                        selected = instruments[i];
                }

                if (selected == chosenInstrument)
                {
                    bool finished = false;
                    Note toPlay = partition.GetNoteAt(currentIndexInSequence);
                    audioSource.clip = Resources.Load<AudioClip>(toPlay.GetFileNameFor(selected));
                    audioSource.Play();

                    selected.StartAnimation(true);

                    if (currentIndexInSequence != sequence.Count - 1)
                    {
                        currentIndexInSequence++;
                        chosenInstrument = sequence[currentIndexInSequence];
                    }
                    else
                    {
                        finished = true;
                    }

                    yield return new WaitForSeconds(toPlay.GetLengthInSeconds());

                    if (finished)
                    {
                        success = true;

                        // MAJ de la séquence max du joueur
                        sequenceCount++;
                        sequenceCountText.text = sequenceCount.ToString();

                        audioSource.Stop();
                        audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_applause");
                        audioSource.Play();
                        yield return new WaitForSeconds(3);
                        playerTurn = false;
                    }

                    selected.StopAnimation();
                }
                else
                {
                    audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_boo");
                    audioSource.Play();                    
                    yield return new WaitForSeconds(audioSource.clip.length);
                    errorCount++;                    
                    playerTurn = false;
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
        countdownText.text = "";
        for (int i = 0; i < 5; i++)
        {
            countdownText.text = (5 - i).ToString();
            yield return new WaitForSeconds(1);
        }
        this.igMenu.enabled = false;
        sequenceIsPlaying = false;
    }   
}
