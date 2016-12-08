using UnityEngine;
using System.Collections;
using System;

public abstract class Instrument {

    const string PATH_TO_PREFABS = "Prefabs/";

    // Noms des prefabs des instruments dans Assets/Resources/Prefabs
    public static string PIANO = "Piano";
    public static string TROMPETTE = "Trompette";
    public static string VIOLON = "Violon";
    public static string MARIMBA = "Marimba";

    public string Name
    {
        get { return this.Model.name; }
        set { this.Model.name = value; }
    }

    private GameObject model;

    public GameObject Model {
        get { return this.model; }
        set { this.model = value; }
    }

    public Instrument(string modelName)
    {
        this.setModelFromName(modelName);
        this.Name = modelName;
    }
    
    public bool setModelFromName(string modelName)
    {
        this.Model = Resources.Load(PATH_TO_PREFABS + modelName, typeof(GameObject)) as GameObject;
        return this.Model != null;
    }

    public Instrument()
    {

    }

    public abstract Vector3 getFarLeftVector();
    public abstract Vector3 getFarRightVector();
    public abstract Vector3 getMiddleLeftVector();
    public abstract Vector3 getMiddleRightVector();

    public void Destroy()
    {
        Resources.UnloadUnusedAssets();
    }
}
