using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
    [SerializeField] private Sprite open;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OpenDoor()
    {
        GetComponent<BoxCollider2D>().enabled = false; //remove collider
        GetComponent<SpriteRenderer>().sprite = open; //switch sprtie
    }
}
