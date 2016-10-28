using UnityEngine;
using System.Collections;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AudioClip swing;
    [SerializeField]
    private AudioClip switchToClub;
    [SerializeField]
    private AudioClip switchToSword;
    [SerializeField]
    private AudioClip coin;
    [SerializeField]
    private AudioClip swordHit;
    [SerializeField]
    private AudioClip clubHit;
    [SerializeField]
    private AudioClip levelTransition;
    [SerializeField]
    private AudioClip oneUp;

    private AudioSource m_AudioSource;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySwing()
    {
        m_AudioSource.clip = swing;
        m_AudioSource.Play();
    }

    public void PlaySwitchToClub()
    {
        m_AudioSource.clip = switchToClub;
        m_AudioSource.Play();
    }

    public void PlaySwitchToSword()
    {
        m_AudioSource.clip = switchToSword;
        m_AudioSource.Play();
    }

    public void PlayCoinPickup()
    {
        m_AudioSource.clip = coin;
        m_AudioSource.Play();
    }

    public void PlaySwordHit()
    {
        m_AudioSource.clip = swordHit;
        m_AudioSource.Play();
    }

    public void ClubHit()
    {
        m_AudioSource.clip = clubHit;
        m_AudioSource.Play();
    }

    public void PlayTransition()
    {
        m_AudioSource.clip = levelTransition;
        m_AudioSource.Play();
    }

    public void PlayOneUp()
    {
        m_AudioSource.clip = oneUp;
        m_AudioSource.Play();
    }
}
