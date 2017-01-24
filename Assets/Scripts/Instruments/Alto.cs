using UnityEngine;
using System.Collections;
using System;

public class Alto : Instrument
{
    public Alto() : base(Instrument.ALTO,
        Category.GetInstance(Category.FROTTEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(25, 38, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(100, 38, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(40, 38, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(90, 38, 210);
    }
}
