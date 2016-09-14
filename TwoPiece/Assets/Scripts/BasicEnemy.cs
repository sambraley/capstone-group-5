using UnityEngine;
using System.Collections;

public class BasicEnemy : MonoBehaviour {
    int health;

	// Use this for initialization
	void Start () {
	   
	}

    void Awake()
    {
        health = 3;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDamage(int damage)
    {
        Debug.Log("Yarrr I've been hit.");
        --health;
        if (health == 0)
        {
            Destroy(gameObject);//die
        }
        else
        {
            //do death animation
        }
    }
}
