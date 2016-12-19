using UnityEngine;
using SimpleJSON;

public class Extract
{
    const string PATH_TO_TRACKS = "Soundtracks/";

    private string clipFileName;
    private string title;
    private int instrumentsCount;
    private string[] instruments;

    public AudioClip Clip { get; set; }
    public int InstrumentsCount
    {
        get { return this.instrumentsCount; }
        set { this.instrumentsCount = value; }
    }

    public string[] InstrumentsNames
    {
        get { return this.instruments; }
        set { this.instruments = value; }
    }

    public Extract(JSONNode node)
    {
        // On récupère les trois premiers attributs classiquement
        this.title = node["title"];

        this.clipFileName = node["clipFileName"];
        this.SetAudioClip(this.clipFileName);

        this.instrumentsCount = node["instrumentsCount"].AsInt;
        this.instruments = new string[this.instrumentsCount];

        // On parse en Array pour les instruments, puis on parse chaque instrument individuellement
        JSONArray instrumentsArray = node["instruments"].AsArray;
        for (int i = 0; i < this.instrumentsCount; i++)
        {
            JSONNode instrument = instrumentsArray[i];
            this.instruments[i] = instrument["instrument"];
        }
    }

    public void SetAudioClip(string clipName)
    {
        if (this.Clip == null)
        {
            this.Clip = Resources.Load<AudioClip>(PATH_TO_TRACKS + clipName);
        }
    }

    public static Extract LoadExtraitFromJson(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(PATH_TO_TRACKS + fileName);
        JSONNode node = JSONNode.Parse(file.text);
        return new Extract(node);
    }

}
