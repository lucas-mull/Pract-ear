using UnityEngine;
using System.Collections;
using System;

public class Violon : Instrument
{
    public Violon() : base(Instrument.VIOLON) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 82, 20);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(110, 82, 20);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(40, 82, 70);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(90, 82, 70);
    }
}
