using UnityEngine;
using System.Collections;
using System;

public class Piano : Instrument {

    public Piano() : base(Instrument.PIANO) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(-5, 64, 70);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(120, 64, 70);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 64, 150);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 64, 150);
    }
}
