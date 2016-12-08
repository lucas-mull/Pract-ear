using UnityEngine;
using System.Collections;
using System;

public class Marimba : Instrument
{
    public Marimba() : base(Instrument.MARIMBA) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(4, 42, 58);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(115, 42, 58);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(20, 42, 140);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(100, 42, 140);
    }
}
