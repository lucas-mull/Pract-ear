using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cette classe est en majeure partie statique. Elle contient la définition de nos 4 arbres de familles pour les instruments.
/// Les arbres sont constitués UNIQUEMENT de CATEGORIES, aucun instrument.
/// Les catégories sont ensuite rajoutés aux instruments directement via leur attribut category.
/// Chaque objet de type InstrumentCategory fonctionne à la manière d'un arbre. Il contient les catégories enfants et parents.
/// </summary>
public class Category
{
    #region noms des catégories

    // VENTS
    private static Category Vents;
    public const string VENTS = "vents";
    public const string BOIS = "bois";
    public const string CUIVRES = "cuivres";
    public const string BISEAU = "biseau";
    public const string ANCHE_SIMPLE = "anche simple";
    public const string ANCH_DOUBLE = "anche double";

    // CORDES
    private static Category Cordes;
    public const string CORDES = "cordes";
    public const string FRAPPEES = "frappees";
    public const string PINCEES = "pincees";
    public const string FROTTEES = "frottees";

    // Percussion
    private static Category Percussions;
    public const string PERCUSSIONS = "percussions";
    public const string DETERMINE = "determine";
    public const string INDETERMINE = "indetermine";

    // Claviers
    private static Category Claviers;
    public const string CLAVIERS = "claviers";

    #endregion

    #region Attributs

    /// <summary>
    /// Indique si les 4 arbres principaux (statiques) ont été initialisés.
    /// </summary>
    static bool isInitialized = false;

    /// <summary>
    /// Nom de la catégorie
    /// </summary>
    string name;

    /// <summary>
    /// Parent de cet catégorie dans l'arbre
    /// </summary>
    Category parent;

    /// <summary>
    /// Enfants de cette catégorie dans l'arbre
    /// </summary>
    List<Category> children = new List<Category>();

    /// <summary>
    /// Indique si cette catégorie est la racine de l'arbre
    /// i.e. { Vents, Cordes, Percussions, Claviers }
    /// </summary>
    bool isRoot = false;

    #endregion

    #region Propriétés

    /// <summary>
    /// Renvoie le nombre d'enfants de cette catégorie
    /// </summary>
    public int ChildrenCount
    {
        get { return children.Count; }
    }

    #endregion

    #region Constructeurs

    // ! ATTENTION ! Cette classe est utilisé statiquement. Pour récupérer une catégorie, on utilise
    // TOUJOURS la méthode ci-dessous. Aucun constructeur public n'existe.

    /// <summary>
    /// Renvoie l'instance de la catégorie donnée depuis l'arbre.
    /// Cela permet d'avoir les parents et enfants correctement assignés sans avoir à recréer l'arbre à chaque appel.
    /// On passe la racine en paramètre pour savoir dans quel arbre chercher.
    /// </summary>
    /// <param name="categoryName">Nom de la catégorie à laquelle l'instrument appartient DIRECTEMENT (donc celle juste au dessus dans l'arbre)</param>
    /// <returns>La catégorie demandée, null si elle n'existe pas dans l'arbre de racine donné</returns>
    public static Category GetInstance(string categoryName)
    {
        // On initialise nos 4 arbres au premier appel à GetInstance
        // Il est également possible de les initialisés soi-même en appelant InitArbres() à nimporte quel moment
        InitArbres();

        switch (categoryName.ToLower())
        {
            case VENTS:
            case BOIS:
            case CUIVRES:
            case BISEAU:
            case ANCH_DOUBLE:
            case ANCHE_SIMPLE:
                return FindCategoryInTree(categoryName, Vents);
            case CORDES:
            case PINCEES:
            case FRAPPEES:
            case FROTTEES:
                return FindCategoryInTree(categoryName, Cordes);
            case PERCUSSIONS:
            case DETERMINE:
            case INDETERMINE:
                return FindCategoryInTree(categoryName, Percussions);
            case CLAVIERS:
                return Claviers;
            default:
                Debug.Log("Catégorie inconnue");
                return null;
        }
    }

