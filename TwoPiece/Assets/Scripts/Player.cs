﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerSounds))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    const float talkCooldown = 1.0f;
    public float currenttalkCooldown = 0.0f;
    bool onLadder = false;
    bool onDialogue = false;
    Collider2D dialogueCollider;
    Text dPrompt;
    public int coins = 0;
    public int health = 3;
    public int maxHealth = 3;
    public int keys = 0;
    public int EnemiesKilled = 0;
    public float damageTakenCooldown;
    public DontDestoryCanvas toSave;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float dashSpeed = 28;
    public float ladderSpeed = 6;

    private float dashDirection = 0.0f;
    public float dashTimer = 0.0f;
    public float dashDuration = 0.2f;
    private float dashCooldown = 0.0f;
    public float DASH_COOLDOWN_TIME = 1.0f;
    float lastDirection = 1.0f;

    bool wasGroundedLastUpdate = true;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerSounds sounds;
    SpriteRenderer playerSprite;

    private string checkpoint = "Prison";
    private bool hitCheckpoint = false;

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
        //if you land reset dash ;)
        if (controller.collisions.below && !wasGroundedLastUpdate)
            dashCooldown = 0.0f;
        wasGroundedLastUpdate = controller.collisions.below;

        if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && onDialogue) //Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)
        { //a talks if you're in dialogue
            Talk();
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //works with controller, takes joystick input

        if (input.x != 0 && input.x != lastDirection)
        {
            lastDirection = input.x;
            FlipSprite();
        }
        
        if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space)) && controller.collisions.below && !onDialogue) //Input.GetKeyDown(KeyCode.Space)
        { //a jumps
            velocity.y = jumpVelocity;
        }



        float targetVelocityX = input.x * moveSpeed;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.x = targetVelocityX;
        bool shouldDash = ( (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.LeftShift)) && dashCooldown <= 0.0f ); // || Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.LeftShift)
        //right bumper or b button dash
        // if true, velocity.x will be replaced
        if (shouldDash)
        {
            dashTimer = dashDuration;
            dashDirection = lastDirection;
        }
        if (dashTimer > 0.0f)
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
        velocityX = dashDirection * dashSpeed;
        dashCooldown = DASH_COOLDOWN_TIME;
    }

    void UpdateCooldowns()
    {
        dashCooldown -= Time.deltaTime;
        dashTimer -= Time.deltaTime;
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
            if ((coins == 10 && maxHealth == 3) || (coins == 20 && (maxHealth == 4 || maxHealth == 5)))
            {
                coins = 0;
                ++maxHealth;
                health = maxHealth;
                gameObject.SendMessage("MaxBandana");
                sounds.PlayOneUp();
            }
            else
            {
                sounds.PlayCoinPickup();
            }
            gameObject.SendMessage("SetCoin", coins);
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
            other.gameObject.layer = 12;
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
            setCheckpoint("PrisonBoss");
        }
        else if (other.gameObject.tag == "EnemyWeapon" && damageTakenCooldown <= 0.0f)
        {
            DamageTaken();
        }
        else if (other.gameObject.tag == "Sword")
        {
            GetComponent<Weapon>().GiveSword();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "PlayerBullet")
        {
            DamageTaken();
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

    private void DamageTaken()
    {
        health--;
        if (health == 0)
            respawn();
        else
        {
            SendMessage("RemoveBandana");
            damageTakenCooldown = 0.5f;
        }
    }

    private void respawn()
    {
        if (hitCheckpoint)
        {
            DontDestroyOnLoad(gameObject);

            health = maxHealth;
            for(int i = 0; i < health; i++)
                gameObject.SendMessage("AddBandana");
            toSave.save();
        }
        SceneManager.LoadScene(checkpoint);
        gameObject.transform.position = new Vector2(5,57.5f);
        Debug.Log("Num enemies Killed: " + EnemiesKilled);
    }

    public void setCheckpoint(string name)
    {
        checkpoint = name;
        hitCheckpoint = true;
        gameObject.SendMessage("Persist");
    }

}