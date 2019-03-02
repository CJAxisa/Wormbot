﻿using System.Collections;
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

        distanceToGround = GetComponent<PolygonCollider2D>().bounds.extents.y;
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

        groundCheck();
        jumpCheck();

    }


    /// <summary>
    /// Checks for jump input from player.
    /// </summary>
    private void jumpCheck()
    {
        if (Input.GetMouseButton(0) && jumpsLeft != 0)
        {
            Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.cyan);
        }

        if (Input.GetMouseButtonUp(0) && jumpsLeft != 0)
        {
            jumpVel = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            jumpVel.z = 0f;
            jumpVel.Normalize();

            //Debug.Log("Original vector: " + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) + "\nNormalized Vel: " +jumpVel+"\n");


            jumpVel *= jumpVelMultiplier;
            rigidbody.velocity = jumpVel;
            
            jumpsLeft--;
            grounded = false;

            
        }
    }

    /// <summary>
    /// Determines whether or not the player is grounded
    /// </summary>
    private void groundCheck()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(0f, -1f*distanceToGround,0f),Color.cyan);

        //Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit2D groundCheckResult = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround+0.1f);
        if (groundCheckResult && groundCheckResult.collider.gameObject.tag!="Player")
        {
            
            grounded = true;
            jumpsLeft = numJumps;
            //Debug.Log("i hit the grount");
        }
        else
        {
            //Debug.Log("did not hit ground");
            grounded = false;
        }
    }

}
