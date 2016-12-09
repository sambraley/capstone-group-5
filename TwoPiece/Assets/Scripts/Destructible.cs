using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

    public int health = 1;
    float lastDamaged = 0.0f;
    const float INVULNERABLE_WINDOW = 0.25f;
    [SerializeField]
    Object[] drops;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        lastDamaged -= Time.deltaTime;
	}

    void DamageTaken()
    {
        if (lastDamaged <= 0.0f)
        {
            health--;
            if (health <= 0)
            {
                foreach (Object thing in drops)
                {
                    GameObject drop = (GameObject)Instantiate(thing);
                    drop.transform.position = gameObject.transform.position;
                }
                Destroy(this.gameObject);
            }
            lastDamaged = INVULNERABLE_WINDOW;
        }
    }

}
