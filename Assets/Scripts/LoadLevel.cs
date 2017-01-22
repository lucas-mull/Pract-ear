using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour {

    const string SIMON_KEY = "simon";
    const string BLINDTEST_KEY = "blindtest";
    const string TAMTAM_KEY = "tamtam";
    const string TEMPO_KEY = "tempo";

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
    public GameObject ModalPanel;
    public Text Name;
    public Text Description;
    public Text Skill;

    // Use this for initialization
    void Start () {
        MenuCanvas.GetComponent<Animator>().Play("MenuFliesIn");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TriggerModalFor(string gameKey)
    {
        Animator animator = ModalPanel.GetComponent<Animator>();

        animator.SetBool("dismissed", false);

        switch (gameKey.ToLower())
        {
            case SIMON_KEY:
                Name.text = SIMON_NAME;
                Description.text = SIMON_DESCRIPTION;
                Skill.text = SIMON_SKILL;
                break;
            case BLINDTEST_KEY:
                Name.text = BLINDTEST_NAME;
                Description.text = BLINDTEST_DESCRIPTION;
                Skill.text = BLINDTEST_SKILL;
                break;
            case TAMTAM_KEY:
                Name.text = TAMTAM_NAME;
                Description.text = TAMTAM_DESCRIPTION;
                Skill.text = TAMTAM_SKILL;
                break;
            case TEMPO_KEY:
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

    public void LoadLevelOnClick(int gameId)
    {
        MenuCanvas.enabled = false;
        ExtrasCanvas.enabled = false;    
        LoadGame(gameId);
    }

    public void LoadGame(int gameId)
    {
        SceneManager.LoadScene(gameId);
    }

    public void Back()
    {
        MenuCanvas.enabled = true;
        MenuCanvas.GetComponent<Animator>().Play("MenuFliesIn");
    }

    public void ShowExtras()
    {
        MenuCanvas.enabled = false;
        Animator animator = ExtrasCanvas.GetComponent<Animator>();

        if (!animator.enabled)
            animator.enabled = true;
        else
            animator.Play("ExtrasFlyIn");
    }

}
