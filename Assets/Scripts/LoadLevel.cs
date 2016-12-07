using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevel : MonoBehaviour {


    public GameObject MenuCanvas;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadLevelOnClick(int gameId)
    {
        MenuCanvas.SetActive(false);
        //SceneManager.LoadScene(gameId);
    }
<<<<<<< HEAD
=======

>>>>>>> 9f6f8856f8b5cd8a27935a5c1c2f4d5475bbb3d2
    public void LoadGame(int gameId)
    {
        SceneManager.LoadScene(gameId);
    }

<<<<<<< HEAD
=======

>>>>>>> 9f6f8856f8b5cd8a27935a5c1c2f4d5475bbb3d2
}
