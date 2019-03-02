using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private float wormyMotionTimer;
    private Vector3 jumpVel;
    private int jumpsLeft;
    private float distanceToGround;

    [Range(0f,15f)]
    public float jumpVelMultiplier;
    [Range(0f, 5f)]
    public float moveSpeedMultiplier;
    public bool facingLeft;
    public bool grounded;
    public int numJumps;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        facingLeft = false;
        jumpsLeft = numJumps;
        distanceToGround = GetComponent<Collider2D>().bounds.extents.y;
    }
    
    void Update()
    {
        if (Input.GetKey("d") && !Input.GetKey("a")) 
        {
            rigidbody.velocity = new Vector2(1f*moveSpeedMultiplier, 0f);
            if (facingLeft)
                facingLeft = false;
        }
        else if (Input.GetKey("a") && !Input.GetKey("d"))
        {
            //rigidbody.AddForce(new Vector2((Mathf.Sin(wormyMotionTimer++) + 1f) * -5f, 0));
            rigidbody.velocity = new Vector2(-1f*moveSpeedMultiplier, 0f);
            if (!facingLeft)
                facingLeft = true;
        }
        jumpCheck();
        groundCheck();
    }


    private void jumpCheck()
    {
        if (Input.GetMouseButton(0) && jumpsLeft != 0)
        {
            Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.cyan);
        }

        if (Input.GetMouseButtonUp(0) && jumpsLeft != 0)
        {
            jumpVel = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            jumpVel = Vector3.ClampMagnitude(jumpVel, 1f);
            jumpVel *= jumpVelMultiplier;
            rigidbody.velocity = jumpVel;
            Debug.Log("Worm Launch");
            jumpsLeft--;
            grounded = false;
        }
    }

    private void groundCheck()
    {
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0f, distanceToGround * -1.5f),Color.cyan);


        Ray groundRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundRay,distanceToGround*1.5f))
        {
            grounded = true;
            jumpsLeft = numJumps;
            Debug.Log("i hit the grount");
        }
    }

}
