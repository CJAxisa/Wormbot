using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 velocity;
    public GameObject worm; // the worm
    public float lifeSpan; // time bullet exists in seconds before it times out
    private float lifetime; // current life in seconds of the bullet
    private Vector3 toWorm; // direction vector to the worm
    private float rad; // radius of the bullet
    LayerMask lm;
    // Start is called before the first frame update
    void Start()
    {
        lifetime = 0;
        lm = LayerMask.GetMask("Ground");
        rad = this.GetComponent<CircleCollider2D>().radius;
        worm = GameObject.FindGameObjectWithTag("Player");
    }

    public void setVelocityRight() {
        this.velocity.x *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += velocity * Time.deltaTime;
        lifetime += Time.deltaTime;
        if (lifetime >= lifeSpan)
            Destroy(this.gameObject);

        // worm collision
        toWorm = worm.transform.position - this.transform.position;
        toWorm = toWorm.normalized;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, toWorm, rad);
        Debug.DrawLine(this.transform.position, (0.16f * toWorm) + this.transform.position, Color.white);
        if (hit == true && hit.collider.tag == "Player") {
            // do worm hit here
            Destroy(this.gameObject);
        }

        // terrain collision
        RaycastHit2D[] circCheck = Physics2D.CircleCastAll(this.transform.position, .5f * rad, this.transform.position + velocity,.5f * rad, lm);

        if (circCheck.Length > 0) {
            Destroy(this.gameObject);
        }

    }
}
