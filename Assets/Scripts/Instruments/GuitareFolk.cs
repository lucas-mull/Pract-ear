using UnityEngine;
using System.Collections;
using System;

public class GuitareFolk : Instrument
{
    public GuitareFolk() : base(Instrument.GUITARE_FOLK,
        Category.GetInstance(Category.PINCEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(15, 40, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(90, 40, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(25, 40, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 40, 210);
    }
}
