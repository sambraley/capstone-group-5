using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DontDestoryCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void save()
    {
        DontDestroyOnLoad(gameObject);
    }
}
