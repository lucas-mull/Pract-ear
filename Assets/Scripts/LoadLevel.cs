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
        SceneManager.LoadScene(gameId);
    }

    public void LoadGame(int gameId)
    {
        SceneManager.LoadScene(gameId);
    }

}
