using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerSounds))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    const float talkCooldown = 1.0f;
    public float currenttalkCooldown = 0.0f;
    bool onLadder = false;
    bool onDialogue = false;
    public bool hasSword = false;
    Collider2D dialogueCollider;
    Text dPrompt;
    public int coins = 0;
    public int health = 3;
    public int maxHealth = 3;
    public int keys = 0;
    public float damageTakenCooldown;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float dashSpeed = 28;
    public float ladderSpeed = 6;
    private float dashCooldown = 0.0f;
    public float DASH_COOLDOWN_TIME = 1.0f;
    float lastDirection = 1.0f;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerSounds sounds;
    SpriteRenderer playerSprite;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        sounds = GetComponent<PlayerSounds>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        UpdateCooldowns();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            Talk();
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.x != 0 && input.x != lastDirection)
        {
            lastDirection = input.x;
            FlipSprite();
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }



        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        bool shouldDash = ( (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Period)) && dashCooldown <= 0.0f );
        // if true, velocity.x will be replaced 
        if (shouldDash)
            Dash(ref velocity.x);
        if (onLadder)
            velocity.y = input.y * ladderSpeed;
        else
            velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void FlipSprite()
    {
        playerSprite.flipX = !playerSprite.flipX;
    }

    // Overrides the current X velocity with our dash velocity in the last moved in direction
    void Dash( ref float velocityX )
    {
        velocityX = lastDirection * dashSpeed;
        dashCooldown = DASH_COOLDOWN_TIME;
    }

    void UpdateCooldowns()
    {
        dashCooldown -= Time.deltaTime;
    }

    public void Talk()
    {
        //if (currenttalkCooldown <= 0.0f)
        //{
            //currenttalkCooldown = talkCooldown;
            dialogueCollider.SendMessageUpwards("NextDialogue");
        //}
    }

    //--------------------------------------------------Trigger logic from here down----------------------------------------------------

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.tag);
        if (other.gameObject.tag == "Ladder")
        {
            onLadder = true;
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.gravityScale = 0;
            //m_Anim.SetBool("isClimbing", true);
        }
        else if (other.gameObject.tag == "Dialogue")
        {
            if (!onDialogue)
            {
                onDialogue = true;
                dialogueCollider = other;
                Talk();
            }
            //dPrompt.enabled = true;
            //gameObject.SendMessage("PromptSet", true);
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
            sounds.PlayCoinPickup();
        }
        else if (other.gameObject.tag == "Bandana")
        {
            Destroy(other.gameObject);
            if (health < maxHealth)
            {
                ++health;
                gameObject.SendMessage("AddBandana");
            }
        }
        else if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
            ++keys;
            if (keys == 1)
                gameObject.SendMessage("ToggleKey");
        }
        else if (other.gameObject.tag == "Door")
        {
            if (keys > 0)
            {
                --keys;
                other.gameObject.SendMessageUpwards("OpenDoor");
                if (keys == 0)
                    gameObject.SendMessage("ToggleKey");
            }
        }
        else if (other.gameObject.tag == "OneWayDoor")
        {
            other.gameObject.SendMessageUpwards("Close");
        }
        else if (other.gameObject.tag == "EnemyWeapon" && damageTakenCooldown <= 0.0f)
        {
            Debug.Log(health);
            health--;
            SendMessage("RemoveBandana");
            damageTakenCooldown = 0.5f;
        }
        else if (other.gameObject.tag == "Sword")
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
            //m_Anim.SetBool("isClimbing", false);
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.gravityScale = 1;
        }
        else if (other.gameObject.tag == "Dialogue")
        {
            //Debug.Log("exiting dialogue range");
            onDialogue = false;
            //dPrompt.enabled = false;
            dialogueCollider.SendMessageUpwards("CloseDialogue");
            dialogueCollider = null;
            //gameObject.SendMessage("PromptSet", false);
        }
    }

    public float GetLastDirection()
    {
        return lastDirection;
    }

}