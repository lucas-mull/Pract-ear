using UnityEngine;
using UnityEngine.UI;

public abstract class Instrument {

    const string PATH_TO_PREFABS = "Prefabs/";

    // Noms des prefabs des instruments dans Assets/Resources/Prefabs
    public static string PIANO = "piano";
    public static string TROMPETTE = "trompette";
    public static string VIOLON = "violon";
    public static string MARIMBA = "marimba";

    #region Attributs

    /// <summary>
    /// Utilisé pour stocker l'instance de l'object découlant de la prefab.
    /// En gros, contient l'instance du GameObject renvoyé par la méthode Object.Instantiate
    /// cf. placeInstrumentFarLeft, etc...
    /// </summary>
    private GameObject instance;

    // Composant UI.Text contenant les noms des instruments au lancement
    private Text toolTip;

    /// <summary>
    /// Modèle (GameObject) de l'instrument.
    /// </summary>
    private GameObject model;

    /// <summary>
    /// Attribut indiquant si notre instrument possède une instance existante sur la scène.
    /// Accessible en dehors de la classe.
    /// </summary>
    public bool isInstantiated;

    #endregion

    #region Propriétés

    /// <summary>
    /// Propriété pour l'attribut privé 'instance'
    /// </summary>
    public GameObject Instance
    {
        set { this.instance = value; }
        get { return this.instance; }
    }

    /// <summary>
    /// Propriété pour l'attribut privé 'toolTip'
    /// </summary>
    public Text Tooltip
    {
        get { return this.toolTip; }
        set
        {
            this.toolTip = value;
            this.toolTip.text = this.Name;
        }
    }

    /// <summary>
    /// Propriété pour le nom du GameObject
    /// </summary>
    public string Name
    {
        get { return this.Model.name; }
        set { this.Model.name = value; }
    }

    /// <summary>
    /// Propriété représentant le component Animator du GameObject
    /// </summary>
    public Animator Animator
    {
        get { return this.Instance.GetComponent<Animator>(); }
    }

    /// <summary>
    /// Propriété pour l'attribut 'model'
    /// </summary>
    public GameObject Model {
        get { return this.model; }
        set { this.model = value; }
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    public Instrument()
    {
        this.isInstantiated = false;
    }

    /// <summary>
    /// Constructeur simple
    /// </summary>
    /// <param name="modelName">Nom de l'instrument pour lequel on souhaite récupérer la prefab</param>
    public Instrument(string modelName) : this()
    {
        this.setModelFromName(modelName);
        this.Name = modelName;
    }

    #endregion

    #region Méthodes abstraites

    // Méthodes abstraites pour les 4 positions possible de chaque instrument
    public abstract Vector3 getFarLeftVector();
    public abstract Vector3 getFarRightVector();
    public abstract Vector3 getMiddleLeftVector();
    public abstract Vector3 getMiddleRightVector();

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Récupération du GameObject depuis les resources du projet pour l'instrument actuel en fonction du nom donné.
    /// </summary>
    /// <param name="modelName">Nom de l'instrument</param>
    /// <returns>true si le GameObject a pu être chargé, false sinon</returns>
    public bool setModelFromName(string modelName)
    {
        this.Model = Resources.Load(PATH_TO_PREFABS + modelName, typeof(GameObject)) as GameObject;
        return this.Model != null;
    }

    /// <summary>
    /// Créé une instance de la prefab de l'instrument actuel à la position donnée
    /// Si une instance existe déjà, la déplace à la position désirée
    /// </summary>
    /// <param name="position">position à laquelle créer l'instance de l'objet</param>
    /// <returns>Le GameObject contenant l'instance de la prefab</returns>
    public GameObject InstantiateOrMoveAt(Vector3 position)
    {
        if (!isInstantiated)
        {
            this.Instance = (GameObject)Object.Instantiate(this.Model, position, this.Model.transform.rotation);
        }
        else
        {
            this.Instance.transform.position = getFarLeftVector();
            isInstantiated = true;
        }

        return this.Instance;
    }

    /// <summary>
    /// Créée une instance de l'instrument actuel le plus à gauche de la scène.
    /// Si une instance existe déjà, la déplace le plus à gauche de la scène.
    /// </summary>
    /// <param name="toolTip">UI Text associé à cet instrument dans la scène.</param>
    /// <returns>Le GameObject correspondant à l'instance de l'instrument créée / déplacée</returns>
    public GameObject PutFarLeft(Text toolTip)
    {
        this.Tooltip = toolTip;
        return InstantiateOrMoveAt(getFarLeftVector());
    }

    /// <summary>
    /// Créée une instance de l'instrument actuel le plus à droite de la scène.
    /// Si une instance existe déjà, la déplace le plus à droite de la scène.
    /// </summary>
    /// <param name="toolTip">UI Text associé à cet instrument dans la scène.</param>
    /// <returns>Le GameObject correspondant à l'instance de l'instrument créée / déplacée</returns>
    public GameObject PutFarRight(Text toolTip)
    {
        this.Tooltip = toolTip;
        return InstantiateOrMoveAt(getFarRightVector());
    }

    /// <summary>
    /// Créée une instance de l'instrument actuel au milieu à gauche de la scène.
    /// Si une instance existe déjà, la déplace au milieu à gauche de la scène.
    /// </summary>
    /// <param name="toolTip">UI Text associé à cet instrument dans la scène.</param>
    /// <returns>Le GameObject correspondant à l'instance de l'instrument créée / déplacée</returns>
    public GameObject PutMiddleLeft(Text toolTip)
    {
        this.Tooltip = toolTip;
        return InstantiateOrMoveAt(getMiddleLeftVector());
    }

    /// <summary>
    /// Créée une instance de l'instrument actuel au milieu à droite de la scène.
    /// Si une instance existe déjà, la déplace au milieu à droite de la scène.
    /// </summary>
    /// <param name="toolTip">UI Text associé à cet instrument dans la scène.</param>
    /// <returns>Le GameObject correspondant à l'instance de l'instrument créée / déplacée</returns>
    public GameObject PutMiddleRight(Text toolTip)
    {
        this.Tooltip = toolTip;
        return InstantiateOrMoveAt(getMiddleRightVector());
    }

    /// <summary>
    /// GarbageCollect cet objet
    /// </summary>
    public void Destroy()
    {
        Resources.UnloadUnusedAssets();
    }

    #endregion
}
