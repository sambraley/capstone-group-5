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


        private void Update()
        {

        }


        private void FixedUpdate()
        {
            // Read the inputs.
            int dir = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                dir = 1;
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                dir = -1;
            bool jump = Input.GetKey(KeyCode.Space);
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool dash = Input.GetKey(KeyCode.LeftShift);

            // Pass all parameters to the character control script.
            m_Character.Move(dir, crouch, jump, dash);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "ladder")
            {
                Debug.Log("Colliding with ladder");
                m_Anim.SetBool("isClimbing", true);
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "ladder")
            {
                Debug.Log("Not colliding with ladder");
                m_Anim.SetBool("isClimbing", false);
            }
        }
    }
}
