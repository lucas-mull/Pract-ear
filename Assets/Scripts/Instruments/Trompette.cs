using UnityEngine;
using System.Collections;
using System;

public class Trompette : Instrument
{
    public Trompette() : base(Instrument.TROMPETTE) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(7.5f, 58, 48);
    }

    public override Vector3 getFarRightVector()
    {        
        return new Vector3(115, 58, 48);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(25, 58, 100);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 58, 100);
    }
}
