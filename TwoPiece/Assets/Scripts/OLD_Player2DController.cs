using System;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(Player2D))]

    public class Player2DController : MonoBehaviour
    {
        private Player2D m_Character;

        private void Awake()
        {
            m_Character = GetComponent<Player2D>();
        }

        private void FixedUpdate()
        {
            m_Character.currentFireCooldown -= Time.fixedDeltaTime;
            // Read the inputs.
            int dir = 0;
            if ( Input.GetKey(KeyCode.RightArrow) )
                dir = 1;
            else if ( Input.GetKey(KeyCode.LeftArrow) )
                dir = -1;
            bool jump = Input.GetKey(KeyCode.Space);
            bool dash = Input.GetKey(KeyCode.A);
            bool melee = Input.GetKey(KeyCode.S);
            bool swapWeapons = Input.GetKey(KeyCode.F);
            bool fire = Input.GetKey(KeyCode.D);

            if (Input.GetKey(KeyCode.UpArrow)  && m_Character.onLadder)
            {
                //m_Anim.SetBool("isClimbing", true);
                m_Character.Climb(1);
            }
            else if(Input.GetKey(KeyCode.DownArrow) && m_Character.onLadder)
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
            m_Character.Move(dir, jump, dash);
            if (!m_Character.onDialogue )
            {
                //m_Character.Fire(fire);
            }
            else
            {
                m_Character.Talk(fire);
            }
            m_Character.SwapWeapons(swapWeapons);
            m_Character.Melee(melee);
            //m_Character.Reload(reload);
        }

        

    }
}
