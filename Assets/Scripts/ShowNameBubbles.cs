using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowNameBubbles : MonoBehaviour {

    bool showBubbles = true;
    bool showing = false;

    public Image[] bubbles;

    public void TriggerBubbles()
    {
        showBubbles = true;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (showBubbles)
        {
            StartCoroutine(CoRoutineBubbles());
        }
    }

    IEnumerator CoRoutineBubbles()
    {
        for (int i = 0; i < bubbles.Length; i++)
        {
            if (bubbles[i].transform.localScale != new Vector3(1.5f, 0.5f, 1.5f))
            {
                bubbles[i].transform.localScale += new Vector3(0.3f / 6.0f, 0.1f / 6.0f, 0.3f / 6.0f);
            }
            else
            {
                showing = true;
            }
        }

        if (showing)
        {
            showBubbles = false;            
            yield return new WaitForSeconds(5);

            for (int i = 0; i < bubbles.Length; i++)
            {
                bubbles[i].transform.localScale = new Vector3(0, 0, 0);
            }

            showing = false;
        }
    }
}
