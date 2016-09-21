using UnityEngine;
using System.Collections;

public class BasicEnemy : MonoBehaviour {
    public int health = 3;
    float walkSpeed = 10.0f;
    int dir = 1;

	// Use this for initialization
	void Start () {
	   
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
      transform.Translate(new Vector3(walkSpeed * dir, 0, 0) * Time.deltaTime);
    }

    void OnAggroZoneExit()
    {
        // Flipping scale flips which way the enemy is moving along with their sprite
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void OnPlayerLeftAggroZone()
    {

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
