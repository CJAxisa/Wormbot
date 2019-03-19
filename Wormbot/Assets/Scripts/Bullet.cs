using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 velocity;
    public float lifeSpan;
    private float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        lifetime = 0;
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
    }
}
