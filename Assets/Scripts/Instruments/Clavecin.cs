using UnityEngine;
using System.Collections;
using System;

public class Clavecin : Instrument
{

    public Clavecin() : base(Instrument.CLAVECIN,
        Category.GetInstance(Category.CLAVIERS))
    {
        this.Info = "Le clavecin est un instrument de musique à cordes muni d'un ou plusieurs claviers" +
            " dont chacune des cordes est pincée mécaniquement lorsque l’on appuie sur une touche. " + 
            "Il était particulièrement populaire aux XVII et XVIII siècles, le son qu’il émet nous fait" + 
            " d’ailleurs penser à cette époque.";
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
