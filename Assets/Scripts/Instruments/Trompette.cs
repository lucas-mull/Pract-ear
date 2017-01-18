using UnityEngine;
using System.Collections;
using System;

public class Trompette : Instrument
{
    public Trompette() : base(Instrument.TROMPETTE,
        Category.GetInstance(Category.CUIVRES)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 35, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 35, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(33, 35, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 35, 210);
    }
}
