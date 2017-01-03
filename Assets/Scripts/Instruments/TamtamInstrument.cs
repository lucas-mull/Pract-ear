using System.Collections.Generic;
using UnityEngine;

public class TamtamInstrument : BlindTestInstrument {

    // attribut indiquant si le tamtam est l'intru
    public bool isIntrus = false;
    public static int tamtamId = 1;
    public Extract  extrait { get; set; }


  
	
    public TamtamInstrument(bool isIntru) : base(Instrument.TAMTAM)
    {
        this.isIntrus = isIntru;
        this.Instrument.Name += tamtamId;
        tamtamId++;
    }

    public TamtamInstrument(bool isIntru, Extract extract) : this(isIntru)
    {
        this.extrait = extract;
    }

    public TamtamInstrument(bool isIntru, Extract extract, Light spotlight) : this(isIntru, extract)
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
        if (isIntrus && this.isLit)
        {
            this.SpotLight.color = Color.green;
            return true;
        }
        else if (!isIntrus && this.isLit)
        {
            this.SpotLight.color = Color.red;
            return false;
        }
        else if (isIntrus && !this.isLit)
        {
            this.ToggleLight();
            this.SpotLight.color = Color.green;
            return false;
        }
        else
            return true;
    }

  
}
