using UnityEngine;

/// <summary>
/// Classe utilisée pour les instruments du jeu OrchestraBlindtest
/// Essentiellement rajoute une spotlight associé à l'instrument pour que ce soit plus facile à toggle on / off.
/// </summary>
public class BlindTestInstrument
{
    public bool isInExtract = false;

    public Instrument Instrument { get; set; }
    public Light SpotLight { get; set; }
    public GameObject Instance
    {
        get { return this.Instrument.Instance; }
    }

    public bool isLit
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

    public BlindTestInstrument(string instrumentName)
    {
        this.Instrument = Instrument.GetInstanceFor(instrumentName.ToLower());
    }

    public BlindTestInstrument(string instrumentName, bool isInExtract) : this(instrumentName)
    {
        this.isInExtract = isInExtract;
    }

    public BlindTestInstrument(string instrumentName, Light spotLight, bool isInExtract) : this(instrumentName)
    {
        this.SpotLight = spotLight;
        this.isInExtract = isInExtract;
    }

    public void ToggleLight(bool on)
    {
        if (this.SpotLight != null)
        {
            this.SpotLight.color = Color.white;
            this.SpotLight.enabled = on;
        }        
    }

    public void ToggleLight()
    {
        this.ToggleLight(!isLit);
    }

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
            this.SpotLight.color = Color.red;
            return false;
        }
        else
        {
            return true;
        }
    }
}
