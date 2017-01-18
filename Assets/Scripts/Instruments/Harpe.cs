using UnityEngine;
using System.Collections;
using System;

public class Harpe : Instrument
{
    public Harpe() : base(Instrument.HARPE,
        Category.GetInstance(Category.PINCEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(10, 55, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 55, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 55, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 55, 210);
    }
}