    /// <summary>
    /// Constructeur privé ! Uniquement utilisé pour l'initialisation des 4 arbres.
    /// </summary>
    /// <param name="name">nom de la catégorie à créer</param>
    private Category(string name)
    {
        this.name = name.ToLower();
    }

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Ajoute un enfant à la catégorie
    /// </summary>
    /// <param name="category">enfant à ajouter</param>
    public void AddChild(Category category)
    {
        if (category != null)
        {
            // On assigne cette categorie comme parent sur la catégorie enfant
            category.parent = this;

            // Et on ajoute simplement l'enfant
            children.Add(category);
        }            
    }

    /// <summary>
    /// Renvoie la racine de l'arbre.
    /// Récursif.
    /// </summary>
    /// <returns>La racine de l'arbre</returns>
    public Category getRoot()
    {
        if (this.isRoot)
            return this;

        return this.parent.getRoot();
    }

    /// <summary>
    /// Indique si une catégorie est une sous-catégorie d'une autre donnée en paramètre.
    /// Récursif
    /// </summary>
    /// <param name="category">catégorie parente à rechercher</param>
    /// <returns>true si la catégorie est un enfant de la categorie donnée, false sinon</returns>
    public bool BelongsTo(string category)
    {
        if (category.ToLower() == this.name)
            return true;

        if (this.parent != null)
            return this.parent.BelongsTo(category);

        return false;
    }

    /// <summary>
    /// Nombre de catégories dans la partie inférieure du noeud.
    /// Récursif.
    /// </summary>
    /// <returns>Le nombre de catégories cumulées en dessous de ce noeud</returns>
    private int CategoriesBelow()
    {
        int count = 0;

        foreach (Category category in this.children)
        {
            count += 1 + category.CategoriesBelow();
        }

        return count;
    }

    /// <summary>
    /// Nombre de catégories totale dans l'arbre
    /// </summary>
    /// <returns>Le nombre de catégories total dans tout l'arbre</returns>
    public int TotalCategoriesInTree()
    {
        return getRoot().CategoriesBelow();
    }

    #endregion

    #region Méthodes statiques

    /// <summary>
    /// Récupère tous les instruments appartenant à une catégorie (enfants directs ou indirects)
    /// i.e. GetAllInstrumentsInCategory("vents") renverra tous les vents peu importe leur place dans l'arbre
    /// </summary>
    /// <param name="categoryName">Nom de la catégorie</param>
    /// <returns>Une liste de tous les instruments de cette catégorie</returns>
    public static List<Instrument> GetAllInstrumentsInCategory(string categoryName)
    {
        List<Instrument> subList = new List<Instrument>();
        foreach (Instrument instr in Instrument.GetInstrumentsList())
        {
            if (instr.Category.BelongsTo(categoryName))
            {
                subList.Add(instr);
            }
        }

        return subList;
    }

    /// <summary>
    /// Génère un instrument au hasard dans une catégorie donnée
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static Instrument GetRandomInstrumentInCategory(string categoryName)
    {
        List<Instrument> all = GetAllInstrumentsInCategory(categoryName);
        return all[Random.Range(0, all.Count)];
    }

    /// <summary>
    /// Génère une liste aléatoire d'un nombre d'instruments donné pour une catégorie donnée.
    /// Exemple: GetRandomInstrumentsInCategory("vents", 4) générera 4 instruments à vent au hasard.
    /// Si il n'y a pas assez d'instruments dans la catégorie, renvoie null.
    /// </summary>
    /// <param name="categoryName">Nom de la catégorie</param>
    /// <param name="numberToGenerate">Nombre d'instruments à générer depuis cette catégorie</param>
    /// <returns>Une liste contenant le nombre d'instruments demandé générée aléatoirement</returns>
    public static List<Instrument> GetRandomInstrumentsInCategory(string categoryName, int numberToGenerate)
    {
        List<Instrument> all = GetAllInstrumentsInCategory(categoryName);
        if (all.Count < numberToGenerate)
        {
            Debug.Log("Pas assez d'instruments à générer dans cette catégorie ! Total est " + all.Count + " - cible était " + numberToGenerate);
            return null;
        }

        // Si il y a pile poil le nombre on renvoie la liste telle quelle
        if (all.Count == numberToGenerate)
        {
            return all;
        }
        else
        {
            // Sinon on supprime un élément au hasard jusqu'à ce qu'on ait le bon compte
            while (all.Count > numberToGenerate)
            {
                all.RemoveAt(Random.Range(0, all.Count));
            }
        }

        return all;
    }

