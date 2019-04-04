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
    //[Range(0f, 3f)]
    public float maxWalkSpeed;
    public bool facingLeft;
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

        animator.SetTrigger("prepairingToJump");
        animator.SetTrigger("jumping");
        animator.SetTrigger("landed");
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
            Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.cyan);

            facingLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;

            animator.SetBool("jumpPrep", true);
        }

        if (Input.GetMouseButtonUp(0) && jumpsLeft != 0)
        {
            jumpVel = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            jumpVel.z = 0f;
            if (jumpVel.magnitude > 6f)
            {
                jumpVel.Normalize();
                jumpVel *= 6f;
            }
            else if (jumpVel.magnitude < 1f)
            {
                jumpVel.Normalize();
            }

            //Debug.Log("Original vector: " + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) + "\nNormalized Vel: " +jumpVel+"\n");
            if (jumpVel.magnitude >= maxWalkSpeed * 3.5f)
            {
                jumpVel=Vector3.ClampMagnitude(jumpVel, maxWalkSpeed * 3.45f);
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

            animator.SetTrigger("jump");
        }
    }

    private void abilityCheck()
    {

        if (ability=="dash"&&Input.GetMouseButton(1))
        {
            dashing == true;
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
                        rightScale.x *= -1;

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
