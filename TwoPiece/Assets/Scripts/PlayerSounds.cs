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
    [SerializeField]
    private AudioClip hit;
    [SerializeField]
    private AudioClip walk;
    [SerializeField]
    private AudioClip dash;

    private AudioSource[] sounds;

    void Awake()
    {
        sounds = new AudioSource[11];
        for (int i = 0; i < 11;++i)
        {
            sounds[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySwing()
    {
        if (!sounds[0].isPlaying)
        {
            sounds[0].clip = swing;
            sounds[0].Play();
        }
    }

    public void PlaySwitchToClub()
    {
        if (!sounds[1].isPlaying)
        {
            sounds[1].clip = switchToClub;
            sounds[1].Play();
        }
    }

    public void PlaySwitchToSword()
    {
        if (!sounds[2].isPlaying)
        {
            sounds[2].clip = switchToSword;
            sounds[2].Play();
        }
    }

    public void PlayCoinPickup()
    {
        if (!sounds[3].isPlaying)
        {
            sounds[3].clip = coin;
            sounds[3].Play();
        }
    }

    public void PlaySwordHit()
    {
        if (!sounds[4].isPlaying)
        {
            sounds[4].clip = swordHit;
            sounds[4].Play();
        }
    }

    public void ClubHit()
    {
        if (!sounds[5].isPlaying)
        {
            sounds[5].clip = clubHit;
            sounds[5].Play();
        }
    }

    public void PlayTransition()
    {
        if (!sounds[6].isPlaying)
        {
            sounds[6].clip = levelTransition;
            sounds[6].Play();
        }
    }

    public void PlayOneUp()
    {
        if (!sounds[7].isPlaying)
        {
            sounds[7].clip = oneUp;
            sounds[7].Play();
        }
    }

    public void PlayHit()
    {
        if (!sounds[8].isPlaying)
        {
            sounds[8].clip = hit;
            sounds[8].Play();
        }
    }

    public void PlayWalk()
    {
        if (!sounds[9].isPlaying)
        {
            sounds[9].clip = walk;
            sounds[9].Play();
        }
    }

    public void StopPlayingWalk()
    {
        if (sounds[9].isPlaying)
        {
            sounds[9].Pause();
        }
    }

    public void PlayDash()
    {
        if (!sounds[10].isPlaying)
        {
            sounds[10].clip = dash;
            sounds[10].Play();
        }
    }
}
