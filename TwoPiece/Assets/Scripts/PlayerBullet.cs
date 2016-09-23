using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Bullet trigger");
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "CrouchCollider" && other.gameObject.tag != "PlayerBullet" && other.gameObject.tag != "ladder" && other.gameObject.tag != "AggroZone"
            && other.gameObject.tag != "MeleeCone" && other.gameObject.tag != "VisionCone")
        {
            Debug.Log("Bullet trigger " + other.tag);
            Debug.Log("Bullet collison info " + other.ToString());
            Destroy(gameObject);
            //other.SendMessageUpwards("OnDamage", 1);
        }
    }
}
