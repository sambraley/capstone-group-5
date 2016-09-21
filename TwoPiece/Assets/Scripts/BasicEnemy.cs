using UnityEngine;
using System.Collections;

public class BasicEnemy : MonoBehaviour {
    public int health = 3;
    public float walkSpeed = 5.0f;
    public float chargeSpeed = 10.0f;
    public const float CHARGE_COOLDOWN = 5.0f;
    public float chargeCurrentCooldown = 0.0f;
    private bool isAggressive = false;
    private GameObject playerGameObject = null;

	// Use this for initialization
	void Start () {
        if (playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
        if (isAggressive)
        {
            FaceTowardsPlayer();
        }
        // Transform regardless to move torwards the player
        transform.Translate(new Vector3(walkSpeed, 0, 0) * Time.deltaTime);
    }

    void FaceTowardsPlayer()
    {

        bool isFacingLeft = (transform.localScale.x < 0);
        float differenceInPosition = transform.position.x - playerGameObject.transform.position.x;
        if (differenceInPosition > 0 && !isFacingLeft)
            Flip();
        else if (differenceInPosition < 0 && isFacingLeft)
            Flip();
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnAggroZoneExit()
    {
        // Flipping scale flips which way the enemy is moving along with their sprite
        Flip();
    }

    public void OnPlayerLeftAggroZone()
    {
        isAggressive = false;
    }

    public void OnPlayerEnterAggroZone()
    {
        isAggressive = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && isAggressive && chargeCurrentCooldown <= 0.0f)
        {
            Debug.Log("Greetixngs");
            walkSpeed = chargeSpeed;
            chargeCurrentCooldown = CHARGE_COOLDOWN;
        }
        //Debug.Log(other.gameObject.tag);
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
