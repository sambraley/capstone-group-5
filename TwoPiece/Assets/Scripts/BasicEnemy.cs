using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BasicEnemy : MonoBehaviour {
    public float walkSpeed = 1.0f;
    private float meleeRange = .8f;

    private float timeToSwing = 0.33f;
    public float weaponSwingCooldown = 1.0f;
    // currentWeaponCooldown will be set to timeToSwing + weaponSwingCooldown
    private float currentWeaponCooldown = 0.0f;


    private Controller2D controller;
    private BoxCollider2D collider;
    private SpriteRenderer sprite;
    private Animator m_Anim;

    private RaycastOrigins rayOrigins;
    private EnemyState state = EnemyState.Patrolling;
    public Vector2 direction = Vector2.right;

    public LayerMask collisionMask;
    public LayerMask playerMask;

    enum EnemyState {
        Patrolling,
        PreparingToSwing
    }

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        m_Anim = GetComponent<Animator>();
    }

    void Awake()
    {

    }

    void Update()
    {
        EnemyHealth enemyStatus = gameObject.GetComponent<EnemyHealth>();
        if (enemyStatus.isAlive())
        {
            UpdateCooldowns();
            if (EnemyState.Patrolling == state)
            {
                UpdateRaycastOrigins();
                if (CheckForPlayerInMeleeRange())
                {
                    if (currentWeaponCooldown <= 0.0f)
                    {
                        m_Anim.SetBool("isAttacking", true);
                        m_Anim.SetBool("isWalking", false);
                        state = EnemyState.PreparingToSwing;
                        StartCoroutine(SwingWeaponAfterTime(timeToSwing));
                    }
                    else
                    {
                        m_Anim.SetBool("isAttacking", false);
                    }
                }
                else
                {
                    if (currentWeaponCooldown <= 0.0f)
                    {
                        CheckForCollisions();
                        controller.Move(direction * walkSpeed * Time.deltaTime);
                        m_Anim.SetBool("isWalking", true);
                        m_Anim.SetBool("isAttacking", false);
                    }
                    else
                    {
                        m_Anim.SetBool("isAttacking", false);
                    }
                }
            }
            // else if(EnemyState.PreparingToSwing == state)

        }
        else
        {
            m_Anim.SetBool("isAttacking", false);
            m_Anim.SetBool("isWalking", false);
            m_Anim.enabled = false;
            enemyStatus.setToDeathSprite();
        }
    }

    void UpdateCooldowns()
    {
        currentWeaponCooldown -= Time.deltaTime;
    }

    bool CheckForPlayerInMeleeRange()
    {
        if (gameObject.GetComponent<EnemyHealth>().isAlive())
        {
            int numHorizontalTraces = 4;
            // Bounds.extents/2 as we're only hitting the top half of her hitbox
            float yOffsetPerTrace = (collider.bounds.extents.y / 2) / (numHorizontalTraces - 1);
            for (int i = 0; i <= numHorizontalTraces; i++)
            {
                Vector2 rayOrigin = (direction == Vector2.left) ? rayOrigins.centerLeft : rayOrigins.centerRight;
                rayOrigin += Vector2.up * yOffsetPerTrace * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, meleeRange, playerMask);
                Debug.DrawRay(rayOrigin, direction, Color.red);
                if (hit && hit.collider.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

    void SwingWeapon()
    {

        int numHorizontalTraces = 4;
        // Bounds.extents/2 as we're only hitting the top half of her hitbox
        float yOffsetPerTrace = (collider.bounds.extents.y / 2) / (numHorizontalTraces - 1);
        bool hitFound = false;
        for (int i = 0; i <= numHorizontalTraces && !hitFound; i++)
        {
            Vector2 rayOrigin = (direction == Vector2.left) ? rayOrigins.centerLeft : rayOrigins.centerRight;
            rayOrigin += Vector2.up * yOffsetPerTrace * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, meleeRange, playerMask);
            Debug.DrawRay(rayOrigin, direction, Color.red);
            if (hit && gameObject.GetComponent<EnemyHealth>().isAlive())
            {
                hit.collider.SendMessage("DamageTaken");
                hitFound = true;
            }
        }
    }

    void CheckForCollisions()
    {
        if( CheckFrontCollisions() || !CheckGroundCollisions() )
        {
            ChangeDirections();
        }
    }

    void ChangeDirections()
    {
        direction = direction * -1;
        FlipSprite();
    }

    // Shoot a ray from the middle of the enemy to distanceToTurnAroundAt in front of them, return true if you hit an obstacle
    bool CheckFrontCollisions()
    {
        const float distanceToTurnAroundAt = 0.5f;
        Vector2 rayOrigin = (direction == Vector2.left) ? rayOrigins.centerLeft : rayOrigins.centerRight;
        // Converted to bool implicitly
        return Physics2D.Raycast(rayOrigin, direction, distanceToTurnAroundAt, collisionMask);
    }

    bool CheckGroundCollisions()
    {
        const float distanceInFrontOfEnemy = 0.1f;
        Vector2 rayOrigin = (direction == Vector2.left) ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
        // OFfset the x to distanceInFrontOfEnemy in front of the enemy
        rayOrigin += direction * distanceInFrontOfEnemy;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, collisionMask);
        return hit;
    }

    void FlipSprite()
    {
        sprite.flipX = !sprite.flipX;
    }

    void UpdateRaycastOrigins()
    {
        const float skinWidth = 0.15f;
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        // Use bounds as we're checking for close collisions with these
        rayOrigins.centerLeft = new Vector2(bounds.min.x, bounds.center.y);
        rayOrigins.centerRight = new Vector2(bounds.max.x, bounds.center.y);
        // Use collider.bounds because we're checking in front of the enemy anyways
        rayOrigins.bottomLeft = new Vector2(collider.bounds.min.x, collider.bounds.min.y);
        rayOrigins.bottomRight = new Vector2(collider.bounds.max.x, collider.bounds.min.y);
    }

    struct RaycastOrigins
    {
        public Vector2 centerLeft, centerRight;
        public Vector2 bottomLeft, bottomRight;
    }

    IEnumerator SwingWeaponAfterTime(float time)
    {
        currentWeaponCooldown = time + weaponSwingCooldown;
        yield return new WaitForSeconds(time);

        UpdateRaycastOrigins();
        SwingWeapon();
        state = EnemyState.Patrolling;

        // Code to execute after the delay
    }

}
