using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour {
    [SerializeField]
    Object[] drops;

    void OnDestroy()
    {
        foreach (Object thing in drops)
        {
            GameObject drop = (GameObject)Instantiate(thing); ;
            drop.transform.position = gameObject.transform.position;
        }
    }
}
