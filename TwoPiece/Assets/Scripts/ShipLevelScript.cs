using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipLevelScript : MonoBehaviour {
    public GameObject calypso;
    public GameObject firstMate;
    public GameObject door;
    public Sprite closedDoor;
    public Image DialogueBox;
    public Text final1;
    public Text final2;
    public Image blackScreen;
    AudioSource doorSound;
    public AudioClip doorSoundClip;
    AudioSource hitSound;
    public AudioClip hitSoundClip;
    bool firstText = true;

	// Use this for initialization
	void Start () {
        doorSound = gameObject.AddComponent<AudioSource>();
        hitSound = gameObject.AddComponent<AudioSource>();
        final1.enabled = false;
        final2.enabled = false;
    }

    void Triggered()
    {
        calypso.SendMessageUpwards("Freeze");
        door.GetComponent<SpriteRenderer>().sprite = closedDoor;
        firstMate.transform.Rotate(new Vector3(180,0,180));
        DialogueBox.enabled = true;
        final1.enabled = true;
        doorSound.clip = doorSoundClip;
        doorSound.Play();
    }

    void Next()
    {
        if (firstText)
        {
            final1.enabled = false;
            final2.enabled = true;
            firstText = false;
        }
        else
        {
            calypso.SendMessageUpwards("Freeze");
            DialogueBox.enabled = false;
            final2.enabled = false;
            blackScreen.enabled = true;
            hitSound.clip = hitSoundClip;
            hitSound.Play();
            StartCoroutine(wait());
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(2);
        Final();
    }

    void Final()
    {
        calypso.SendMessageUpwards("SceneSwap");
    }
}
