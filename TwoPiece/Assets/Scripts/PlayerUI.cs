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
        //        [SerializeField]
        //        private Image promptBackground;
        void Awake()
        {
            m_Character = GetComponent<Player>();
            PlayerState p = PlayerState.Instance;
            for (int  i = 0; i < p.getMaxHealth(); ++i)
            {
                bandanas[i].enabled = true;
            }
            for(int i = p.getMaxHealth(); i < 6;++i)
            {
                bandanas[i].enabled = false;
            }
            skulls[0].enabled = false;
            skulls[1].enabled = false;
            skulls[2].enabled = false;
            nonLethal.enabled = true; //always start in non-lethal mode
            lethal.enabled = false;
            coin.enabled = true;
            coinCount.text = PlayerState.Instance.getCoins().ToString();
            key.enabled = false;
            //promptBackground.enabled = false;
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

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
            PlayerState p = PlayerState.Instance;
            if (p.getHealth() > 0 && p.getHealth() <= bandanas.Length)
            {
                bandanas[p.getHealth() - 1].enabled = false;
            }
        }

        void AddBandana()
        {
            PlayerState p = PlayerState.Instance;
            if ( p.getHealth() < p.getMaxHealth()) //can't go above max
            {
                bandanas[p.getHealth()].enabled = true;
            }
        }

        void MaxBandana()
        {
            PlayerState p = PlayerState.Instance;
            for (int i = 0; i < p.getMaxHealth(); ++i)
            {
                bandanas[i].enabled = true;
            }
            for(int j = p.getMaxHealth(); j < bandanas.Length; j++)
            {
                bandanas[j].enabled = false;
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

        void ToggleKey()
        {
            key.enabled = !key.enabled;
        }
        void Persist()
        {
            DontDestroyOnLoad(gameObject);
        }

        void DeathScreen() {
            
        }

    }
}