using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    float damage;
    float bulletVel;
    float bulletRange;
    Vector3 bulletAngle;
    Vector2 origin;
    Rigidbody2D rb2d;
    public LayerMask obstacleLayer;
    public LayerMask zombieLayer;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = bulletVel * (new Vector2(Mathf.Cos(bulletAngle.z * Mathf.Deg2Rad), Mathf.Sin(bulletAngle.z * Mathf.Deg2Rad)));
        if (Vector2.Distance(transform.position, origin) >= bulletRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject hit = collision.gameObject;
        if (hit != null)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                collision.gameObject.GetComponent<Obstacle>().TakeDamage(damage);
                Destroy(gameObject);
            }
            if (collision.gameObject.tag == "Zombie")
            {
                collision.gameObject.GetComponent<Zombie>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void SetBullet(float dmg, float vel, float rng, Vector2 firePos, Vector3 angle)
    {
        damage = dmg;
        bulletVel = vel;
        bulletRange = rng;
        origin = firePos;
        bulletAngle = angle;
        transform.rotation = Quaternion.Euler(angle);
        transform.position = firePos;
    }
}
