using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowNameBubbles : MonoBehaviour
{

    public Image[] UIBubble;

    public void TriggerBubbles()
    {
        foreach (Image bubble in UIBubble)
        {
            bubble.GetComponent<Animator>().Play("bubbleShow");
        }
    }

}