using UnityEngine;
using System.Collections;
using System;

public class Violon : Instrument
{
    public Violon() : base(Instrument.VIOLON) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(5, 52, 65);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(125, 52, 70);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(25, 52, 127);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 52, 127);
    }
}
