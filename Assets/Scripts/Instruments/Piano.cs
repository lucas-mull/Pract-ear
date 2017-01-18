using UnityEngine;
using System.Collections;
using System;

public class Piano : Instrument {

    public Piano() : base(Instrument.PIANO, 
        Category.GetInstance(Category.CLAVIERS)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 40, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 40, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 40, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(85, 40, 210);
    }
}
