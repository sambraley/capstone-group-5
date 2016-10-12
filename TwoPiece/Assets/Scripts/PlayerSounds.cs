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
}
