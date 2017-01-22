using UnityEngine;
using System.Collections;
using System;

public class Guitare : Instrument
{
    public Guitare() : base(Instrument.GUITARE,
        Category.GetInstance(Category.PINCEES))
    {
        this.Info = "La guitare classique est un instrument de la famille des cordes pincées, " +
            "son son est amplifié grâce à sa caisse de résonnance. " + 
            "Elle possède six cordes en nylon que l’on peut pincer ou gratter.\n" +
            "C’est la guitare qui représente l’espagne, elle est plutôt utilisée en solo pour jouer de la musique classique" +
            ", cubaine, brésilienne, latine et espagnole comme la bossa nova et le flamenco.";
    }

    public override Vector3 getFarLeftVector()
    {
        return new Vector3(20, 40, 130);
    }

    public override Vector3 getFarRightVector()
    {
        return new Vector3(85, 40, 130);
    }

    public override Vector3 getMiddleLeftVector()
    {
        return new Vector3(30, 40, 190);
    }

    public override Vector3 getMiddleRightVector()
    {
        return new Vector3(75, 40, 190);
    }
}
