using UnityEngine;
using System.Collections;
using System;

public class Harpe : Instrument
{
    public Harpe() : base(Instrument.HARPE,
        Category.GetInstance(Category.PINCEES))
    {
        this.Info = "La harpe est un grand instrument de musique à cordes pincées de forme le plus souvent triangulaire"
            + ", muni de cordes tendues de longueurs variables dont les plus courtes" +
            " donnent les notes les plus aiguës. C'est un instrument asymétrique, " +
            "contrairement à la lyre dont les cordes sont tendues entre deux montants parallèles.";
    }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(10, 42, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(95, 42, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 42, 210);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 42, 210);
    }
}
