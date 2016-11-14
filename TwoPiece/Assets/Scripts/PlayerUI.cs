using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(Player))]
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Image[] bandanas;
        [SerializeField]
        private Image[] skulls; //0 is empty skull, 1 is half skull, 2 is full skull
        private int skullCount = -1; //-1 is no skull
        [SerializeField]
        private Image nonLethal;
        [SerializeField]
        private Image lethal;
        [SerializeField]
        private Image coin;
        [SerializeField]
        private Text coinCount;
        private Player m_Character;
        [SerializeField]
        private Text enemiesKilled;
        [SerializeField]
        private Text resolution;
        [SerializeField]
        private Image key;
        [SerializeField]
        private Image DEADImage;
        [SerializeField]
        private Text DeathText;
        [SerializeField]
        private Image DeathBackground;
        void Awake()
        {
            PlayerState p = PlayerState.Instance;
            m_Character = GetComponent<Player>();
            for (int  i = 0; i < m_Character.maxHealth; ++i)
            {
                bandanas[i].enabled = true;
            }
            for(int i = m_Character.maxHealth; i < 6;++i)
            {
                bandanas[i].enabled = false;
            }
            skulls[0].enabled = false;
            skulls[1].enabled = false;
            skulls[2].enabled = false;
            nonLethal.enabled = true; //always start in non-lethal mode
            lethal.enabled = false;
            coin.enabled = true;
            coinCount.text = p.coins.ToString() + "/15";
            key.enabled = false;
            DEADImage.enabled = false;
            DeathText.enabled = false;
            DeathBackground.enabled = false;

            if (p.enemiesKilled > 0)
            {
                skulls[Mathf.Min(p.enemiesKilled - 1, 2)].enabled = true;
            }
        }
        
        void PromptSet(bool set)
        {
            //promptBackground.enabled = set;
        }

        void AddKill()
        {
            m_Character.EnemiesKilled++;
            if (skullCount < 2 && skullCount >= -1)
            {
                if (skullCount >= 0)
                    skulls[skullCount].enabled = false;
                ++skullCount;
                skulls[skullCount].enabled = true;
                SetTextKiller();
            }
        }
        void CheckKills()
        {
            Debug.Log("you have killed: " + skullCount + 1);
            if(skullCount > -1)
            {
                SetTextKiller();
            }
        }
        void SetTextKiller()
        {
            enemiesKilled.text = "You slay the warden in cold blood for daring to imprison you.";
            resolution.text = "You search around the island a find a boat that'll take you to your old crew's hideout";
        }

        void RemoveBandana()
        {
            if (m_Character.health > 0 && m_Character.health <= bandanas.Length)
            {
                bandanas[m_Character.health - 1].enabled = false;
            }
        }

        void AddBandana()
        {
            if (m_Character.health < m_Character.maxHealth) //can't go above max
            {
                bandanas[m_Character.health].enabled = true;
            }
        }

        void MaxBandana()
        {
            for (int i = 0; i < m_Character.maxHealth; ++i)
            {
                bandanas[i].enabled = true;
            }
            for(int j = m_Character.maxHealth; j < bandanas.Length; j++)
            {
                bandanas[j].enabled = false;
            }
        }

        void SetCoin(int coins)
        {
            coinCount.text = coins.ToString() + "/15";
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

        void ToggleKey()
        {
            key.enabled = !key.enabled;
        }

        void DeathScreen() {
            skulls[0].enabled = false;
            skulls[1].enabled = false;
            skulls[2].enabled = false;
            nonLethal.enabled = false; //always start in non-lethal mode
            lethal.enabled = false;
            coin.enabled = false;
            key.enabled = false;
            DEADImage.enabled = true;
            DeathText.enabled = true;
            DeathBackground.enabled = true;
            
        }

        void DisableDeathScreen()
        {
            DEADImage.enabled = false;
            DeathText.enabled = false;
            DeathBackground.enabled = false;

        }
    }
}