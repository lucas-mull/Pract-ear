using UnityEngine;
using System.Collections;
using System;

public class Contrebasse : Instrument
{
    public Contrebasse() : base(Instrument.CONTREBASSE,
        Category.GetInstance(Category.FROTTEES))
    { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(30, 55, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(110, 55, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(40, 55, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(100, 55, 210);
    }
}
