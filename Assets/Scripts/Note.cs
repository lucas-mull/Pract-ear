
/// <summary>
/// Classe pour définir une note de musique. Utilisé dans les partitions.
/// </summary>
public class Note
{
    const string SOURCE_FOLDER = "Notes/";

    const string LONGUEUR_NOIRE = "noire";
    const string LONGUEUR_BLANCHE = "blanche";
    const string LONGUEUR_CROCHE = "croche";

    #region Attributs

    string note;                // Intitulé de la note ("do", "ré", "mi", etc...)
    string longueur;            // Longueur de la note (cf. constantes ci-dessus)

    #endregion

    #region Propriétés

    // Propriétés pour les attributs - readonly.
    public string Name
    {
        get { return this.note; }
    }

    public string Length
    {
        get { return this.longueur; }
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur basique avec note et longueur
    /// </summary>
    /// <param name="note"></param>
    /// <param name="longueur"></param>
    public Note(string note, string longueur)
    {
        this.note = note;
        this.longueur = longueur;
    }

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Retourne le nom du fichier nécessaire pour jouer la note actuelle pour un instrument donné
    /// </summary>
    /// <param name="instrument">Instrument qui doit jouer la note</param>
    /// <returns>Le nom du fichier contenant la note pour l'instrument donné (ex: Notes/do_marimba)</returns>
    public string GetFileNameFor(Instrument instrument)
    {
        string folder = SOURCE_FOLDER + this.note + "/";
        string fileName = this.note.ToLower() + "_" + instrument.Name;

        return folder + fileName;
    }

    /// <summary>
    /// Renvoie la longueur de la note en secondes.
    /// </summary>
    /// <returns>
    ///     La durée en secondes de la note.
    ///     - Blanche = 1 seconde
    ///     - Noire = 0.5 secondes
    ///     - Croche = 0.25 secondes
    /// </returns>
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

    #endregion
}
