using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Crate : MonoBehaviour {
    public float gravity = -10;
    Vector3 velocity;

    Controller2D controller;
    // Use this for initialization
    void Awake () {
        controller = GetComponent<Controller2D>();
    }

    // Update is called once per frame
    void Update () {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        
    }
}
