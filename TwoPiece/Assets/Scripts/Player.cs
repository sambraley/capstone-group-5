using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public int health = 3;
    public int maxHealth = 3;
    public int keys = 0;
    public int coins = 0;
    public int EnemiesKilled = 0;
    public float damageTakenCooldown;
    private Animator m_Anim;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    public float moveSpeed = 6;
    public float dashSpeed = 28;
    public float ladderSpeed = 6;

    private float dashDirection = 0.0f;
    public float dashTimer = 0.0f;
    public float dashDuration = 0.2f;
    private float dashCooldown = 0.0f;
    public float DASH_COOLDOWN_TIME = 1.0f;
    float lastDirection = 1.0f;

    public GameObject prisonPlotObject;
    public GameObject cavePlotObject;

    bool wasGroundedLastUpdate = true;
    bool canJump = true;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerSounds sounds;
    SpriteRenderer playerSprite;
    [SerializeField]
    private Sprite swordIdle;
    [SerializeField]
    private Sprite clubIdle;
    public Sprite current = null;

    bool frozen = false;
    GameObject storyObject;

    void Start()
    {
        LoadState();
        controller = GetComponent<Controller2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        sounds = GetComponent<PlayerSounds>();
        current = clubIdle;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
        m_Anim = GetComponent<Animator>();
    }

    void Update()
    {
        //test code for trying to move skull on killing to where UI is to show player what happened
        if (!frozen) {
            UpdateCooldowns();
            //if you land reset dash and jump ;)
            if (controller.collisions.below && !wasGroundedLastUpdate)
            {
                dashCooldown = 0.0f;
                canJump = true;
            }
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

            // If input.x and lastDirection have the same signedness, the result will be positive (- * - or + * +), however if they're different we'll enter
            if (input.x != 0 && ((input.x * lastDirection) < 0))
            {
                lastDirection = input.x > 0 ? 1.0f : -1.0f;
                FlipSprite();
            }

            if ((Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space)) && canJump && !onDialogue) //Input.GetKeyDown(KeyCode.Space)
            { //a jumps
                velocity.y = jumpVelocity;
                canJump = false;
            }



            float targetVelocityX = input.x * moveSpeed;
            //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.x = targetVelocityX;
            bool shouldDash = ((Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.LeftShift)) && dashCooldown <= 0.0f); // || Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.LeftShift)
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
            {
                velocity.y = input.y * ladderSpeed;
                Debug.Log(input.y);
                if (input.y != 0 || m_Anim.GetBool("isAttacking"))
                { 
                    m_Anim.speed = 1;
                }
                else
                {
                    m_Anim.speed = .15f;
                }
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
                m_Anim.speed = 1;
            }
            controller.Move(velocity * Time.deltaTime);
            if (velocity.x != 0 && !m_Anim.GetBool("isClimbing") /* && is not jumping*/)
            {
                m_Anim.SetBool("isRunning", true);
                sounds.PlayWalk();
            }
            else
            {
                m_Anim.SetBool("isRunning", false);
                sounds.StopPlayingWalk();
            }

        }
        else
        {
            m_Anim.SetBool("isRunning", false);
            if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                storyObject.SendMessage("Next");
            }
        }
    }

    void FlipSprite()
    {
        playerSprite.flipX = !playerSprite.flipX;
    }

    // Overrides the current X velocity with our dash velocity in the last moved in direction
    void Dash( ref float velocityX )
    {
        sounds.PlayDash();
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
            m_Anim.SetBool("isClimbing", true);
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
        else if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            coins++;
            if (coins >= 15 && maxHealth <= 5)
            {
                coins = 0;
                maxHealth++;
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
                gameObject.SendMessage("AddBandana");
                health++;
            }
        }
        else if (other.gameObject.tag == "KillZone")
        {
            respawn();
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
        else if (other.gameObject.tag == "Checkpoint")
        {
            SaveState(other.gameObject.transform.position, true);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "PlayerBullet")
        {
            DamageTaken();
        }
        else if(other.gameObject.tag == "SceneSwap")
        {
            SceneSwap();
        }
        else if (other.gameObject.tag == "StoryObject")
        {
            storyObject = other.gameObject;
            other.gameObject.SendMessageUpwards("Triggered");
        }
        else if (other.gameObject.tag == "StoryObjectPrison")
        {
            storyObject = prisonPlotObject.gameObject;
            prisonPlotObject.gameObject.SendMessageUpwards("Triggered", EnemiesKilled);
        }
        else if (other.gameObject.tag == "StoryObjectCave")
        {
            PlayerState p = PlayerState.Instance;
            storyObject = cavePlotObject.gameObject;
            cavePlotObject.gameObject.SendMessageUpwards("Triggered", p.totalEnemiesKilled);
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
            //dPrompt.enabled = false;
            dialogueCollider.SendMessageUpwards("CloseDialogue");
            dialogueCollider = null;
            //gameObject.SendMessage("PromptSet", false);
        }
    }

    public void incKills()
    {
        EnemiesKilled++;
    }

    public float GetLastDirection()
    {
        return lastDirection;
    }

    private void DamageTaken()
    {
        if (health == 1)
            respawn();
        else
        {
            SendMessage("RemoveBandana");
            damageTakenCooldown = 0.5f;
            StartCoroutine(FlashSprite(GetComponent<SpriteRenderer>(), 2));
            health--;
            sounds.PlayHit();
        }
    }

    IEnumerator FlashSprite(SpriteRenderer s, int numTimes)
    {
        for(int i = 0; i < numTimes; i++)
        {
            s.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            s.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void respawn()
    {
        //dont allow player to move while dead
        frozen = true;
        gameObject.SendMessage("DeathScreen");
        StartCoroutine(wait()); //wait for a bit
    }

    IEnumerator wait()
    {
        PlayerState p = PlayerState.Instance;
        float deathScreenTime = (p.timesDied == 0 ? 2.0f : .5f);
        p.timesDied++;
        yield return new WaitForSecondsRealtime(deathScreenTime);
        RealRespawn();
    }

    private void RealRespawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        frozen = false;
    }

    private void SaveState(Vector2 pos = default(Vector2), bool hitCheckpoint = false)
    {
        PlayerState p = PlayerState.Instance;
        p.maxHealth = maxHealth;
        p.coins = coins;
        p.pos = pos;
        p.hitCheckpoint = hitCheckpoint;
        //if you ever hit checkpoint you get sword forever
        if(hitCheckpoint)
        {
            p.enemiesKilled = EnemiesKilled;
            p.hasSword = true;
        }
        else
        {
            p.enemiesKilled = 0;
        }
    }
    private void LoadState()
    {
        PlayerState p = PlayerState.Instance;
        maxHealth = p.maxHealth;
        health = maxHealth;
        gameObject.SendMessage("MaxBandana");
        coins = p.coins;
        if(p.hitCheckpoint)
        {
            gameObject.transform.position = p.pos;
            EnemiesKilled = p.enemiesKilled;
        }
        if(p.hasSword)
        {
            GetComponent<Weapon>().GiveSword();
        }
    }

    void Freeze()
    {
        frozen = !frozen;
    }
    public void switchWeapons(bool isSword)
    {
        if(isSword)
        {
            //playerSprite.sprite = swordIdle;
            //playerSprite.color = Color.yellow;
        }
        else
        {
            //playerSprite.sprite = clubIdle;
            //playerSprite.color = Color.green;
        }
    }

    void SceneSwap()
    {
        PlayerState p = PlayerState.Instance;
        p.totalEnemiesKilled += EnemiesKilled;
        SaveState();
        string currentSceneName = SceneManager.GetActiveScene().name;
        Destroy(gameObject);
        switch (currentSceneName)
        {
            case "Ship":
                SceneManager.LoadScene("Prison");
                break;
            case "Prison":
                SceneManager.LoadScene("Beach");
                break;
            case "Beach":
                SceneManager.LoadScene("Cave");
                break;
        }
    }
}