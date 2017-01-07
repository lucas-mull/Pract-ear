using UnityEngine;
using System.Collections;
using System;

public class Marimba : Instrument
{
    public Marimba() : base(Instrument.MARIMBA, 
        Category.GetInstance(Category.DETERMINE)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(3, 47, 59);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(117, 47, 59);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(20, 47, 120);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(77, 47, 120);
    }
}
