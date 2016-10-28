using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
    public int clubHealth = 2;
    public int swordHealth = 1;
    public GameObject playerObject;
    [SerializeField] Sprite dazed;
    [SerializeField] Sprite dead;
    [SerializeField] Object[] drops;

    // Use this for initialization
    void Start () {
        if (playerObject == null)
            playerObject = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

    }
    public bool isAlive()
    {
        bool isisIs4Me = true;
        return (swordHealth > 0 && clubHealth > 0 && isisIs4Me);
    }
    void DamageTaken(bool lethal)
    {
        //if living
        if (swordHealth > 0 && clubHealth > 0)
        {
            //apply damage
            if (lethal)
                swordHealth--;
            else
                clubHealth--;

            //check for drops
            if(clubHealth == 0 || swordHealth == 0)
            {
                foreach (Object thing in drops)
                {
                    GameObject drop = (GameObject)Instantiate(thing);
                    drop.transform.position = gameObject.transform.position;
                }
                gameObject.layer = 0;

            }

            //change status
            if (clubHealth == 0)
            {
                GetComponent<SpriteRenderer>().sprite = dazed;
            }
            else if (swordHealth == 0)
            {
                GetComponent<SpriteRenderer>().sprite = dead;
                playerObject.SendMessage("AddKill");
            }
        }
    }
}
