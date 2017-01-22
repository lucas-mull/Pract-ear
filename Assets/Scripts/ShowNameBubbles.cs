using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowNameBubbles : MonoBehaviour
{

    public Image[] UIBubble;
    bool _start = true;

    void Start()
    {
        
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
        foreach (Image bubble in UIBubble)
        {
            bubble.GetComponent<Animator>().SetTrigger("show");
        }
    }

    IEnumerator WaitForCountdown()
    {
        yield return new WaitForSeconds(5);
        TriggerBubbles();
    }

}