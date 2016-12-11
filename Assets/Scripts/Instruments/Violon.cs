using UnityEngine;
using System.Collections;
using System;

public class Violon : Instrument
{
    public Violon() : base(Instrument.VIOLON) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 82, 10);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(100, 82, 10);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(45, 82, 60);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 82, 60);
    }
}
