using UnityEngine;
using System.Collections;
using System;

public class Violon : Instrument
{
    public Violon() : base(Instrument.VIOLON) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(7.5f, 60, 55);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(120, 60, 55);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 60, 110);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 60, 110);
    }
}
