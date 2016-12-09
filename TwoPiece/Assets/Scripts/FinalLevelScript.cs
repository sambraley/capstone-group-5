using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinalLevelScript : MonoBehaviour {

    public GameObject calypso;
    public GameObject first_mate;

    public Image DialogueBox;
    public Text spare1;
    public Text spare2;
    public Text kill1;
    public Text kill2;

    public Text final;
    public Image blackScreen;

    AudioSource hitSound;
    public AudioClip hitSoundClip;
    public AudioClip clubSoundClip;
    bool firstText = true;
    bool killMate = false;

    // Use this for initialization
    void Start()
    {
        hitSound = gameObject.AddComponent<AudioSource>();
        spare1.enabled = false;
        spare2.enabled = false;
        kill1.enabled = false;
        kill2.enabled = false;
        final.enabled = false;
    }

    void Triggered(int kills)
    {
        if (kills >= 2)
        {
            killMate = true;
        }
        calypso.SendMessageUpwards("Freeze");
        DialogueBox.enabled = true;
        if (killMate)
        {
            kill1.enabled = true;
        }
        else
        {
            spare1.enabled = true;
        }
    }

    void Next()
    {
        if (firstText && killMate)
        {
            kill1.enabled = false;
            kill2.enabled = true;
            firstText = false;
            return;
        }
        else if (firstText)
        {
            spare1.enabled = false;
            spare2.enabled = true;
            firstText = false;
            return;
        }

        DialogueBox.enabled = false;
        blackScreen.enabled = true;

        if (killMate)
        {
            kill2.enabled = false;
            hitSound.clip = hitSoundClip;
            hitSound.Play();
            //CHANGE SPRITE FOR FIRST MATE
        }
        else
        {
            spare2.enabled = false;
            hitSound.clip = clubSoundClip;
            hitSound.Play();
            //CHANGE SPRITE FOR FIRST MATE
        }
        //takeShip.enabled = true;
        calypso.SendMessageUpwards("Freeze");
        //StartCoroutine(wait());
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
