using UnityEngine;
using System.Collections;
using System;

public class Trombone : Instrument
{
    public Trombone() : base(Instrument.TROMBONE,
        Category.GetInstance(Category.CUIVRES))
    {
        this.Info =
            "Le trombone est reconnaissable à sa coulisse, " +
            "qui lui permet de faire varier la hauteur des sons. " +
            "Il est un peu plus grave que la trompette, mais reste plus aigus" +
            " que ses cousins les gros cuivres commes les tubas et saxhorn.";
    }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(15, 45, 125);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(90, 45, 125);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 45, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(80, 45, 210);
    }
}
