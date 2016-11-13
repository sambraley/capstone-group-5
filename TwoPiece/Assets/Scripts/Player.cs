using UnityEngine;
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
    public int health = 3;
    public int maxHealth = 3;
    public int keys = 0;
    public int EnemiesKilled = 0;
    public float damageTakenCooldown;
    public DontDestoryCanvas toSave;
    private Animator m_Anim;

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

    public string checkpoint = "Prison";
    private bool hitCheckpoint = false;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        sounds = GetComponent<PlayerSounds>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
        m_Anim = GetComponent<Animator>();
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

        // If input.x and lastDirection have the same signedness, the result will be positive (- * - or + * +), however if they're different we'll enter
        if (input.x != 0 && ((input.x * lastDirection) < 0))
        {
            lastDirection = input.x > 0 ? 1.0f : -1.0f;
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
            PlayerState p = PlayerState.Instance;
            p.incrementCoins();
            int coins = PlayerState.Instance.getCoins();
            Debug.Log(coins);
            if (coins >= 15 && maxHealth <=5)
            {
                p.setCoins(0);
                p.incrementMaxHealth();
                p.giveMaxHealth();
                gameObject.SendMessage("MaxBandana");
                sounds.PlayOneUp();
            }
            else
            {
                sounds.PlayCoinPickup();
            }
            gameObject.SendMessage("SetCoin", PlayerState.Instance.getCoins());
        }
        else if (other.gameObject.tag == "Bandana")
        {
            Destroy(other.gameObject);
            PlayerState p = PlayerState.Instance;
            if (p.getHealth() < p.getMaxHealth())
            {
                gameObject.SendMessage("AddBandana");
                p.incrementHealth();
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
            if(checkpoint == "Prison")
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
        else if(other.gameObject.tag == "SceneSwap")
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Destroy(gameObject);
            if(toSave != null && toSave.gameObject != null)
                Destroy(toSave.gameObject);
            switch (currentSceneName)
            {
                case "Ship":
                    SceneManager.LoadScene("Prison");
                    break;
                case "Prison":
                case "PrisonBoss":
                    SceneManager.LoadScene("Beach");
                    break;
                case "Beach":
                    SceneManager.LoadScene("Cave");
                    break;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            onLadder = false;
            m_Anim.SetBool("isClimbing", false);
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.gravityScale = 1;
        }
        else if (other.gameObject.tag == "Dialogue")
        {
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
        PlayerState p = PlayerState.Instance;
        if (p.getHealth() == 1)
            respawn();
        else
        {
            SendMessage("RemoveBandana");
            damageTakenCooldown = 0.5f;
            StartCoroutine(FlashSprite(GetComponent<SpriteRenderer>(), 2));
            p.decrementHealth();
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
        //PlayerState p = PlayerState.Instance;
        //p.giveMaxHealth();
        if (hitCheckpoint)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SendMessage("MaxBandana");
            toSave.save();
            gameObject.transform.position = new Vector2(5, 57.5f);
        }
        SceneManager.LoadScene(checkpoint);
    }

    public void setCheckpoint(string name)
    {
        checkpoint = name;
        hitCheckpoint = true;
        gameObject.SendMessage("Persist");
    }

}