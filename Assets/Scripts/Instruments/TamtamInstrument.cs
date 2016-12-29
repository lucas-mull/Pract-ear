using UnityEngine;

public class TamtamInstrument : BlindTestInstrument {

    // attribut indiquant si le tamtam est l'intru
    public bool isIntru = false;

    public TamtamInstrument(string instrumentName) :base(instrumentName)
    {
    }
	
    public TamtamInstrument(string instrumentName, bool isIntru) : this(instrumentName)
    {
        this.isIntru = isIntru;
    }

    public TamtamInstrument(string instrumentName, bool isIntru, Light spotlight) : this(instrumentName, isIntru)
    {
        this.SpotLight = spotlight;
    }

    /// <summary>
    /// Méthode dérivée de la classe BlindTestInstrument
    /// </summary>
    /// <param name="affirmative"></param>
    /// <returns></returns>

    public new bool ToggleLightAnswerFor(bool affirmative)
    {
        if (isIntru && this.isLit)
        {
            this.SpotLight.color = Color.green;
            return true;
        }
        else if (!isIntru && this.isLit)
        {
            this.SpotLight.color = Color.red;
            return false;
        }
        else if (isIntru && !this.isLit)
        {
            this.ToggleLight();
            this.SpotLight.color = Color.green;
            return false;
        }
        else
            return true;
    }
}
