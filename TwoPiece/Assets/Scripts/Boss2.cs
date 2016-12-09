using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss2 : MonoBehaviour
{
    [SerializeField] private GameObject m_bullet;
    public GameObject[] triggers1;
    public GameObject[] triggers2;
    [SerializeField] Object[] drops;

    private int health = 3;
    private float walkSpeed = 6f;
    private bool fightStarted = false;
    private static GameObject playerGameObject = null;
    int count = 40;
    bool jumping = false;
    float[] jumpWidth = { 179f, 224f };
    float[] jumpHeight = { -6.5f, 3.5f };
    bool wasSpooked = false;
    int facingDir = 1; //1 for left -1 for right so boss moves away correctly

    public List<GameObject> bulletList;


    // Use this for initialization
    void Start()
    {
        if (playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    public void WakeUp()
    {
        fightStarted = true;
        PlayerState p = PlayerState.Instance;
        if (p.totalEnemiesKilled > 0)
        Debug.Log("youve been a bad player killing " + p.totalEnemiesKilled + " enemies");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
        Vector2 playerPos = playerGameObject.transform.position;
        Vector2 bossPos = gameObject.transform.position;
        if (jumping)
        {
            wasSpooked = false;
            if (bossPos.x < jumpWidth[(3 - health) - 1])
            {
                transform.Translate(new Vector2(walkSpeed *2 * facingDir, 0) * Time.deltaTime);
                if (bossPos.y < jumpHeight[(3 - health) - 1])
                {
                    transform.Translate(new Vector2(0, 3) * Time.deltaTime);
                }
            }
            else
            {
                jumping = false;
                wasSpooked = false;
                Flip();
            }
        }
        if (fightStarted && health > 0)
        {
            bool isSpooked = Mathf.Abs(Mathf.Abs(playerPos.x) - Mathf.Abs(bossPos.x)) < 6;

            if (isSpooked || wasSpooked)
            {
                if (!wasSpooked)
                    wasSpooked = true;
                count--;
            }
            count++;
            PlayerState p = PlayerState.Instance;
            //punish player for killing enemies caps it so its not impossible for mass murdering players
            if (count > (90 - Mathf.Min(p.totalEnemiesKilled*4.5f, 30f)))
            {
                GameObject Temporary_Bullet_Handler;
                Vector3 pos = transform.position;
                pos.y -= .5f;
                Temporary_Bullet_Handler = Instantiate(m_bullet, pos, new Quaternion()) as GameObject;

                //Retrieve the Rigidbody component from the instantiated Bullet and control it.
                Rigidbody2D Temporary_RigidBody;
                Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

                //Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.R
                if (health == 3)
                {
                    Temporary_RigidBody.velocity = new Vector2(Random.Range(-4, -7), Random.Range(3, 5));
                    Temporary_RigidBody.gravityScale = .05f;
                }
                else if(health == 2)
                {
                    Temporary_RigidBody.velocity = new Vector2(Random.Range(-4, -8), Random.Range(3, 6));
                    Temporary_RigidBody.gravityScale = .05f;
                }
                else if (health == 1)
                {
                    Temporary_RigidBody.velocity = new Vector2(Random.Range(-5, -9), Random.Range(-2, 6));
                    Temporary_RigidBody.gravityScale = .05f;
                }

                //Clean up the bullets
                Destroy(Temporary_Bullet_Handler, 7.0f);
                count = 0;
            }
        }
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
        facingDir *= -1;
    }
    void FaceDirection(int dir)
    {
        if (dir == -1)
            FaceLeft();
        else
            FaceRight();
    }
    void FaceRight()
    {
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x);
        transform.localScale = theScale;
    }
    void FaceLeft()
    {
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x) * -1;
        transform.localScale = theScale;
    }

    void DamageTaken() //, lethal
    {
        health -= 1;
        Flip();
        if (health <= 0)
        {
            foreach (Object thing in drops)
            {
                GameObject drop = (GameObject)Instantiate(thing);
                drop.transform.position = gameObject.transform.position;
            }
            //Destroy(gameObject);//die
            //set to dead sprite
        }
        else
        {
            StartCoroutine(FlashSprite(GetComponent<SpriteRenderer>(), 2));
            jumping = true;
            count = 0;
            //do damage animation
        }

        GameObject[] list = { };
        if (health == 2)
        {
            list = triggers1;
        }
        else if (health == 1)
        {
            list = triggers2;
        }
        foreach (GameObject g in list)
        {
            g.gameObject.SetActive(true);
        }
    }

    IEnumerator FlashSprite(SpriteRenderer s, int numTimes)
    {
        for (int i = 0; i < numTimes; i++)
        {
            s.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            s.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }
}