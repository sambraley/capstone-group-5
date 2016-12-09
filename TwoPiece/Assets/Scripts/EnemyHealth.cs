using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int clubHealth = 2;
    public int swordHealth = 1;
    public GameObject playerObject;
    [SerializeField]
    Sprite dazed;
    [SerializeField]
    Sprite dead;
    [SerializeField]
    Object[] drops;
    private AudioSource m_AudioSource;
    [SerializeField]
    private AudioClip hitByClub;
    [SerializeField]
    private AudioClip hitBySword;
    private Sprite deathSprite;

    // Use this for initialization
    void Start()
    {
        if (playerObject == null)
            playerObject = GameObject.FindWithTag("Player");
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool isAlive()
    {
        return (swordHealth > 0 && clubHealth > 0);
    }
    void DamageTaken(bool lethal)
    {
        //if living
        if (swordHealth > 0 && clubHealth > 0)
        {
            //apply damage
            if (lethal)
            {
                m_AudioSource.clip = hitBySword;
                swordHealth--;
            }
            else
            {
                m_AudioSource.clip = hitByClub;
                clubHealth--;
            }
            m_AudioSource.Play();
            //check for drops
            if (clubHealth == 0 || swordHealth == 0)
            {
                foreach (Object thing in drops)
                {
                    GameObject drop = (GameObject)Instantiate(thing);
                    drop.transform.position = gameObject.transform.position;
                }
                gameObject.layer = 0;

            }

            StartCoroutine(FlashSprite(GetComponent<SpriteRenderer>(), 2));

            //change status
            if (clubHealth == 0)
            {
                GetComponent<SpriteRenderer>().sprite = dazed;
                gameObject.transform.Translate(0, -.5f, 0);
                deathSprite = dazed;
            }
            else if (swordHealth == 0)
            {
                GetComponent<SpriteRenderer>().sprite = dead;
                gameObject.transform.Translate(0, -.5f, 0);
                playerObject.SendMessage("AddKill");
                deathSprite = dead;
            }
        }
    }

    IEnumerator FlashSprite(SpriteRenderer s, int numTimes)
    {
        for (int i = 0; i < numTimes; i++)
        {
            s.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            s.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void setToDeathSprite()
    {
        GetComponent<SpriteRenderer>().sprite = deathSprite;
    }
}
