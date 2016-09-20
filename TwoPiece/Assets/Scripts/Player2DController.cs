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

        private void Awake()
        {
            m_Character = GetComponent<Player2D>();
            m_Anim = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            m_Character.currentFireCooldown -= Time.fixedDeltaTime;
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

            if(Input.GetKey(KeyCode.W)  && m_Character.onLadder)
            {
                //m_Anim.SetBool("isClimbing", true);
                m_Character.Climb(1);
            }
            else if(Input.GetKey(KeyCode.S) && m_Character.onLadder)
            {
                //m_Anim.SetBool("isClimbing", true);
                m_Character.Climb(-1);
            }
            else if(m_Character.onLadder)
            {
                m_Character.Climb(0);
            }

            if (dir != 0)
            {
                m_Character.lastDir = dir;
            }
            // Pass all parameters to the character script.
            m_Character.Move(dir, crouch, jump, dash);
            if (!m_Character.onDialogue)
            {
                m_Character.Fire(fire);
            }
            else
            {
                m_Character.Talk(fire);
            }
            m_Character.Melee(melee);
        }

        

    }
}
