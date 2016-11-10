using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss2 : MonoBehaviour
{
    [SerializeField] private GameObject m_bullet;
    public GameObject[] triggers1;
    public GameObject[] triggers2;

    private int health = 3;
    private float walkSpeed = 6f;
    private bool fightStarted = false;
    private static GameObject playerGameObject = null;
    int lastDir = 0;
    int count = 40;
    bool jumping = false;
    float[] jumpWidth = { 179f, 224f };
    float[] jumpHeight = { -6.5f, 2.5f };
    bool wasSpooked = false;

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
                transform.Translate(new Vector2(walkSpeed *2, 0) * Time.deltaTime);
                if (bossPos.y < jumpHeight[(3 - health) - 1])
                {
                    transform.Translate(new Vector2(0, 1) * Time.deltaTime);
                }
            }
            else
            {
                jumping = false;
                wasSpooked = false;
            }
        }
        if (fightStarted && health > 0)
        {
            bool isSpooked = Mathf.Abs(Mathf.Abs(playerPos.x) - Mathf.Abs(bossPos.x)) < 3;

            if (isSpooked || wasSpooked)
            {
                if (!wasSpooked)
                    wasSpooked = true;
                Debug.Log(bossPos.x);
                count--;
            }
            count++;
            if (count > 60)
            {
                GameObject Temporary_Bullet_Handler;
                Vector3 pos = transform.position;
                pos.y -= .5f;
                Temporary_Bullet_Handler = Instantiate(m_bullet, pos, new Quaternion()) as GameObject;

                //Retrieve the Rigidbody component from the instantiated Bullet and control it.
                Rigidbody2D Temporary_RigidBody;
                Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

                //Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.R
                Temporary_RigidBody.velocity = new Vector2(-4f, 0);

                //Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
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
    }
    void FaceDirection(int dir)
    {
        lastDir = dir;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Boss Collided with " + other.tag);
        //Debug.Log(other.gameObject.tag);
    }

    void DamageTaken() //, lethal
    {
        Debug.Log("ouch.");
        health -= 1;
        if (health <= 0)
        {
            Destroy(gameObject);//die
            //set to dead sprite
        }
        else
        {
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
}
