using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class Weapon : MonoBehaviour {

    Player player;
    BoxCollider2D playerCollider;
    RaycastOrigins raycastOrigins;
    public float weaponSize = 0.0f;
    public LayerMask enemyHitboxMask;
    private bool isLethal = false;


    private float weaponCooldown = 0.0f;
    public float WEAPON_COOLDOWN = 0.5f;

    private float swapWeaponsCooldown = 0.0f;
    public float SWAP_WEAPON_COOLDOWN = 0.5f;

    private bool hasSword = false;


	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();
        playerCollider = GetComponent<BoxCollider2D>();
        UpdateRaycastOrigins();
        weaponSize = playerCollider.size.x / 2;
	}
	
	// Update is called once per frame
	void Update () {
        weaponCooldown -= Time.deltaTime;
        swapWeaponsCooldown -= Time.deltaTime;
        // Swap Weapon
        if (hasSword && ( Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Comma) ) && swapWeaponsCooldown <= 0.0f)
        {
            SwapWeapon();
            swapWeaponsCooldown = SWAP_WEAPON_COOLDOWN;
        }
        // Swing Weapon
        if( (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Slash)) && weaponCooldown <= 0.0f)
        {
            SwingWeapon();
            weaponCooldown = WEAPON_COOLDOWN;
        }
	}

    void SwingWeapon()
    {
        UpdateRaycastOrigins();
        int numHorizontalTraces = 4;
        // Bounds.extents/2 as we're only hitting the top half of her hitbox
        float yOffsetPerTrace = (playerCollider.bounds.extents.y / 2) / (numHorizontalTraces - 1);
        for (int i = 0; i <= numHorizontalTraces; i++)
        { 
            Vector2 rayOrigin = (player.GetLastDirection() > 0.0f) ? raycastOrigins.right : raycastOrigins.left;
            rayOrigin += Vector2.up * yOffsetPerTrace * i;
            Vector2 rayDirection = (player.GetLastDirection() > 0.0f) ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, weaponSize, enemyHitboxMask);
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
            if (hit)
            {
                hit.collider.SendMessageUpwards("DamageTaken", isLethal);
                if(hit.collider.CompareTag("Enemy"))
                {
                    break;
                }
            }
        }
    }

    void SwapWeapon()
    {
        gameObject.SendMessage("ToggleLethal");
        isLethal = !isLethal;
    }

    void UpdateRaycastOrigins()
    {
        float centerY = playerCollider.bounds.center.y;
        raycastOrigins.left = new Vector2(playerCollider.bounds.min.x, centerY);
        raycastOrigins.right = new Vector2(playerCollider.bounds.max.x, centerY);
    }

    public void GiveSword()
    {
        hasSword = true;
    }

    struct RaycastOrigins
    {
        public Vector2 left, right;
    }

}
