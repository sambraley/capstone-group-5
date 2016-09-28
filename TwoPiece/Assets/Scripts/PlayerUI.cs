using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(Player2D))]
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Image[] bandanas;
        private int bandanaCount = 0;
        [SerializeField]
        private Image[] skulls; //0 is no skull, 1 is half skull, 2 is full skull
        private int skullCount = 0;
        [SerializeField]
        private Image nonLethal;
        [SerializeField]
        private Image lethal;
        [SerializeField]
        private Image coin;
        [SerializeField]
        private Text coinCount;
        private Player2D m_Character;
        [SerializeField]
        private Text enemiesKilled;
        void Awake()
        {
            m_Character = GetComponent<Player2D>();
            foreach (Image bandana in bandanas)
            {
                bandana.enabled = true;
            }
            bandanaCount = bandanas.Length - 1; //index of the current bandana value
            skulls[0].enabled = true;
            skulls[1].enabled = false;
            skulls[2].enabled = false;
            nonLethal.enabled = true; //always start in non-lethal mode
            lethal.enabled = false;
            coin.enabled = true;
            coinCount.text = m_Character.coins.ToString();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void AddKill()
        {
            if (skullCount < 2 && skullCount >= 0)
            {
                skulls[skullCount].enabled = false;
                ++skullCount;
                skulls[skullCount].enabled = true;
                enemiesKilled.text = "You wretch! You have the blood of your enemies on your hands.";   
            }
        }

        void RemoveBandana()
        {
            if (bandanaCount >= 0 && bandanaCount < bandanas.Length)
            {
                bandanas[bandanaCount].enabled = false;
                --bandanaCount;
            }
        }

        void AddBandana()
        {
            if (bandanaCount >= 0 && bandanaCount < (bandanas.Length - 1)) //can't go above max
            {
                ++bandanaCount;
                bandanas[bandanaCount].enabled = true;
            }
        }
        void SetCoin(int coins)
        {
            coinCount.text = coins.ToString();
        }

        void SetLethal(bool isLethal)
        {
            if (isLethal)
            {
                lethal.enabled = true;
                nonLethal.enabled = false;
            }
            else
            {
                lethal.enabled = false;
                nonLethal.enabled = true;
            }
        }

        void ToggleLethal()
        {
            lethal.enabled = !lethal.enabled;
            nonLethal.enabled = !nonLethal.enabled;
        }
    }
}