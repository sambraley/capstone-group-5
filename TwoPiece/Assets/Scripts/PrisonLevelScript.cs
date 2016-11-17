using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrisonLevelScript : MonoBehaviour {
    public GameObject calypso;
    public GameObject warden;

    public Image DialogueBox;
    public Text spareWarden1;
    public Text spareWarden2;
    public Text killWarden1;
    public Text killWarden2;

    public Text takeShip;
    public Image blackScreen;

    AudioSource hitSound;
    public AudioClip hitSoundClip;
    bool firstText = true;
    bool killWarden = false;

    // Use this for initialization
    void Start()
    {
        hitSound = gameObject.AddComponent<AudioSource>();
        spareWarden1.enabled = false;
        spareWarden2.enabled = false;
        killWarden1.enabled = false;
        killWarden2.enabled = false;
        takeShip.enabled = false;
    }

    void Triggered(int kills)
    {
        if (kills >= 2)
        {
            killWarden = true;
        }
        calypso.SendMessageUpwards("Freeze");
        DialogueBox.enabled = true;
        if (killWarden)
        {
            killWarden1.enabled = true;
        }
        else
        {
            spareWarden1.enabled = true;
        }
    }

    void Next()
    {
        if (firstText && killWarden)
        {
            killWarden1.enabled = false;
            killWarden2.enabled = true;
            firstText = false;
            return;
        }
        else if(firstText)
        {
            spareWarden1.enabled = false;
            spareWarden2.enabled = true;
            firstText = false;
            return;
        }

        DialogueBox.enabled = false;
        blackScreen.enabled = true;

        if (killWarden)
        {
            killWarden2.enabled = false;
            hitSound.clip = hitSoundClip;
            hitSound.Play();
        }
        else
        {
            spareWarden2.enabled = false;
        }
        takeShip.enabled = true;
        calypso.SendMessageUpwards("Freeze");
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(3);
        Final();
    }

    void Final()
    {
        calypso.SendMessageUpwards("SceneSwap");
    }
}
