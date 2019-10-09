using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private float wormyMotionTimer;
    private Vector3 jumpVel;
    private int jumpsLeft;
    private float distanceToGround;

    [Range(0f, 15f)]
    public float jumpVelMultiplier;
    [Range(0f, 5f)]
    public float moveSpeedMultiplier;
    [Range(0f, 0.5f)]
    public float tempo;
    //[Range(0f, 3f)]
    public float maxWalkSpeed;
    public bool facingLeft;
    public GameObject arrow;
    public bool grounded;
    public bool captured;
    public bool capturing;
    public bool ejecting;
    public int numJumps;
    public string ability;
    private const int CAPTURE_FRAMES = 60;
    private int captureFramesLeft=0;
    private GameObject transTarget;
    private bool dashing;

    LayerMask lm;
    private Vector2 walkVelocity;
    private Sprite startingSprite;
    private Vector2 startingScale;
    private Animator test;

    private Vector2 startingColliderSize;
    private Vector2 startingColliderOffset;

    private Vector3 wormScale;
    private Vector3 leftScale;
    private Vector3 rightScale;





    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        facingLeft = false;
        jumpsLeft = numJumps;
        captured = false;

        distanceToGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        lm = LayerMask.GetMask("Ground");
        
        startingSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        startingScale = transform.lossyScale;

        //

        setMovementStats(gameObject.GetComponent<MovementStats>());
        startingColliderSize = gameObject.GetComponent<BoxCollider2D>().size;
        startingColliderOffset = gameObject.GetComponent<BoxCollider2D>().offset;

        wormScale = transform.localScale;
        leftScale = transform.localScale;
        rightScale = transform.localScale;
        rightScale.x *= -1;
    }

    void Update()
    {
        if (Input.GetKey("d") && !Input.GetKey("a"))
        {
            walkVelocity = new Vector2(1f * moveSpeedMultiplier, 0f);

            if (facingLeft)
                facingLeft = false;
        }
        else if (Input.GetKey("a") && !Input.GetKey("d"))
        {
            //rigidbody.AddForce(new Vector2((Mathf.Sin(wormyMotionTimer++) + 1f) * -5f, 0));
            walkVelocity = new Vector2(-1f * moveSpeedMultiplier, 0f);

            if (!facingLeft)
                facingLeft = true;
        }
        else
        {
            walkVelocity = Vector2.zero;
        }

        if (walkVelocity.x > maxWalkSpeed)
            walkVelocity = new Vector2(maxWalkSpeed, 0f);
        else if (walkVelocity.x < -1f * maxWalkSpeed)
            walkVelocity = new Vector2(-1f * maxWalkSpeed, 0f);

        if (Mathf.Abs(rigidbody.velocity.x) < maxWalkSpeed)
            rigidbody.velocity += walkVelocity;

        if (facingLeft)
        {
            transform.localScale = leftScale;
        }
        else
        {
            transform.localScale = rightScale;
        }

        groundCheck();
        jumpCheck();
        captureCheck();

        animator.SetBool("facingLeft", facingLeft);
        animator.SetBool("grounded", grounded);
    }


    /// <summary>
    /// Checks for jump input from player.
    /// </summary>
    private void jumpCheck()
    {

        if (Input.GetMouseButton(0) && jumpsLeft != 0)
        {
            arrow.GetComponent<SpriteRenderer>().enabled = true;
            Vector3 playerToMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            playerToMouse.z = 0;
            float arrowMag = playerToMouse.magnitude;


            //Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.cyan);
            //arrow.transform.forward = jumpVel;
            //arrow.transform.rotation = arrow.transform.RotateAround()
            //arrow.transform.up =Vector3.RotateTowards(arrow.transform.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, 1f, 1f);

            //Debug.Log("Distance Vector = " + playerToMouse + "\nMagnitude = " + playerToMouse.magnitude);


            Vector3 arrowRot = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            arrowRot.z = 0;


            arrow.transform.up = arrowRot;

            

            //makes sure that the arrow does not display past 6 units or less than 1 unit
            if(arrowMag>=1f&&arrowMag<=6f)
            {
                arrow.transform.localScale = new Vector3(arrow.transform.localScale.x,
                                                    arrowMag*0.8f,
                                                    arrow.transform.localScale.z);
            }
            else if(arrowMag<1f)
            {
                arrow.transform.localScale = new Vector3(arrow.transform.localScale.x,
                                                    1f,
                                                    arrow.transform.localScale.z);
                playerToMouse.Normalize();
                arrowMag = 1f;
            }
            else
            {
                arrow.transform.localScale = new Vector3(arrow.transform.localScale.x,
                                                    5f,
                                                    arrow.transform.localScale.z);

                playerToMouse = Vector3.ClampMagnitude(playerToMouse, 5f);
                arrowMag = 5f;
            }
            

            facingLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;

            jumpVel = playerToMouse;
            
            
            animator.SetBool("jumpPrep", true);
        }
        else if (jumpsLeft != 0 && Input.GetMouseButtonUp(0))       //on mouse let go  AKA actual jump physics
        {
            //jumpVel = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //jumpVel.z = 0f;
            //if (jumpVel.magnitude > 6f)
            //{
            //    jumpVel.Normalize();
            //    jumpVel *= 6f;
            //}
            //else if (jumpVel.magnitude < 1f)
            //{
            //    jumpVel.Normalize();
            //}

            //not deactivatin for a sec

            //Debug.Log("Original vector: " + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) + "\nNormalized Vel: " +jumpVel+"\n");
            if (jumpVel.magnitude >= maxWalkSpeed * 3.5f)
            {
                jumpVel = Vector3.ClampMagnitude(jumpVel, maxWalkSpeed * 3.45f);
            }

            jumpVel *= jumpVelMultiplier;

            if (ejecting)
            {
                jumpVel *= 1.5f;
                ejecting = false;
                setMovementStats(gameObject.GetComponent<MovementStats>());
                gameObject.GetComponent<SpriteRenderer>().sprite = startingSprite;

                gameObject.GetComponent<BoxCollider2D>().size = startingColliderSize;
                gameObject.GetComponent<BoxCollider2D>().offset = startingColliderOffset;
                distanceToGround = GetComponent<BoxCollider2D>().bounds.extents.y;
            }

            rigidbody.velocity = jumpVel;

            jumpsLeft--;
            grounded = false;
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().enabled = false;
        }

        

    }

    private void abilityCheck()
    {

        if (ability=="dash"&&Input.GetMouseButton(1))
        {
            dashing = true;
        }

        if (dashing)
        {

        }
            
    }

    /// <summary>
    /// Determines whether or not the player is grounded
    /// </summary>
    private void groundCheck()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(0f, -1f * distanceToGround, 0f), Color.cyan);

        //Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit2D groundCheckResult = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround + 0.1f, lm);
        if (groundCheckResult && groundCheckResult.collider.gameObject.tag != "Player")
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
        animator.SetBool("jumpPrep", false);
    }


    /// <summary>
    /// Checks for player input to capture.
    /// If Capture button pressed, check for Enemy that can be captured.
    /// If both, set players movement stats to that of Enemy.
    /// </summary>
    private void captureCheck()
    {
        RaycastHit2D[] circCheck = Physics2D.CircleCastAll(transform.position, gameObject.GetComponent<BoxCollider2D>().bounds.size.x*2f, Vector2.zero, 0f);
        if (Input.GetKey(KeyCode.E))
        {
            if (captured)
            {
                captured = false;
                ejecting = true;

                leftScale = wormScale;
                rightScale = wormScale;
                rightScale.x *= -1;

                transTarget.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                foreach (RaycastHit2D hit in circCheck)
                {
                    if (hit && hit.collider.tag == "CanCapture" && !captured)
                    {
                        Debug.Log("Captured fhella");
                        setMovementStats(hit.collider.gameObject.GetComponent<MovementStats>());
                        capturing = true;
                        captureFramesLeft = CAPTURE_FRAMES;
                        transTarget = hit.collider.gameObject;
                        transTarget.GetComponent<Rigidbody2D>().simulated = false;
                        gameObject.GetComponent<SpriteRenderer>().sprite = transTarget.GetComponent<SpriteRenderer>().sprite;
                        transform.localScale = transTarget.transform.localScale;
                        //gameObject.GetComponent<BoxCollider2D>().autoTiling
                        transTarget.GetComponent<SpriteRenderer>().enabled = false;
                        transTarget.GetComponent<Robot>().captured = true;

                        gameObject.GetComponent<BoxCollider2D>().size = transTarget.GetComponent<BoxCollider2D>().size;
                        gameObject.GetComponent<BoxCollider2D>().offset = transTarget.GetComponent<BoxCollider2D>().offset;
                        distanceToGround = gameObject.GetComponent<BoxCollider2D>().bounds.extents.y;

                        leftScale = transform.localScale;
                        rightScale = transform.localScale;
                        leftScale.x = Mathf.Abs(rightScale.x);
                        rightScale.x = Mathf.Abs(rightScale.x) * -1;

                        break;
                    }
                }
            }

        }

        if(capturing&&captureFramesLeft>0)
        {
            rigidbody.velocity = (transTarget.transform.position - transform.position);
            captureFramesLeft--;
        }
        else if(capturing&&captureFramesLeft==0)
        {
            capturing = false;
            captured = true;
        }


    }

    private void setMovementStats(MovementStats newStats)
    {
        jumpVelMultiplier = newStats.jumpVelMult;
        maxWalkSpeed = newStats.maxMoveSpeed;
        numJumps = newStats.numJumps;
    }

}
