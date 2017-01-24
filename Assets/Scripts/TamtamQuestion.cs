using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TamtamQuestion : BlindTestQuestion {

    const string PATH_TO_TAMTAM = "Soundtracks/Tamtam/Json/";

    public string type { get; set; }

    
    public TamtamQuestion() :base(4)
    {
        this.type = "Rythm";
        //int _questionNumber = UnityEngine.Random.Range(1, 3);
        //switch (_questionNumber)
        //{
        //    case 1:
        //        this.type = "Ternaire";             //Question pour savoir si le rythme est ternaire
        //        break;
        //    case 2:
        //        this.type = "Piano";                //Question pour savoir si la nuance est piano
        //        break;              
        //    default:                                //renvoyer une erreur si le numbre est negatif;
        //        break;                          
        //}
    }
    #region Attributs


    #endregion
    /// <summary>
    /// Propriété dérivée de la classe BlindTestQuestion qui génère la question appropriée en fonction 
    /// de la valeur de "affirmative" pour le TamtamIntru
    /// </summary>
    public new string Question
    {
        get
        {
            if (String.Compare(this.type, "Rythm") == 0)
            {
                if (affirmative)
                    return "Quel Tamtam joue le rythme ternaire ?";
                else
                    return "Quel Tamtam ne joue pas le rythme ternaire ?";
            }
            else
            {
                if (affirmative)
                    return "Quel Tamtam joue avec une nuance Piano ?";
                else
                    return "Quel Tamtam joue avec une nuance Forte ?";
            }

        }
    }

    public List<Extract> GetExtractsforQuestion()
    {
        // Initialisation d'une liste de string avec les noms, et d'une liste d'objets de type Extract pour les sons de chaque tamatam

        List<string> list = new List<string>();
        List<Extract> res = new List<Extract>();
        List<Extract> wrong = new List<Extract>();

        //if (String.Compare(this.type, "Ternaire") == 0)
        //{
        string pd = PATH_TO_TAMTAM + "Ternary/";
        res.Add(Extract.LoadExtraitFromJson("ternary_tom_toms", pd));
        wrong = Extract.LoadAll(PATH_TO_TAMTAM + "Binary/");
        res.AddRange(wrong);
        //}
        //else
        //{

        //}
        return res;
    }

    public new List<TamtamInstrument> GenerateInstrumentListForQuestion()
    {


        List<string> list = new List<string>();
        List<TamtamInstrument> res = new List<TamtamInstrument>();

        List<Extract> extractList = this.GetExtractsforQuestion();

        int currentcount = 0;
        list.Add("tamtam1");
        res.Add(new TamtamInstrument(true, extractList[0]));

        currentcount++;
        
        for (currentcount = 1; currentcount < 4; currentcount++)
        { 
            list.Add("tamtam" + (currentcount + 1));
            res.Add(new TamtamInstrument(false, extractList[currentcount]));
        }

        return res;

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
