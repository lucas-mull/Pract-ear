using UnityEngine;
using System.Collections;
using System;

public class Violoncelle : Instrument
{
    public Violoncelle() : base(Instrument.VIOLONCELLE,
        Category.GetInstance(Category.FROTTEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(30, 45, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(105, 45, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(40, 45, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(95, 45, 210);
    }
}
