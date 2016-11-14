using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevel : MonoBehaviour {

    public void LoadGame(int gameId)
    {
        SceneManager.LoadScene(gameId);
    }

}
