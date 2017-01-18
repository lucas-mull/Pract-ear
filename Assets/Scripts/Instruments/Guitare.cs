using UnityEngine;
using System.Collections;
using System;

public class Guitare : Instrument
{
    public Guitare() : base(Instrument.GUITARE,
        Category.GetInstance(Category.PINCEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 40, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(85, 40, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 40, 190);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 40, 190);
    }
}