    /// <summary>
    /// Cherche une catégorie donnée dans un arbre donné
    /// </summary>
    /// <param name="categoryName">La catégorie à trouver </param>
    /// <param name="tree">L'arbre à fouiller</param>
    /// <returns>La catégorie si elle a été trouvée, null sinon</returns>
    private static Category FindCategoryInTree(string categoryName, Category tree)
    {
        Category res;
        // On cherche récursivement dans la partie supérieure de l'arbre
        res = SearchParents(categoryName, tree);
        if (res == null)
        {
            // Si on n'a rien trouvé on cherche dans la partie inférieure de l'arbre
            res = SearchChildren(categoryName, tree);
        }        

        return res;
    }

    /// <summary>
    /// Helper pour la méthode @FindCategoryInTree
    /// Cherche uniquement dans les enfants du noeud actuel.
    /// Récursif.
    /// </summary>
    /// <param name="categoryName">catégorie à trouver</param>
    /// <param name="tree">arbre à fouiller</param>
    /// <returns>La catégorie si elle a été trouvée, null sinon</returns>
    private static Category SearchChildren(string categoryName, Category tree)
    {
        if (tree.name == categoryName.ToLower())
            return tree;

        Category result = null;
        foreach (Category child in tree.children)
        {
            if (result == null)
                result = SearchChildren(categoryName, child);
        }

        return result;
    }

    /// <summary>
    /// Helper pour la méthode @FindCategoryInTree
    /// Cherche uniquement dans les parents du noeud actuel.
    /// Récursif.
    /// </summary>
    /// <param name="categoryName">catégorie à trouver</param>
    /// <param name="tree">arbre à fouiller</param>
    /// <returns>La catégorie si elle a été trouvée, null sinon</returns>
    private static Category SearchParents(string categoryName, Category tree)
    {
        if (tree.name == categoryName.ToLower())
            return tree;

        if (tree.parent != null)
            return FindCategoryInTree(categoryName, tree.parent);

        return null;
    }

    #endregion

    #region Initialisation des arbres

    /// <summary>
    /// Initialisation des 4 arbres
    /// </summary>
    private static void InitArbres()
    {
        if (!isInitialized)
        {
            InitArbreVents();
            InitArbreCordes();
            InitArbrePercussions();
            InitArbreClaviers();

            isInitialized = true;
        }
    }

    /// <summary>
    /// Méthode construisant l'arbre des vents
    /// </summary>
    /// <returns>l'arbre des vents</returns>
    public static void InitArbreVents()
    {
        Vents = new Category(VENTS);
        Vents.isRoot = true;

        // Bois
        Category bois = new Category(BOIS);
        bois.AddChild(new Category(BISEAU));
        bois.AddChild(new Category(ANCHE_SIMPLE));
        bois.AddChild(new Category(ANCH_DOUBLE));
        Vents.AddChild(bois);

        // Cuivres
        Vents.AddChild(new Category(CUIVRES));
    }

    /// <summary>
    /// Méthode construisant l'arbre des cordes
    /// </summary>
    /// <returns>l'arbre des cordes</returns>
    public static void InitArbreCordes()
    {
        Cordes = new Category(CORDES);
        Cordes.isRoot = true;

        Cordes.AddChild(new Category(FRAPPEES));
        Cordes.AddChild(new Category(PINCEES));
        Cordes.AddChild(new Category(FROTTEES));
    }

    /// <summary>
    /// Méthode construisant l'arbre des percussions
    /// </summary>
    /// <returns>l'arbre des percussions</returns>
    public static void InitArbrePercussions()
    {
        Percussions = new Category(PERCUSSIONS);
        Percussions.isRoot = true;

        Percussions.AddChild(new Category(DETERMINE));
        Percussions.AddChild(new Category(INDETERMINE));
    }

    /// <summary>
    /// Méthode construisant l'arbre des claviers
    /// </summary>
    /// <returns>l'arbre des claviers</returns>
    public static void InitArbreClaviers()
    {
        Claviers = new Category(CLAVIERS);
        Claviers.isRoot = true;
    }

    #endregion
}
