using UnityEngine;
using System.Collections;
using System;

public class Tamtam : Instrument
{
    public Tamtam() : base(Instrument.TAMTAM,
        Category.GetInstance(Category.INDETERMINE)) { }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(25, 33, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(90, 33, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(35, 33, 210);        
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 33, 210);
    }
}
