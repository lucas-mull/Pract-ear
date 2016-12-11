using System;

public class Note
{
    const string SOURCE_FOLDER = "Notes/";

    const string LONGUEUR_NOIRE = "noire";
    const string LONGUEUR_BLANCHE = "blanche";
    const string LONGUEUR_CROCHE = "croche";

    string note;
    string longueur;

    public string Name
    {
        get { return this.note; }
    }

    public string Length
    {
        get { return this.longueur; }
    }

    public Note(string note, string longueur)
    {
        this.note = note;
        this.longueur = longueur;
    }

    public string GetFileNameFor(Instrument instrument)
    {
        string folder = SOURCE_FOLDER + this.note + "/";
        string fileName = this.note.ToLower() + "_" + instrument.Name;

        return folder + fileName;
    }

    public float GetLengthInSeconds()
    {
        switch(this.Length)
        {
            case LONGUEUR_BLANCHE:
                return 1.0f;
            case LONGUEUR_NOIRE:
                return 0.5f;
            case LONGUEUR_CROCHE:
                return 0.25f;
            default:
                return 1.0f;
        }
    }
}
