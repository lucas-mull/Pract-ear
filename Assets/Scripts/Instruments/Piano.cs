using UnityEngine;
using System.Collections;
using System;

public class Piano : Instrument {

    public Piano() : base(Instrument.PIANO, 
        Category.GetInstance(Category.CLAVIERS)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(2, 64, 67);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(120, 64, 67);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(25, 64, 130);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 64, 135);
    }
}
