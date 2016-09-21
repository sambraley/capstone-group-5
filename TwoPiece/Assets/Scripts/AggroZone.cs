using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AggroZone : MonoBehaviour {

    public List<BasicEnemy> list = new List<BasicEnemy>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerExit2D(Collider2D c)
    {
        if(c.tag == "Enemy")
        {
            c.SendMessageUpwards("OnAggroZoneExit");
        }
        if(c.tag == "Player")
        {
           foreach(BasicEnemy b in list)
            {
                b.OnPlayerLeftAggroZone();
            }
        }
    }
}
