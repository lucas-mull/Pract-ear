using UnityEngine;
using System.Collections;
using System;

public class Contrebasse : Instrument
{
    public Contrebasse() : base(Instrument.CONTREBASSE,
        Category.GetInstance(Category.FROTTEES))
    {
        this.Info = "La contrebasse fait partie de la famille des instruments à cordes." +
            "De tous les cordes, c’est l’instrument le plus grand et le plus grave." + 
            "Son corps est légèrement plus bombé que celui d’un violon ou violoncelle. " + 
            "Il est possible d’en jouer en frottant les cordes avec un archet (arco) ou bien" + 
            " en les pinçant avec les doigts (pizzicato).";
    }

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
