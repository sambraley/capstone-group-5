using System;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(Player2D))]

    public class Player2DController : MonoBehaviour
    {
        private Animator m_Anim;            // Reference to the player's animator component.
        private Player2D m_Character;
        int lastDir;
        float playerWidth;
        const float fireCooldown = 1.0f;
        float currentFireCooldown = 0.0f;
        private bool onLadder = false;
        private bool onDialogue = false;
        private Collider2D dialogueCollider;

        private void Awake()
        {
            m_Character = GetComponent<Player2D>();
            m_Anim = GetComponent<Animator>();
            lastDir = 0;
            BoxCollider2D hitbox = GetComponent<BoxCollider2D>();
            //Rigidbody2D rigid = GetComponent<>
            playerWidth = transform.localScale.x*(hitbox.size.x / 2);
        }


        private void Update()
        {

        }


        private void FixedUpdate()
        {
            currentFireCooldown -= Time.fixedDeltaTime;
            // Read the inputs.
            int dir = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                dir = 1;
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                dir = -1;
            bool jump = Input.GetKey(KeyCode.Space);
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool dash = Input.GetKey(KeyCode.LeftShift);
            bool fire = Input.GetKey(KeyCode.E);
            bool melee = Input.GetKey(KeyCode.F);

            if(Input.GetKey(KeyCode.W) && onLadder)
            {
                m_Anim.SetBool("isClimbing", true);
            }

            if (dir != 0)
            {
                lastDir = dir;
            }
            // Pass all parameters to the character control script.
            m_Character.Move(dir, crouch, jump, dash);
            if (!onDialogue)
            {
                Fire(fire);
            }
            else
            {
                Talk(fire);
            }
            Melee(melee);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "ladder")
            {
                onLadder = true;
                //Debug.Log("Colliding with ladder");
                //m_Anim.SetBool("isClimbing", true);
            }
            else if (other.gameObject.tag == "dialogue")
            {
                onDialogue = true;
                dialogueCollider = other;
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "ladder")
            {
                //Debug.Log("Not colliding with ladder");
                onLadder = false;
                m_Anim.SetBool("isClimbing", false);
                m_Anim.SetBool("isRunning", true);
            }
            else if (other.gameObject.tag == "dialogue")
            {
                Debug.Log("exiting dialogue range");
                onDialogue = false;
                dialogueCollider.SendMessageUpwards("CloseDialogue");
                dialogueCollider = null;
            }
        }

        void Fire(bool fire)
        {
            if (fire && currentFireCooldown <= 0.0f)
            {
                currentFireCooldown = fireCooldown;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + (playerWidth*lastDir), transform.position.y), new Vector2(lastDir,0));
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
            }
        }

        void Talk(bool talk)
        {
            if (talk && currentFireCooldown <= 0.0f)
            {
                currentFireCooldown = fireCooldown;
                dialogueCollider.SendMessageUpwards("NextDialogue");
            }
        }

        void Melee(bool melee)
        {
            if (melee && currentFireCooldown <= 0.0f)
            {
                currentFireCooldown = fireCooldown;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + (playerWidth * lastDir), transform.position.y), new Vector2(lastDir, 0), playerWidth*2);
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
            }
        }

    }
}
