using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerSounds))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float dashSpeed = 28;
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

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        UpdateCooldowns();

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
        bool shouldDash = ( Input.GetKeyDown(KeyCode.X) && dashCooldown <= 0.0f );
        // if true, velocity.x will be replaced 
        if (shouldDash)
            Dash(ref velocity.x);
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

}