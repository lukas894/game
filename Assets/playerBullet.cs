using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour
{
    private float bulletSpeed = 9f;
    private float damage = 33f;
    private float damageOffset = 10f;


    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = bulletSpeed*angleTonVector(transform.rotation.eulerAngles.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Ground")
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag == "Enemy")
        {
            float dmg = Random.Range(-damageOffset, damageOffset) + damage;
            collision.gameObject.GetComponent<enemy>().gotHit(dmg);

            
            Destroy(this.gameObject);

            
        }

    }

    private Vector2 angleTonVector(float angle)
    {
        angle = (Mathf.PI / 180) * angle;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    
}
