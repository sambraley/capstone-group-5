using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tileBackground : MonoBehaviour {

    [SerializeField] private GameObject tile;
    [SerializeField] private int xOffset = 5;
    [SerializeField] private int yOffset = 2;
    [SerializeField] private int xLength = 15;
    [SerializeField] private int yLength = 30;
    
    [SerializeField] private List<GameObject> listSpecialTiles = new List<GameObject>();
    private List<GameObject> listTiles = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //The Bullet instantiation happens here.
        if (transform.position.x == 0 && transform.position.y == 0)
        {
            GameObject tileCreated = null;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    if (x % xOffset == 0 && y % yOffset == 0)
                    {
                        tileCreated = Instantiate(listSpecialTiles[((y*yOffset)%(2*yOffset))/yOffset], new Vector3(transform.position.x + 2 * x, transform.position.y + 2 * y, 1), new Quaternion()) as GameObject;
                    }
                    else
                    {
                        tileCreated = Instantiate(tile, new Vector3(transform.position.x + 2*x, transform.position.y + 2*y, 1), new Quaternion()) as GameObject;
                    }
                    listTiles.Add(tileCreated);
                }
            }
        }

    }
}
