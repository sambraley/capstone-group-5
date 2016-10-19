using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
    public int clubHealth = 2;
    public int swordHealth = 1;
    private static GameObject playerObject;

	// Use this for initialization
	void Start () {
        if (playerObject != null)
            playerObject = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

        if (swordHealth < 1 || clubHealth < 1)
        {
            Destroy(gameObject);
        }
        if( swordHealth < 1 )
        {
            playerObject.SendMessage("AddKill");
        }
    }

    void DamageTaken(bool lethal)
    {
        if (lethal)
            swordHealth--;
        else
            clubHealth--;
    }
}
