using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueBox : MonoBehaviour {
    [SerializeField] private Text[] dialogue;
    [SerializeField] private Image background;
    [SerializeField] private Canvas canvas;
    private int curDialogue;

    // Use this for initialization
	void Start () {
        foreach(Text stuff in dialogue)
        {
            stuff.enabled = false;
        }
        curDialogue = 0;
        if(background != null)
            background.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void CloseDialogue()
    {
        foreach (Text stuff in dialogue)
        {
            stuff.enabled = false;
        }
        curDialogue = 0;
        if(background != null)
            background.enabled = false;
    }

    void NextDialogue()
    {
        if (curDialogue == 0)
        {
            dialogue[curDialogue].enabled = true;
            if(background != null)
                background.enabled = true;
            ++curDialogue;
        }
        else
        {
            dialogue[curDialogue - 1].enabled = false;
            if (curDialogue == dialogue.Length)
            {
                CloseDialogue();
            }
            else
            {
                dialogue[curDialogue].enabled = true;
                ++curDialogue;
            }
        }
    }
}
