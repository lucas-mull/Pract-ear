using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

/// <summary>
/// Classe utilisée pour définir un Extrait de musique pour le blind test
/// </summary>
public class Extract
{

    const string PATH_TO_TAMTAM = "Soundtracks/Tamtam/";

    #region Attributs

    private string clipFileName;            // Nom du fichier pour le clip audio de l'extrait
    private string title;                   // Titre de l'extrait
    private int instrumentsCount;           // Nombre d'instruments jouant dans l'extrait
    private string[] instruments;           // Noms des instruments jouant dans l'extrait

    #endregion

    #region Propriétés

    /// <summary>
    /// AudioClip de l'extrait.
    /// </summary>
    public AudioClip Clip { get; set; }

    /// <summary>
    /// Nombre d'instruments présents dans l'extrait - Accesseurs à l'attribut correspondant
    /// </summary>
    public int InstrumentsCount
    {
        get { return this.instrumentsCount; }
        set { this.instrumentsCount = value; }
    }

    /// <summary>
    /// Noms des instruments présents dans l'extrait - Accesseurs à l'attribut correspondant
    /// </summary>
    public string[] InstrumentsNames
    {
        get { return this.instruments; }
        set { this.instruments = value; }
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur d'un extrait à partir du JSONNode à la racine du fichier
    /// </summary>
    /// <param name="node">Racine du fichier JSON</param>
    public Extract(JSONNode node)
    {
        // On récupère les trois premiers attributs classiquement
        this.title = node["title"];

        this.clipFileName = node["clipFileName"];
        this.SetAudioClip(node,this.clipFileName);

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

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Assigne l'AudioClip en fonction du nom du clip donné
    /// </summary>
    /// <param name="clipName">Nom du clip</param>
    public void SetAudioClip(JSONNode node, string clipName)
    {
        // A noter qu'on ne recrée pas le clip s'il existe déjà pour éviter de charger des ressources inutilement
        if (this.Clip == null)
        {
            this.Clip = Resources.Load<AudioClip>(node["path"] + clipName);
        }
    }

    #endregion

    #region Méthodes statiques

    /// <summary>
    /// Créée un objet Extract à partir d'un fichier JSON
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Extract LoadExtraitFromJson(string fileName, string PathToSoundtracks)
    {
        TextAsset file;
       
        file = Resources.Load<TextAsset>(PathToSoundtracks + fileName);
        JSONNode node = JSONNode.Parse(file.text);
        return new Extract(node);
    }

    public static List<Extract> LoadFalseExtraitsFromJson(string pathToJsons)
    {
        Object[] files;
        List<Extract> res = new List<Extract>();

        files = (Object[])Resources.LoadAll(pathToJsons);

        foreach (TextAsset t in files)
        {
            JSONNode node = JSONNode.Parse(t.text);
            res.Add(new Extract(node));
        }
        return res;
    }



    #endregion

}
