using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TamTamLoad : MonoBehaviour {


    const string DIFFICULTY_EASY = "easy";
    const string DIFFICULTY_MEDIUM = "medium";
    const string DIFFICULTY_HARD = "hard";

    const int MAX_INSTRUMENTS = 4;
    const int QUESTION_READING_TIME = 3;

    #region Attributs assignés à travers l'Inspector

    public Button _play1stButton;           // Bouton 'Play' jouer / arreter l'extrait du premier tamtam
    public Button _play2ndButton;           // Bouton 'Play' jouer / arreter l'extrait du deuxieme tamtam
    public Button _play3rdButton;           // Bouton 'Play' jouer / arreter l'extrait du troisieme tamtam
    public Button _play4thButton;           // Bouton 'Play' jouer / arreter l'extrait du quatrieme tamtam
    public Canvas _igInterface;             // Canvas contenant l'interface du jeu
    public Sprite _playSprite;              // Sprite 'Play' du bouton play
    public Sprite _pauseSprite;             // Sprite  'Pause' du bouton play
    public Camera _mainCamera;              // Camera de la scene
    public Text _questionText;              // Texte qui afiche la question actuelle

    // Ensemble des spots en fonction de leur position sur la scene
    public Light farLeftLight;
    public Light farRightLight;
    public Light middleLeftLight;
    public Light middleRightLight;

    #endregion
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
