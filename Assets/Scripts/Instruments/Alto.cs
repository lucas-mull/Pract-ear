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
        return new Vector3(30, 45, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(100, 45, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(40, 45, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(90, 45, 210);
    }
}
