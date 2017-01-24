using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowNameBubbles : MonoBehaviour
{

    public Image[] UIBubble;
    public Button _triggerButton;
    bool _start = true;
    bool _show = true;
    Sprite _showSprite;
    Sprite _hideSprite;

    void Start()
    {
        _showSprite = Resources.Load<Sprite>("Sprites/chat_bubble");
        _hideSprite = Resources.Load<Sprite>("Sprites/chat_bubble_hide");
    }

    void Update()
    {
        if (_start)
        {
            _start = false;
            StartCoroutine(WaitForCountdown());
        }
    }

    public void TriggerBubbles()
    {
        _show = !_show;
        if (_show)
        {
            ButtonHide();
        }
        else
        {
            ButtonShow();
        }

        foreach (Image bubble in UIBubble)
        {
            bubble.GetComponent<Animator>().SetBool("show", _show);
        }
    }

    IEnumerator WaitForCountdown()
    {
        yield return new WaitForSeconds(5);
        TriggerBubbles();
    }

    void ButtonShow()
    {
        _triggerButton.GetComponentInChildren<Text>().text = "Voir les noms";
        _triggerButton.transform.GetChild(1).GetComponent<Image>().sprite = _showSprite;
    }

    void ButtonHide()
    {
        _triggerButton.GetComponentInChildren<Text>().text = "Cacher les noms";
        _triggerButton.transform.GetChild(1).GetComponent<Image>().sprite = _hideSprite;
    }

}