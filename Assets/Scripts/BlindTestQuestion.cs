using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe utilisée pour générer des questions de Blind test et tout ce qui s'y rapporte
/// </summary>
public class BlindTestQuestion
{
    #region Attributs

    public bool affirmative = true;         // Indique si la question est affirmative (true) ou négative (false)

    public string[] inExtract;              // Regroupe les noms des instruments qui sont présents dans l'extrait
    public int totalInstrumentCount;        // Nombre d'instruments total jouant dans l'extrait

    #endregion

    #region Propriétés

    /// <summary>
    /// Propriété utilisant l'attribut 'inExtract'
    /// </summary>
    public string[] InstrumentsInExtract
    {
        get { return this.inExtract; }
        set { this.inExtract = value; }
    }

    /// <summary>
    /// Propriété générant la question appropriée en fonction de la valeur de 'affirmative'
    /// </summary>
    public string Question
    {
        get
        {
            if (affirmative)
                return "Quels instruments jouent dans le morceau ?";
            else
                return "Quels instruments sont absents du morceau ?";
        }
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur basique
    /// </summary>
    /// <param name="totalInstrumentCount">Nombre d'instruments présents dans l'extrait</param>
    public BlindTestQuestion(int totalInstrumentCount)
    {
        this.affirmative = System.Convert.ToBoolean(Random.Range(0, 2));
        this.totalInstrumentCount = totalInstrumentCount;
    }

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Génére une liste d'instruments aléatoires correspondant à la question
    /// </summary>
    /// <returns></returns>
    public List<BlindTestInstrument> GenerateInstrumentListForQuestion()
    {
        // Initialisation d'une liste de string avec les noms, et d'une liste d'objets de type BlindTestInstrument
        List<string> list = new List<string>();
        List<BlindTestInstrument> res = new List<BlindTestInstrument>();

        // Nombre d'instruments générés
        int currentCount = 0;

        // On ajout d'abord tous les instruments qui jouent dans l'extrait
        for (int i = 0; i < inExtract.Length; i++)
        {
            // On met à jour les deux listes
            list.Add(inExtract[i].ToLower());
            res.Add(new BlindTestInstrument(inExtract[i], true));

            // et on incrémente le nombre d'instruments
            currentCount++;
        }

        // Tant qu'il reste des instruments à générer...
        while (currentCount < this.totalInstrumentCount)
        {
            // On génére un instrument au hasard
            string randomInstrument = Instrument.GenerateRandomInstrumentName();

            // On réessaye jusqu'à ce que l'instrument généré n'existe pas déjà dans la liste
            while (list.Contains(randomInstrument))
            {
                randomInstrument = Instrument.GenerateRandomInstrumentName();
            }

            // Et on l'ajoute à la liste des noms
            list.Add(randomInstrument);
            currentCount++;
        }

        // On finit par créer les dernières instances de BlindTestInstrument pour les instruments précédents.
        for (int i = inExtract.Length; i < list.Count; i++)
        {
            res.Add(new BlindTestInstrument(list[i]));
        }

        return res;
    }

    #endregion
}
