using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

    int health = 1;
    float lastDamaged = 0.0f;
    const float INVULNERABLE_WINDOW = 0.25f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
            Destroy(this.gameObject);
        lastDamaged -= Time.deltaTime;
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "PlayerBullet" || c.tag == "Sword" || c.tag == "Club" )
            health--;
    }

    void DamageTaken()
    {
        if (lastDamaged <= 0.0f)
        {
            health--;
            lastDamaged = INVULNERABLE_WINDOW;
        }
    }

}
