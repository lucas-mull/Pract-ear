using UnityEngine;
using System.Collections;
using System;

public class Trompette : Instrument
{
    public Trompette() : base(Instrument.TROMPETTE) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(0, 50, 62);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(120, 50, 65);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(25, 50, 125);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 50, 125);
    }
}
