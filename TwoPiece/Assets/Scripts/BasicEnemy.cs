using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BasicEnemy : MonoBehaviour {
    public int clubHealth = 2;
    public int swordHealth = 1;
    public float walkSpeed = 1.0f;

    private Controller2D controller;
    private BoxCollider2D collider;
    SpriteRenderer sprite;
    private EnemyState state = EnemyState.Patrolling;
    public Vector2 direction = Vector2.right;
    public LayerMask collisionMask;

    enum EnemyState {
        Patrolling
    }

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
	}

    void Awake()
    {

    }

    void Update()
    {
        if(EnemyState.Patrolling == state)
        {
            CheckForCollisions();
            controller.Move(direction * walkSpeed * Time.deltaTime);
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

    bool CheckFrontCollisions()
    {
        const float distanceToTurnAroundAt = 0.5f;
        Vector2 rayOrigin = (direction == Vector2.right) ? new Vector2(collider.bounds.max.x, collider.bounds.center.y) : new Vector2(collider.bounds.min.x, collider.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, distanceToTurnAroundAt, collisionMask);
        return hit;
    }

    bool CheckGroundCollisions()
    {
        const float distanceInFrontOfEnemy = 0.1f;
        Vector2 rayOrigin = (direction == Vector2.right) ? new Vector2(collider.bounds.max.x, collider.bounds.min.y) : new Vector2(collider.bounds.min.x, collider.bounds.min.y);
        rayOrigin += direction * distanceInFrontOfEnemy;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, collisionMask);
        return hit;
    }

    void FlipSprite()
    {
        sprite.flipX = !sprite.flipX;
    }

    void OnSwordDamage()
    {
        swordHealth--;
    }

    void OnClubDamage()
    {
        clubHealth--;
    }

}
