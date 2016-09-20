using UnityEngine;
using System.Collections;

public class BasicEnemy : MonoBehaviour {
    public int health = 3;

	// Use this for initialization
	void Start () {
	   
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerBullet")
        {
            OnDamage(3);
            Destroy(other.gameObject);

        }
    }

    void OnDamage(int damage) //, lethal
    {
        Debug.Log("Yarrr I've been hit.");
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);//die
            //set to dead sprite
        }
        else
        {
            //do damage animation
        }
    }
}
