using UnityEngine;
using System.Collections;
using System;

public class Clavecin : Instrument
{

    public Clavecin() : base(Instrument.CLAVECIN,
        Category.GetInstance(Category.CLAVIERS))
    {

    }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 42, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(100, 42, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 42, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 42, 210);
    }
}
