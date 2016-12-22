using UnityEngine;
using System.Collections;
using System;

public class Tamtam : Instrument
{
    public Tamtam() : base(Instrument.TAMTAM) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(0, 48, 62);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(119, 48, 66);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(21, 48, 125);        
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(77, 48, 125);
    }
}
