using UnityEngine;

/// <summary>
/// Classe utilisée pour les instruments du jeu OrchestraBlindtest
/// Essentiellement rajoute une spotlight associé à l'instrument pour que ce soit plus facile à toggle on / off.
/// </summary>
public class BlindTestInstrument
{
    // Attribut indiquant si l'instrument est présent dans l'extrait joué
    public bool isInExtract = false;

    #region Propriétés

    public Instrument Instrument { get; set; }      // Instrument correspondant à cet objet
    public Light SpotLight { get; set; }            // Spot se situant au dessus de cet instrument
    public GameObject Instance                      // Instance de cet instrument
    {
        get { return this.Instrument.Instance; }
    }

    public bool isLit                               // Permet de savoir si l'instrument est actuellement illuminé
    {
        get
        {
            if (this.SpotLight != null)
            {
                return this.SpotLight.enabled;
            }

            return false;
        }        
    }

    #endregion

    #region Constructeurs

    /// <summary>
    /// Constructeur basique à partir du nom de l'instrument
    /// </summary>
    /// <param name="instrumentName">Nom de l'instrument à créer</param>
    public BlindTestInstrument(string instrumentName)
    {
        this.Instrument = Instrument.GetInstanceFor(instrumentName.ToLower());
    }

    /// <summary>
    /// Constructeur précisant si l'instrument est dans l'extrait ou pas
    /// </summary>
    /// <param name="instrumentName">Nom de l'instrument à créer</param>
    /// <param name="isInExtract">true si l'instrument est dans l'extrait, false sinon</param>
    public BlindTestInstrument(string instrumentName, bool isInExtract) : this(instrumentName)
    {
        this.isInExtract = isInExtract;
    }

    /// <summary>
    /// Constructeur avec tous les paramètres
    /// </summary>
    /// <param name="instrumentName">Nom de l'instrument à créer</param>
    /// <param name="spotLight">Spot à toggle pour cet instrument</param>
    /// <param name="isInExtract">true si l'instrument est dans l'extrait, false sinon</param>
    public BlindTestInstrument(string instrumentName, Light spotLight, bool isInExtract) : this(instrumentName, isInExtract)
    {
        this.SpotLight = spotLight;
    }

    #endregion

    #region Méthodes de classe

    /// <summary>
    /// Allume ou éteint le spot de cet instrument en fonction de la valeur du booléen 'on'
    /// </summary>
    /// <param name="on">true pour allumer la lumière, false pour l'éteindre</param>
    public void ToggleLight(bool on)
    {
        if (this.SpotLight != null)
        {
            this.SpotLight.color = Color.white;
            this.SpotLight.enabled = on;
        }        
    }

    /// <summary>
    /// Surcharge de la méthode précédente.
    /// Si la lumière est éteinte, on l'allume, sinon on l'éteint.
    /// </summary>
    public void ToggleLight()
    {
        this.ToggleLight(!isLit);
    }

    /// <summary>
    /// Allumer le spot 'réponse' pour cet instrument en fonction de si la question est affirmative ou pas.
    /// Système potentiellement à revoir. Autre solution plus simple et peut-être plus intuitive serait simplement
    /// d'allumer les réponses correctes en vert et les mauvaises en rouge, sans tenir compte de la sélection de l'utilisateur
    /// - S'il faut trouver les instruments présents dans le morceau:
    ///     - Si l'instrument est sélectionné et présent --> VERT
    ///     - Si l'instrument est sélectionné et pas présent --> ROUGE
    ///     - Si l'instrument n'est pas sélectionné et présent --> VERT
    ///     - Si l'instrument n'est pas sélectionné est absent --> AUCUN SPOT
    /// - S'il faut trouver les instruments absents du morceau:
    ///     - Si l'instrument est sélectionné est absent --> VERT
    ///     - Si l'instrument est sélectionné et présent --> ROUGE
    ///     - Si l'instrument n'est pas sélectionné et absent --> VERT
    ///     - Si l'instrument n'est pas sélectionné et présent --> AUCUN SPOT
    /// </summary>
    /// <param name="affirmative"></param>
    /// <returns></returns>
    public bool ToggleLightAnswerFor(bool affirmative)
    {
        if (isInExtract && this.isLit)
        {
            if (affirmative)
            {
                this.SpotLight.color = Color.green;
                return true;
            }
            else
            {
                this.SpotLight.color = Color.red;
                return false;
            }
        }
        else if (!isInExtract && this.isLit)
        {
            if (!affirmative)
            {
                this.SpotLight.color = Color.green;
                return true;
            }
            else
            {
                this.SpotLight.color = Color.red;
                return false;
            }
        }
        else if (!this.isLit && isInExtract && affirmative)
        {
            this.ToggleLight();
            this.SpotLight.color = Color.green;
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion
}
