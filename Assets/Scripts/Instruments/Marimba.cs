using UnityEngine;
using System.Collections;
using System;

public class Marimba : Instrument
{
    public Marimba() : base(Instrument.MARIMBA, 
        Category.GetInstance(Category.DETERMINE)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 32, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(97, 32, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(35, 32, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 32, 210);
    }
}
