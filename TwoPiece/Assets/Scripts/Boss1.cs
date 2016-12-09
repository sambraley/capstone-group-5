using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private GameObject m_bullet;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    private Animator m_anim;
    public Object deadWarden;
    public AudioClip dead;
    public GameObject[] triggers1;
    public GameObject[] triggers2;

    private int health = 3;
    private float walkSpeed = 6f;
    private bool fightStarted = false;
    private static GameObject playerGameObject = null;
    int lastDir = 0;
    int count = 40;
    bool jumping = false;
    float[] jumpHeight = { 82.5f, 101.5f };
    bool wasSpooked = false;

    AudioSource sound;


    // Use this for initialization
    void Start()
    {
        m_anim = GetComponent<Animator>();
        if (playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
        sound = GetComponent<AudioSource>();
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
            if (bossPos.y < jumpHeight[(3 - health) - 1])
            {
                m_anim.SetBool("isWalking", true);
                transform.Translate(new Vector2(0, walkSpeed) * Time.deltaTime);
            }
            else
            {
                m_anim.SetBool("isWalking", false);
                jumping = false;
                wasSpooked = false;
            }
        }
        if (fightStarted && health > 0)
        {
            bool isSpooked = Mathf.Abs(Mathf.Abs(playerPos.y) - Mathf.Abs(bossPos.y)) < .6;
            bool closeEnough = Mathf.Abs(Mathf.Abs(playerPos.x) - Mathf.Abs(bossPos.x)) < .2;
            int direction = (bossPos.x > playerPos.x ? -1 : 1);
            bool isWalking = false;

            if (isSpooked || wasSpooked)
            {
                if(!wasSpooked)
                    wasSpooked = true;
                if (bossPos.x < rightBound)
                {
                    FaceDirection(-1);
                    isWalking = true;
                    transform.Translate(new Vector2(walkSpeed * lastDir * 3, 0) * Time.deltaTime);
                    //transform.Translate(new Vector2(walkSpeed * -direction * lastDir, 0) * Time.deltaTime);
                    //FaceDirection(direction);
                }
                count--;
            }
            else
            {
                if (!closeEnough)
                {
                    if (bossPos.x > leftBound)
                    {
                        isWalking = true;
                        transform.Translate(new Vector2(walkSpeed * direction * lastDir, 0) * Time.deltaTime);
                        FaceDirection(direction * -1);
                    }
                    else
                    {
                        if(bossPos.x < playerPos.x)
                        {
                            isWalking = true;
                            transform.Translate(new Vector2(walkSpeed, 0) * Time.deltaTime);
                            FaceDirection(1);
                        }
                    }
                }
            }
            m_anim.SetBool("isWalking", isWalking);
            count++;
            if (count > 75)
            {
                GameObject Temporary_Bullet_Handler;
                Vector3 pos = transform.position;
                pos.y -= .5f;
                Temporary_Bullet_Handler = Instantiate(m_bullet, pos, new Quaternion()) as GameObject;

                //Retrieve the Rigidbody component from the instantiated Bullet and control it.
                Rigidbody2D Temporary_RigidBody;
                Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

                //make the object fall constant rate
                Temporary_RigidBody.velocity = new Vector2(0f, -4f);

                //Basic Clean Up.
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

    void DamageTaken() //, lethal
    {
        health -= 1;
        if (health <= 0)
        {
            StartCoroutine(WaitOnSound());
            Destroy(gameObject);//die
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
        if (health ==2)
        {
            list = triggers1;
        }
        else if(health == 1)
        {
            list = triggers2;
        }
        foreach(GameObject g in list)
        {
            g.gameObject.SetActive(true);
        }
    }

    IEnumerator WaitOnSound()
    {
        sound.clip = dead;
        sound.Play();
        yield return new WaitWhile(() => sound.isPlaying);
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

    void OnDestroy()
    {
        if (health <= 0)
        {
            GameObject dead = (GameObject)Instantiate(deadWarden);
            dead.transform.position = gameObject.transform.position;
        }
    }
}
