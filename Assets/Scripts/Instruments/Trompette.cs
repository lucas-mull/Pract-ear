using UnityEngine;
using System.Collections;
using System;

public class Trompette : Instrument
{
    public Trompette() : base(Instrument.TROMPETTE,
        Category.GetInstance(Category.CUIVRES))
    {
        this.Info = "La trompette est un des plus vieux cuivres. " +
            "Elle est reconnaissable à son son puissant et clair. " +
            "C’est le plus petit et plus aigu des cuivres " +
            "(mais existe en plusieurs variantes dont la piccolo est vraiment l’extrême).";
    }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 35, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 35, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(33, 35, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 35, 210);
    }
}
