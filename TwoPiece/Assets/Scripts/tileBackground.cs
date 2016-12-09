using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tileBackground : MonoBehaviour {

    [SerializeField] private GameObject tile;
    [SerializeField] private float xStart = 5;
    [SerializeField] private float yStart = 5;
    [SerializeField] private int xOffset = 5;
    [SerializeField] private int yOffset = 2;
    [SerializeField] private int xLength = 15;
    [SerializeField] private int yLength = 30;
    
    [SerializeField] private List<GameObject> listSpecialTiles = new List<GameObject>();
    private List<GameObject> listTiles = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //The Bullet instantiation happens here.
        if (transform.position.x == xStart && transform.position.y == yStart)
        {
            GameObject tileCreated = null;
            for (int x = 0; x < xLength; x+=2)
            {
                for (int y = 0; y < yLength; y+=2)
                {
                    if (x == 0 && y == 0)
                        continue;
                    if (x % xOffset == 0 && y % yOffset == 0)
                    {
                        tileCreated = Instantiate(listSpecialTiles[((y*yOffset)%(2*yOffset))/yOffset], new Vector3(transform.position.x + 2 * x, transform.position.y + 2 * y, 10), new Quaternion()) as GameObject;
                    }
                    else
                    {
                        tileCreated = Instantiate(tile, new Vector3(transform.position.x + x, transform.position.y + y, 10), new Quaternion()) as GameObject;
                    }
                    listTiles.Add(tileCreated);
                }
            }
        }

    }
}
