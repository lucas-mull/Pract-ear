using UnityEngine;
using System.Collections;

public class OrchestraLoad : MonoBehaviour {

    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    private string difficulty_level;

    public Instrument instrument_1;
    public Instrument instrument_2;
    public Instrument instrument_3;
    public Instrument instrument_4;

    public AudioSource audioSource;

    public Camera mainCamera;

    // Use this for initialization
    void Start()
    {
        setDifficulty(DIFFICULTY_EASY);

        instrument_1 = new Marimba();
        instrument_2 = new Trompette();
        instrument_3 = new Violon();
        instrument_4 = new Piano();

        placeInstrumentFarLeft(instrument_4);
        placeInstrumentMiddleLeft(instrument_2);
        placeInstrumentMiddleRight(instrument_3);
        placeInstrumentFarRight(instrument_1);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
	
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
}
