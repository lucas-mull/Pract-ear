using UnityEngine;
using System.Collections;
using System;

public class Piano : Instrument {

    public Piano() : base(Instrument.PIANO) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(14, 64, 43);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(115, 64, 43);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(29, 64, 150);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(79, 64, 150);
    }
}
