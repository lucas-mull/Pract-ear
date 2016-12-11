using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;

public class SimonLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    private string difficulty_level;

    public Instrument[] instruments = new Instrument[4];

    public List<Instrument> sequence = new List<Instrument>();

    Instrument chosenInstrument;

    public Partition partition;

    public AudioSource audioSource;

    public Camera mainCamera;

    bool playerTurn, sequenceIsPlaying, success = true;

    int currentIndexInSequence;

    // Use this for initialization
    void Start () {
        SetDifficulty(DIFFICULTY_EASY);

        instruments[0] = new Marimba();
        instruments[1] = new Trompette();
        instruments[2] = new Violon();
        instruments[3] = new Piano();

        partition = LoadPartitionFromJson("Partitions/clair_de_la_lune");
        partition.StartReading();

        PlaceInstrumentFarLeft(instruments[0]);
        PlaceInstrumentMiddleLeft(instruments[1]);
        PlaceInstrumentMiddleRight(instruments[2]);
        PlaceInstrumentFarRight(instruments[3]);

        audioSource = GetComponent<AudioSource>();

        playerTurn = false;
        sequenceIsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerTurn && !sequenceIsPlaying)
        {
            StartCoroutine(PlayCurrentSequence());
            sequenceIsPlaying = true;
        }
        else
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

    void PlaceInstrumentFarLeft(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getFarLeftVector(), instrument.Model.transform.rotation);
    }

    void PlaceInstrumentFarRight(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getFarRightVector(), instrument.Model.transform.rotation);
    }

    void PlaceInstrumentMiddleLeft(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getMiddleLeftVector(), instrument.Model.transform.rotation);
    }

    void PlaceInstrumentMiddleRight(Instrument instrument)
    {
        Instantiate(instrument.Model, instrument.getMiddleRightVector(), instrument.Model.transform.rotation);
    }

    public Partition LoadPartitionFromJson(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);
        JSONNode node = JSONNode.Parse(file.text);
        return new Partition(node);
    }

    public Instrument PickRandomInstrument()
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

    public void GenerateInstrumentSequence()
    {

    }

    IEnumerator PlayCurrentSequence()
    {
        if (success)
        {
            chosenInstrument = PickRandomInstrument();
            sequence.Add(chosenInstrument);
        }
        
        chosenInstrument = sequence[0];

        if (audioSource.isPlaying)
            audioSource.Stop();

        for (int i = 0; i < sequence.Count; i++)
        {
            Note note = partition.GetNoteAt(i);
            audioSource.clip = Resources.Load<AudioClip>(note.GetFileNameFor(sequence[i]));
            audioSource.Play();
            yield return new WaitForSeconds(note.GetLengthInSeconds());
        }

        currentIndexInSequence = 0;
        playerTurn = true;
        sequenceIsPlaying = false;
        success = false;
    }

    IEnumerator PlayerCoRoutine()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Instrument selected;

            if (Physics.Raycast(ray, out hit))
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                selected = instruments[0];
                for (int i = 0; i < instruments.Length; i++)
                {
                    if (hit.transform.name == instruments[i].Name + "(Clone)")
                        selected = instruments[i];
                }

                if (selected == chosenInstrument)
                {
                    Note toPlay = partition.GetNoteAt(currentIndexInSequence);
                    audioSource.clip = Resources.Load<AudioClip>(toPlay.GetFileNameFor(selected));
                    audioSource.Play();

                    selected.Animation.Play("Play");

                    yield return new WaitForSeconds(toPlay.GetLengthInSeconds());
                    //playingAnimation.Stop("Play");
                    if (currentIndexInSequence == sequence.Count - 1)
                    {
                        success = true;
                        audioSource.Stop();
                        audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_applause");
                        audioSource.Play();
                        yield return new WaitForSeconds(3);
                        playerTurn = false;
                    }
                    else
                    {
                        currentIndexInSequence++;
                        chosenInstrument = sequence[currentIndexInSequence];
                    }                    
                }
                else
                {
                    audioSource.clip = Resources.Load<AudioClip>("SFX/crowd_boo");
                    audioSource.Play();
                    yield return new WaitForSeconds(audioSource.clip.length);
                    playerTurn = false;
                }
            }
        }
    }
}
