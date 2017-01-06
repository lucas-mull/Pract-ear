using UnityEngine;
using SimpleJSON;


public class Partition
{
    const string PATH_TO_PARTITIONS = "Partitions/";

    #region Attributs

    private string title;       // Titre de la partition
    private int notesCount;     // Nombre total de notes dans la partition
    private Note[] notes;       // Tableau des notes de la partition
    private int currentRead;    // Index de la note actuelle dans la partition (lorsqu'une lecture est en cours)

    #endregion

    #region Propriétés

    public string Title
    {
        get { return this.title; }
        set { this.title = value; }
    }

    public int NotesCount
    {
        get { return this.notesCount; }
        set { this.notesCount = value; }
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur manuel
    /// </summary>
    /// <param name="title">Titre</param>
    /// <param name="notes">Notes</param>
    /// <param name="notesCount">Nombre de notes</param>
    public Partition(string title, Note[] notes, int notesCount)
    {
        this.title = title;
        this.notes = notes;
        this.notesCount = notesCount;
    }

    /// <summary>
    /// Constructeur pour créer une partition à partir d'un fichier JSON
    /// </summary>
    /// <param name="node">JSONNode à la base du fichier</param>
    public Partition(JSONNode node)
    {
        // On récupère les trois premiers attributs classiquement
        this.title = node["title"];
        this.notesCount = node["notesCount"].AsInt;
        this.notes = new Note[this.notesCount];

        // On parse en Array pour les notes, puis on parse chaque note individuellement
        JSONArray notesArray = node["notes"].AsArray;
        for (int i = 0; i < this.notesCount; i++)
        {
            JSONNode note = notesArray[i];
            this.notes[i] = new Note(note["note"], note["longueur"]);
        }
    }

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Initialise la lecture d'une partition
    /// </summary>
    public void StartReading()
    {
        currentRead = 0;
    }

    /// <summary>
    /// Renvoie la prochaine note dans la partition, et incrémente l'index de lecture.
    /// </summary>
    /// <returns>La note suivante à jouer dans la partition</returns>
    public Note ReadNextNote()
    {
        // Si on est à la fin de la lecture, on revient au début
        if (currentRead == notesCount)
            StartReading();

        Note next = this.notes[currentRead];
        currentRead++;

        return next;
    }

    /// <summary>
    /// Récupère la note à l'index donné de la partition
    /// </summary>
    /// <param name="index">index de la note à récupérer</param>
    /// <returns>La note se trouvant à l'index donné. Si on est au dela des limites, renvoie null</returns>
    public Note GetNoteAt(int index)
    {
        if (index < this.notesCount)
            return this.notes[index];

        return null;
    }

    #endregion

    #region Méthodes statiques

    /// <summary>
    /// Charge une partition à partir d'un fichier JSON
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Partition LoadPartitionFromJson(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(PATH_TO_PARTITIONS + fileName);
        JSONNode node = JSONNode.Parse(file.text);
        return new Partition(node);
    }

    #endregion

}
