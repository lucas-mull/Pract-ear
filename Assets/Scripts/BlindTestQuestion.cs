using System.Collections.Generic;
using UnityEngine;

public class BlindTestQuestion
{    
    public bool affirmative = true;

    public string[] inExtract;
    public int totalInstrumentCount;

    public string[] InstrumentsInExtract
    {
        get { return this.inExtract; }
        set { this.inExtract = value; }
    }

    public string Question
    {
        get
        {
            if (affirmative)
                return "Quels instruments jouent dans le morceau ?";
            else
                return "Quels instruments sont absents du morceau ?";
        }
    }

    public BlindTestQuestion(int totalInstrumentCount)
    {
        this.affirmative = System.Convert.ToBoolean(Random.Range(0, 2));
        this.totalInstrumentCount = totalInstrumentCount;
    }

    public List<BlindTestInstrument> GenerateInstrumentListForQuestion()
    {
        List<string> list = new List<string>();
        List<BlindTestInstrument> res = new List<BlindTestInstrument>();
        int currentCount = 0;
        for (int i = 0; i < inExtract.Length; i++)
        {
            list.Add(inExtract[i].ToLower());
            res.Add(new BlindTestInstrument(inExtract[i], true));
            currentCount++;
        }

        while (currentCount < this.totalInstrumentCount)
        {
            string randomInstrument = Instrument.GenerateRandomInstrumentName();
            while (list.Contains(randomInstrument))
            {
                randomInstrument = Instrument.GenerateRandomInstrumentName();
            }
            list.Add(randomInstrument);
            currentCount++;
        }

        for (int i = inExtract.Length; i < list.Count; i++)
        {
            res.Add(new BlindTestInstrument(list[i]));
        }

        return res;
    }
}
