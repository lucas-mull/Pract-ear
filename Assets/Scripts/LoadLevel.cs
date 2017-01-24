using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LoadLevel : MonoBehaviour {

    const string SIMON_NAME = "Instru' Simon";
    const string BLINDTEST_NAME = "Orchestra Blindtest";
    const string TAMTAM_NAME = "Tamtam Intrus";
    const string TEMPO_NAME = "Tempo Master";

    const string SIMON_DESCRIPTION = 
        "A l'instar du célèbre jeu du Simon, reproduisez une séquence de note en devinant correctement les instruments.\n Jusqu'où irez-vous ?";
    const string BLINDTEST_DESCRIPTION = "Ecoutez un extrait musical, et répondez à des questions portant sur les instruments !";
    const string TAMTAM_DESCRIPTION = "Retrouvez le tamtam qui n'a pas sa place parmi les autres.";
    const string TEMPO_DESCRIPTION = "Ecoutez un rythme et retrouvez le bon tempo en vous aidant du métronome.";

    const string SIMON_SKILL = "Reconnaître les sonorités des instruments seuls";
    const string BLINDTEST_SKILL = "Reconnaître les sonorités des instruments dans un morceau";
    const string TAMTAM_SKILL = "Reconnaître les rythmes";
    const string TEMPO_SKILL = "Reconnaître un tempo";

    public Canvas MenuCanvas;
    public Canvas ModalCanvas;
    public Canvas ExtrasCanvas;
    public Canvas LevelCanvas;
    public GameObject ModalPanel;
    public Text Name;
    public Text Description;
    public Text Skill;
    public Button[] _difficultyButtons;
    public Button _tutoButton;

    Sprite _unlockedSprite;
    Sprite _lockedSprite;
    int _selectedGame = 0;
    bool _onDifficultyScreen = false;
    GraphicRaycaster _raycaster;
    List<RaycastResult> _results = new List<RaycastResult>();
    PointerEventData _ped = new PointerEventData(null);

    // Use this for initialization
    void Start () {
        MenuCanvas.GetComponent<Animator>().Play("MenuFliesIn");
        _unlockedSprite = Resources.Load<Sprite>("Sprites/next_icon");
        _lockedSprite = Resources.Load<Sprite>("Sprites/lock_closed");
        _raycaster = LevelCanvas.GetComponent<GraphicRaycaster>();
        if (!DifficultyManager.isDifficultyUnlocked(DifficultyManager.EASY, DifficultyManager.BLINDTEST))
        {
            DifficultyManager.UnlockDifficulty(DifficultyManager.EASY, DifficultyManager.BLINDTEST);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (_onDifficultyScreen)
        {
            if (Utils.Clicked())
            {
                int size = _results.Count;
                _ped.position = Utils.GetClickedPosition();
                _raycaster.Raycast(_ped, _results);
                if (_results.Count > size)
                {
                    Debug.Log("clicked : " + _results[size].gameObject.name);
                    Button clicked = _results[size].gameObject.GetComponent<Button>();
                    if (clicked != null)
                    {
                        if (!clicked.IsInteractable())
                            return;

                        if (DifficultyManager.isDifficultyValid(clicked.tag))
                        {
                            DifficultyManager.PICKED_DIFFICULTY = clicked.tag;
                            LoadGame(_selectedGame);
                        }
                    }
                }
            }
        }
	}

    public void TriggerModalFor(string gameKey)
    {
        Animator animator = ModalPanel.GetComponent<Animator>();

        animator.SetBool("dismissed", false);

        switch (gameKey.ToLower())
        {
            case DifficultyManager.SIMON:
                Name.text = SIMON_NAME;
                Description.text = SIMON_DESCRIPTION;
                Skill.text = SIMON_SKILL;
                break;
            case DifficultyManager.BLINDTEST:
                Name.text = BLINDTEST_NAME;
                Description.text = BLINDTEST_DESCRIPTION;
                Skill.text = BLINDTEST_SKILL;
                break;
            case DifficultyManager.TAMTAM:
                Name.text = TAMTAM_NAME;
                Description.text = TAMTAM_DESCRIPTION;
                Skill.text = TAMTAM_SKILL;
                break;
            case DifficultyManager.TEMPO:
                Name.text = TEMPO_NAME;
                Description.text = TEMPO_DESCRIPTION;
                Skill.text = TEMPO_SKILL;
                break;
            default:
                break;
        }

        ModalCanvas.enabled = true;        
        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        else
        {
            animator.Play("ModalAppear");
        }
    }

    public void DismissModal()
    {
        ModalPanel.GetComponent<Animator>().SetBool("dismissed", true);
    }

    public void LoadDifficultyScreen(int gameId)
    {
        _selectedGame = gameId;
        
        foreach (Button button in _difficultyButtons)
        {
            string difficulty = button.tag;
            Image icon = button.transform.GetChild(1).GetComponent<Image>();
            if (DifficultyManager.isDifficultyUnlocked(difficulty, _selectedGame))
            {
                button.interactable = true;                
                icon.sprite = _unlockedSprite;
            }
            else
            {
                button.interactable = false;
                icon.sprite = _lockedSprite;
            }
        }

        if (gameId == DifficultyManager.ID_SIMON)
        {
            _tutoButton.interactable = true;
        }
        else
        {
            _tutoButton.interactable = false;
        }            

        ShowLevelPicking();
    }

    public void LoadGame(int gameId)
    {
        if (gameId == 6 && !_tutoButton.IsInteractable())
        {
            return;
        }

        ShowLoadingScreen();
        SceneManager.LoadSceneAsync(gameId);
    }

    public void Back()
    {
        _onDifficultyScreen = false;
        MenuCanvas.enabled = true;
        LevelCanvas.enabled = true;
        MenuCanvas.GetComponent<Animator>().Play("MenuFliesIn");
    }

    public void ShowExtras()
    {
        LevelCanvas.enabled = false;
        MenuCanvas.enabled = false;
        ExtrasCanvas.enabled = true;       
        Animator animator = ExtrasCanvas.GetComponent<Animator>();

        animator.Play("ExtrasFlyIn");
    }

    public void ShowLevelPicking()
    {
        MenuCanvas.enabled = false;
        Animator animator = LevelCanvas.GetComponent<Animator>();

        animator.Play("LevelFliesIn");
        _onDifficultyScreen = true;
    }

    void ShowLoadingScreen()
    {
        ExtrasCanvas.enabled = false;
        LevelCanvas.enabled = false;
    }

}
