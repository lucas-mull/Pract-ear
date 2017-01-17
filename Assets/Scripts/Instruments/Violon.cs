using UnityEngine;
using System.Collections;
using System;

public class Violon : Instrument
{
    public Violon() : base(Instrument.VIOLON,
        Category.GetInstance(Category.FROTTEES)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(25, 38, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 38, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(35, 38, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(85, 38, 210);
    }
}
