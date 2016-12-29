using UnityEngine;
using System.Collections;

public class TamtamQuestion : BlindTestQuestion {


    public TamtamQuestion() :base(4)
    {
    }

    /// <summary>
    /// Propriété dérivée de la classe BlindTestQuestion qui génère la question appropriée en fonction 
    /// de la valeur de "affirmative" pour le TamtamIntru
    /// </summary>
    public new string Question
    {
        get
        {
            if (affirmative)
                return "Quel Tamtam joue le rythme ternaire ?";
            else
                return "Quel Tamtam ne joue pas le rythme ternaire ?";
        }
    } 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
