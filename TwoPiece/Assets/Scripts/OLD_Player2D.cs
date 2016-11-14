using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class Player2D : MonoBehaviour
    {
        [SerializeField]
        private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField]
        private float m_JumpForce = 300;                  // Amount of force added when the player jumps.
        [SerializeField]
        private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField]
        private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField]
        private GameObject m_bullet;                  // A mask determining what is ground to the character
        [SerializeField]
        private bool reloaded = true;                  // A mask determining what is ground to the character
        [SerializeField]
        private Text dPrompt;
        [SerializeField]
        private AudioClip swing;
        [SerializeField]
        private AudioClip switchToClub;
        [SerializeField]
        private AudioClip switchToSword;


        private AudioSource m_AudioSource;
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        public bool onLadder = false;
        public bool onDialogue = false;
        private Collider2D dialogueCollider;
        private BoxCollider2D meleeCollider;
        public int lastDir = 1;
        const float fireCooldown = 1.0f;
        public float currentFireCooldown = 0.0f;
        private float meleeCooldown = 0.0f;
        private float swapWeaponsCooldown = 0.0f;
        [SerializeField]
        private bool hasSword;

        private int MAX_NUM_DASH = 2;
        private float MAX_COOLDOWN_DASH = 5f;
        private float MAX_TIME_TILL_NEXT_DASH = 1f;
        private float MAX_TIME_LEFT_IN_DASH = .2f;
        private int numDash = 2;
        private float coolDownDash = 0f;
        private float tillNextDash = 0f;
        private float timeLeftInDash = 0f;
        private float lockedDashDirection = 0;
        private float damageTakenCooldown = 0f;

        [SerializeField] private int maxHealth;
        [SerializeField] private int health;
        public int coins = 0;
        public int keys = 0;
        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            //m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            meleeCollider = transform.Find("MeleeCollider").gameObject.GetComponentInChildren<BoxCollider2D>();
            lastDir = 0;
            dPrompt.enabled = false;
            m_AudioSource = GetComponent<AudioSource>();
        }

        private float timeDown(float time, float delta)
        {
            if (time > 0f)
            {
                time -= delta;
            }
            if (time < 0f)
                time = 0f;
            return time;
        }

        private void FixedUpdate()
        {
            m_Grounded = false;
            // start timeTillNextDash once finish dash
            timeLeftInDash = timeDown(timeLeftInDash, Time.fixedDeltaTime);
            if (timeLeftInDash == 0f)
            {
                tillNextDash = timeDown(tillNextDash, Time.fixedDeltaTime);
            }
            coolDownDash = timeDown(coolDownDash, Time.fixedDeltaTime);
            if (coolDownDash == 0f && numDash < MAX_NUM_DASH)
            {
                Debug.Log("NEW DASH AVAILABLE");
                numDash++;
            }
            if(numDash < MAX_NUM_DASH && coolDownDash == 0f)
            {
                coolDownDash = MAX_COOLDOWN_DASH;
            }

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                {
                    m_Grounded = true;
                    //m_Anim.SetBool("isJumping", false);
                }
            }

            meleeCooldown -= Time.fixedDeltaTime;
            swapWeaponsCooldown -= Time.fixedDeltaTime;
            damageTakenCooldown -= Time.fixedDeltaTime;
            //m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            //m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

        public void Climb(float move)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 6.0f * move);
        }

        public void Move(float move, bool jump, bool dash)
        {
            if(move == 0)
                m_Anim.SetBool("isRunning", false);
            else
                m_Anim.SetBool("isRunning", true);


            if (dash && move != 0)
            {
                //check if really can dash
                if (numDash > 0 && tillNextDash == 0f && timeLeftInDash == 0f)
                {
                    //if at max dashes start cooldown refresh timer
                    if(numDash == MAX_NUM_DASH)
                    {
                        coolDownDash = MAX_COOLDOWN_DASH;
                    }
                    numDash--;
                    timeLeftInDash = MAX_TIME_LEFT_IN_DASH;
                    tillNextDash = MAX_TIME_TILL_NEXT_DASH;
                    lockedDashDirection = move;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                //dash speed up if in dash
                move = (timeLeftInDash > 0f ? lockedDashDirection * 3 : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                //m_Anim.SetBool("isRunning", true);

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // Use translate if grounded and add force if midair to avoid bug where the player gets stuck upon landing
                // Not sure why this works (bug is in Unity physics) , but using SetVelocity will cause the bug to reappear
                if( m_Grounded )
                {
                    m_Rigidbody2D.transform.Translate( new Vector2(move * lastDir * m_MaxSpeed * Time.fixedDeltaTime, 0) );
                }
                else
                {
                    m_Rigidbody2D.AddForce( new Vector2(m_MaxSpeed * move, 0) );
                }   

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump /*&& m_Anim.GetBool("Ground")*/)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                //m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.velocity = (new Vector2(m_Rigidbody2D.velocity.x, 0));
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                //m_Anim.SetBool("isJumping", true);
            }

        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.tag);
            if (other.gameObject.tag == "Ladder")
            {
                onLadder = true;
                Rigidbody2D rigid = GetComponent<Rigidbody2D>();
                rigid.gravityScale = 0;
                m_Anim.SetBool("isClimbing", true);
            }
            else if (other.gameObject.tag == "Dialogue")
            {
                onDialogue = true;
                dialogueCollider = other;
                dPrompt.enabled = true;
                gameObject.SendMessage("PromptSet", true);
            }
            else if (other.gameObject.tag == "MeleeCone")
            {
                Debug.Log("Yarr you've been damaged");
            }
            else if (other.gameObject.tag == "Coin")
            {
                Destroy(other.gameObject);
                ++coins;
                gameObject.SendMessage("SetCoin", coins);
            }
            else if (other.gameObject.tag == "Bandana")
            {
                Destroy(other.gameObject);
                if (health < maxHealth) {
                    ++health;
                    gameObject.SendMessage("AddBandana");
                }
            }
            else if(other.gameObject.tag == "Key")
            {
                Destroy(other.gameObject);
                ++keys;
                if (keys == 1)
                    gameObject.SendMessage("ToggleKey");
            }
            else if(other.gameObject.tag == "Door")
            {
                if(keys > 0)
                {
                    --keys;
                    other.gameObject.SendMessage("OpenDoor");
                    if (keys == 0)
                        gameObject.SendMessage("ToggleKey");
                }
            }
            else if (other.gameObject.tag == "EnemyWeapon" && damageTakenCooldown <= 0.0f)
            {
                health--;
                SendMessage("RemoveBandana");
                damageTakenCooldown = 0.5f;
            }
            else if(other.gameObject.tag == "Sword")
            {
                hasSword = true;
                Destroy(other.gameObject);
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "Ladder")
            {
                //Debug.Log("Not colliding with ladder");
                onLadder = false;
                m_Anim.SetBool("isClimbing", false);
                Rigidbody2D rigid = GetComponent<Rigidbody2D>();
                rigid.gravityScale = 1;
            }
            else if (other.gameObject.tag == "Dialogue")
            {
                //Debug.Log("exiting dialogue range");
                onDialogue = false;
                dPrompt.enabled = false;
                dialogueCollider.SendMessageUpwards("CloseDialogue");
                dialogueCollider = null;
                gameObject.SendMessage("PromptSet", false);
            }
        }

        public void Fire(bool fire)
        {
            if (fire && currentFireCooldown <= 0.0f && reloaded)
            {
                /*currentFireCooldown = fireCooldown;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + (playerWidth * lastDir), transform.position.y), new Vector2(lastDir, 0));
                //Vector3[] array = { new Vector3(transform.position.x + (playerWidth * lastDir), transform.position.y,0),new Vector3(transform.position.x + (playerWidth * lastDir) + 20, transform.position.y, 0) };
                //gameObject.GetComponent<LineRenderer>().SetPositions(array);
                if (hit.collider == null)
                {
                    Debug.Log("Do nothing");
                    //do nothing
                }
                else if (hit.collider.gameObject.tag == "Enemy")
                {
                    hit.collider.SendMessageUpwards("OnDamage", -1);
                }
                else if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.tag);
                }*/

                //The Bullet instantiation happens here.
                GameObject Temporary_Bullet_Handler;
                Temporary_Bullet_Handler = Instantiate(m_bullet, transform.position, new Quaternion()) as GameObject;

                //Retrieve the Rigidbody component from the instantiated Bullet and control it.
                Rigidbody2D Temporary_RigidBody;
                Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody2D>();

                //Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.R
                Temporary_RigidBody.velocity = new Vector2((m_MaxSpeed + 3f) * lastDir, 0f);

                //Basic Clean Up, set the Bullets to self destruct after 10 Seconds, I am being VERY generous here, normally 3 seconds is plenty.
                Destroy(Temporary_Bullet_Handler, 3.0f);
                reloaded = false;
            }
        }

        public void Talk(bool talk)
        {
            if (talk && currentFireCooldown <= 0.0f)
            {
                currentFireCooldown = fireCooldown;
                dialogueCollider.SendMessageUpwards("NextDialogue");
            }
        }

        public void Melee(bool melee)
        {
            const float ANIMATION_TIME = 0.5f;
            if(melee && meleeCooldown <= 0.0f)
            {
                PlaySwing();
                meleeCollider.enabled = true;
                meleeCooldown = ANIMATION_TIME;
            }
            if(meleeCooldown < ANIMATION_TIME)
            {
                meleeCollider.enabled = false;
            }

            /*if (melee && currentFireCooldown <= 0.0f)
            {
                currentFireCooldown = fireCooldown;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + (playerWidth * lastDir), transform.position.y), new Vector2(lastDir, 0), playerWidth * 2);
                //Vector3[] array = { new Vector3(transform.position.x + (playerWidth * lastDir), transform.position.y,0),new Vector3(transform.position.x + (playerWidth * lastDir) + 20, transform.position.y, 0) };
                //gameObject.GetComponent<LineRenderer>().SetPositions(array);
                if (hit.collider == null)
                {
                    Debug.Log("Do nothing");
                    //do nothing
                }
                else if (hit.collider.gameObject.tag == "Enemy")
                {
                    hit.collider.SendMessageUpwards("OnDamage", -1);
                }
                else if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.tag);
                }
            }*/
        }

        public void Reload(bool reload)
        {
            if(reload)
            {
                reloaded = true;
            }
        }

        public void SwapWeapons(bool swapWeapons)
        {
            const float SWAP_WEAPONS_COODLDOWN = 0.5f;
            if (swapWeapons && hasSword && swapWeaponsCooldown <= 0.0f && meleeCooldown <= 0.0f)
            {
                String previousTag = meleeCollider.tag;
                meleeCollider.tag = ( (previousTag == "Club") ? "Sword" : "Club" );
                swapWeaponsCooldown = SWAP_WEAPONS_COODLDOWN;
                gameObject.SendMessage("ToggleLethal");
                if (meleeCollider.tag == "Club")
                    PlaySwitchToClub();
                else
                    PlaySwitchToSword();
            }
        }
        //-----------------------------------
        private void PlaySwing()
        {
            m_AudioSource.clip = swing;
            m_AudioSource.Play();
        }

        private void PlaySwitchToClub()
        {
            m_AudioSource.clip = switchToClub;
            m_AudioSource.Play();
        }

        private void PlaySwitchToSword()
        {
            m_AudioSource.clip = switchToSword;
            m_AudioSource.Play();
        }
    }
}
