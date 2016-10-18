using UnityEngine;
using System.Collections;

public class TriggerDamage : MonoBehaviour
{

    [SerializeField]
    private GameObject[] triggers;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            foreach (GameObject trigger in triggers)
            {
                trigger.gameObject.SendMessage("Damage", 1);
            }
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            box.enabled = false;
        }
    }
}
