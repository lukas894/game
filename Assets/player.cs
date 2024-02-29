using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public void gotHit(float dmg, int dir)
    {
        health -= dmg;

        if (health <= 0)
        {
            health = 100f;
            transform.position = new Vector2(-120, 10);

            killCount = 0;
            
        }
        healthSpr.color = new Color(health / 100, health / 100, health / 100);

        rb.velocity = new Vector2(dir*knockback, 0);
        justGotHit = true;
    }



    Rigidbody2D rb;
    SpriteRenderer spr;
    SpriteRenderer healthSpr;
    Animator ani;

    public GameObject backGround;
    public GameObject gunPoint;
    public GameObject bulletPrefab;
    public GameObject healthIndicator;
    public Text killCountUI;


    public float walkSpeed = 5.5f;
    public float movebackGroundSpeed = 0.05f;
    public float jumpForce = 5f;
    public float knockback = 5f;
    public float stunTime = 0.3f;
    public int killCount = 0;

    private float backGroundStoreX = 0f;
    private bool grounded = false;
    private bool justGotHit = false;
    private float health = 100f;
    private float timer = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        healthSpr = healthIndicator.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // // INPUT // //
        float hor = Input.GetAxisRaw("Horizontal");
        float smoothHor = Input.GetAxis("Horizontal");
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool crouch = Input.GetKey("left ctrl");
        bool fire = Input.GetMouseButtonDown(0);

        
        // // MOVEMENT // //
        if (hor > 0)
        {
            // // GOT HIT BY AN ENEMY // //
            if (justGotHit)
            {

            }
            // // IN AIR MOVEMENT // //
            else if (!grounded)
            {
                rb.velocity = new Vector2(walkSpeed*hor, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            }
            spr.flipX = false;
            backGroundStoreX -= linearMove(backGroundStoreX, -1);
            backGround.transform.position = new Vector2(transform.position.x + backGroundStoreX, transform.position.y);
            ani.SetBool("walking", true);
        }
        else if (hor < 0)
        {
            // // GOT HIT BY AN ENEMY // //
            if (justGotHit)
            {
                if (timer >= stunTime)
                {
                    justGotHit = false;
                    timer = 0;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            // // IN AIR MOVEMENT // //
            else if (!grounded)
            {
                rb.velocity = new Vector2(walkSpeed * hor, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
            }
            spr.flipX = true;
            backGroundStoreX += linearMove(backGroundStoreX, 1);
            backGround.transform.position = new Vector2(transform.position.x + backGroundStoreX, transform.position.y);
            ani.SetBool("walking", true);
        }
        else
        {
            // // GOT HIT BY AN ENEMY // //
            if (justGotHit)
            {
                if (timer >= stunTime)
                {
                    justGotHit = false;
                    timer = 0;
                    
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                ani.SetBool("walking", false);
            }
        }

        // // UP ~ DOWN // //
        if (jump && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (crouch && grounded)
        {
            rb.velocity = Vector2.zero;
        }

        // // SHOOTING // //
        Vector2 direction = (-transform.position + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (fire && grounded)
        {
            float bulletAngle = vectorToangle(direction);

            Instantiate(bulletPrefab, gunPoint.transform.position, Quaternion.Euler(new Vector3(0, 0, bulletAngle)));

            // // PLAYER NOT CORRECTLY ROTATED // //
            if (spr.flipX && (bulletAngle < 90 || bulletAngle > 270))
            {
                spr.flipX = false;
            }
            else if (!spr.flipX && (bulletAngle > 90 && bulletAngle < 270))
            {
                spr.flipX = true;
            }

            ani.SetBool("shooting", true);
        }
        else
        {
            ani.SetBool("shooting", false);
        }


    }

    private void FixedUpdate()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);
        if (transform.position.y <= -4.4)
        {
            transform.position = new Vector2(0, 0);
        }
        if (hitGround)
        {
            grounded = true;
            ani.SetBool("jumping", false);
        }
        else
        {
            grounded = false;
            ani.SetBool("jumping", true);
        }

        killCountUI.text = "Enemies killed: " + killCount.ToString();
    }

    private float linearMove(float self, int n)
    {
        if ((self <= 3.11 || n == -1) && (self >= -5.71 || n == 1))
        {
            return movebackGroundSpeed;
        }
        
        return 0;
    }

    float vectorToangle(Vector2 vector)
    {
        vector = vector.normalized;
        if (vector.x >= 0)
        {
            if (vector.y >= 0)
            {
                return (180 / Mathf.PI) * Mathf.Asin(Mathf.Abs(vector.y));
            }
            else
            {
                return 360 - (180 / Mathf.PI) * Mathf.Asin(Mathf.Abs(vector.y));
            }
        }
        else
        {
            if (vector.y > 0)
            {
                return 180 - (180 / Mathf.PI) * Mathf.Asin(Mathf.Abs(vector.y));
            }
            else
            {
                return 180 + (180 / Mathf.PI) * Mathf.Asin(Mathf.Abs(vector.y));
            }
        }

    }
}
