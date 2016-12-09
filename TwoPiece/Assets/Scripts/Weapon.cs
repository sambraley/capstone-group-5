using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class Weapon : MonoBehaviour {

    Player player;
    BoxCollider2D playerCollider;
    RaycastOrigins raycastOrigins;
    private float weaponSize = 0.9f;
    public LayerMask enemyHitboxMask;
    private bool isLethal = false;


    private float weaponCooldown = 0.0f;
    private float WEAPON_COOLDOWN = .66f;

    private float swapWeaponsCooldown = 0.0f;
    public float SWAP_WEAPON_COOLDOWN = 0.5f;

    private bool hasSword = false;


	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();
        playerCollider = GetComponent<BoxCollider2D>();
        UpdateRaycastOrigins();
	}
	
	// Update is called once per frame
	void Update () {
        weaponCooldown -= Time.deltaTime;
        if(weaponCooldown < 0)
            player.GetComponent<Animator>().SetBool("isAttacking", false);
        swapWeaponsCooldown -= Time.deltaTime;
        // Swap Weapon
        if (hasSword && (Input.GetKeyDown(KeyCode.JoystickButton3)|| Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Comma)) && swapWeaponsCooldown <= 0.0f) //( Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Comma) )
        { //Y button on controller
            SwapWeapon();
            swapWeaponsCooldown = SWAP_WEAPON_COOLDOWN;
            
        }
        // Swing Weapon
        if((Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Slash)) && weaponCooldown <= 0.0f) //(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Slash))
        { //X button on controller
            SwingWeapon();
            weaponCooldown = WEAPON_COOLDOWN;
        }
	}

    void SwingWeapon()
    {
        player.GetComponent<Animator>().SetBool("isAttacking", true);
        player.SendMessageUpwards("PlaySwing");
        UpdateRaycastOrigins();
        int numHorizontalTraces = 4;
        // Bounds.extents/2 as we're only hitting the top half of her hitbox
        float yOffsetPerTrace = (playerCollider.bounds.extents.y) / (numHorizontalTraces);
        for (int i = 0; i <= numHorizontalTraces; i++)
        { 
            Vector2 rayOrigin = (player.GetLastDirection() > 0.0f) ? raycastOrigins.right : raycastOrigins.left;
            rayOrigin += Vector2.down * yOffsetPerTrace * i;
            Vector2 rayDirection = (player.GetLastDirection() > 0.0f) ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, weaponSize, enemyHitboxMask);
            Debug.DrawRay(rayOrigin, rayDirection, Color.yellow, 1.0f);
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
        if (isLethal)
        {
            player.SendMessageUpwards("PlaySwitchToSword");
            player.switchWeapons(true);
        }
        else
        {
            player.SendMessageUpwards("PlaySwitchToClub");
            player.switchWeapons(false);
        }
        player.GetComponent<Animator>().SetBool("hasSword", isLethal);
    }

    void UpdateRaycastOrigins()
    {
        float centerY = playerCollider.bounds.center.y;
        float yStart = centerY + (playerCollider.bounds.extents.y / 2);
        raycastOrigins.left = new Vector2(playerCollider.bounds.min.x, yStart);
        raycastOrigins.right = new Vector2(playerCollider.bounds.max.x, yStart);
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
