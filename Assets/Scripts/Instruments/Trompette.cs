using UnityEngine;
using System.Collections;
using System;

public class Trompette : Instrument
{
    public Trompette() : base(Instrument.TROMPETTE) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(95, 82, -50);
    }

    public override Vector3 getFarRightVector()
    {        
        return new Vector3(195, 82, -50);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(110, 82, 9);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(180, 82, 9);
    }
}
