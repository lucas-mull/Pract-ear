using UnityEngine;
using System.Collections;
using System;

public class Trombone : Instrument
{
    public Trombone() : base(Instrument.TROMBONE,
        Category.GetInstance(Category.CUIVRES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 50, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(90, 50, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 50, 200);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 50, 200);
    }
}
