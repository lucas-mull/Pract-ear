using UnityEngine;
using System.Collections;
using System;

public class Marimba : Instrument
{
    public Marimba() : base(Instrument.MARIMBA) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(3, 47, 59);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(110, 47, 59);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 47, 150);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(90, 47, 150);
    }
}
