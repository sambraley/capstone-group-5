using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinalLevelScript : MonoBehaviour {

    public GameObject calypso;
    public GameObject first_mate;

    public Sprite deadmate;
    public Sprite sleepmate;

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
    bool done = false;

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
        if (done) {
            final.enabled = true;
            
            blackScreen.enabled = true;
            StartCoroutine(wait());
        }
        else {
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
        if (killMate)
        {
            first_mate.SendMessageUpwards("DamageTaken");
            kill2.enabled = false;
            hitSound.clip = hitSoundClip;
            hitSound.Play();
            DialogueBox.enabled = false;
            first_mate.GetComponent<SpriteRenderer>().sprite = deadmate;
            first_mate.GetComponent<BoxCollider2D>().enabled = false;
            first_mate.transform.Translate(new Vector3(0, -.5f, 0));
            calypso.SendMessageUpwards("Freeze");
            done = true;
            Destroy(gameObject);
        }
        else
        {
            first_mate.SendMessageUpwards("DamageTaken");
            spare2.enabled = false;
            hitSound.clip = clubSoundClip;
            hitSound.Play();
            DialogueBox.enabled = false;
            first_mate.GetComponent<SpriteRenderer>().sprite = sleepmate;
            first_mate.GetComponent<BoxCollider2D>().enabled = false;
            first_mate.transform.Translate(new Vector3(0, -.5f, 0));
            calypso.SendMessageUpwards("Freeze");
            done = true;
            Destroy(gameObject);
        }

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
