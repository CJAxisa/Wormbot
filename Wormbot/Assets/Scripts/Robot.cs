﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    private new Rigidbody2D rigidbody;
    private float wormyMotionTimer;
    private Vector3 toWorm;

    public float moveSpeedMultiplier;
    [Range(0f, 3f)]
    public float maxWalkSpeed;
    public bool facingLeft;
    public int walkDist;
    public float attackDelay;
    private float attackTimer;
    private int distMoved;
    public int direct;
    public GameObject bullet;
    private GameObject worm;
    public float bulletSpeed;
    public float visionRange;
    public bool captured;

    LayerMask lm;
    public Vector2 walkVelocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        facingLeft = false;
        direct = 1;
        distMoved = 0;
        walkDist = 150;
        maxWalkSpeed = 0.5f;
        moveSpeedMultiplier = 1f;
        attackTimer = 0;
        captured = false;

        lm = LayerMask.GetMask("Ground");
        worm = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(distMoved < walkDist)
        {
            if(direct == 1)
            {
                walkVelocity = new Vector2(1f * moveSpeedMultiplier, 0f);
                if (facingLeft)
                    facingLeft = false;
                distMoved++;
            }
            else if (direct == -1)
            {
                walkVelocity = new Vector2(-1f * moveSpeedMultiplier, 0f);
                if (!facingLeft)
                    facingLeft = true;

                distMoved++;
            }
        }
        else if (distMoved >= walkDist)
        {
            distMoved = 0;
            direct = direct * -1;   
        }


        if (walkVelocity.x > maxWalkSpeed)
            walkVelocity = new Vector2(maxWalkSpeed, 0f);
        else if (walkVelocity.x < -1f * maxWalkSpeed)
            walkVelocity = new Vector2(-1f * maxWalkSpeed, 0f);

        if (Mathf.Abs(rigidbody.velocity.x) < maxWalkSpeed)
            rigidbody.velocity += walkVelocity;

        toWorm = worm.transform.position - this.transform.position;
        toWorm = toWorm.normalized;
        bool hitsWorm = false;
        if (!captured)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(this.transform.position, toWorm, visionRange);
            foreach (RaycastHit2D targetHit in hit)
            {
                if (targetHit.collider.tag == "Player")
                {
                    hitsWorm = true;
                }
            }
            if (hitsWorm)
            { // robot targeting, red is in range, green is out of range
                Debug.DrawLine(this.transform.position, (visionRange * toWorm) + this.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(this.transform.position, (visionRange * toWorm) + this.transform.position, Color.green);
            }
            // attack at set intervals
            if (attackTimer <= 0 && hitsWorm)
            {
                GameObject newBullet = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                Bullet bulletBullet = newBullet.GetComponent<Bullet>();
                bulletBullet.velocity = bulletSpeed * toWorm;
                //bulletBullet.worm = this.worm;
                attackTimer = attackDelay;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }
    }
}
