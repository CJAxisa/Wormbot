using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private float wormyMotionTimer;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (Input.GetKey("d") && !Input.GetKey("a")) 
        {
            rigidbody.AddForce(new Vector2((Mathf.Sin(wormyMotionTimer++) + 1f) * 5f, 0));
        }
        else if (Input.GetKey("a"))
        {
            rigidbody.AddForce(new Vector2((Mathf.Sin(wormyMotionTimer++) + 1f) * -5f, 0));
        }

    }
}
