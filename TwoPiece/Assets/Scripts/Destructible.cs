using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

    int health = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
            Destroy(this.gameObject);
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.tag == "PlayerBullet")
            health--;
    }

}
