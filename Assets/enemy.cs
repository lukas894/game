using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
    public void gotHit(float health)
    {

        currentHealth -= health;

        if (currentHealth < 0)
        {
            Player.killCount += 1;
            spawner.enemyCount -= 1;
            Destroy(this.gameObject);
        }


        justGotHit = true;
        healthIndicator.color = new Color(currentHealth / 100, currentHealth / 100, currentHealth / 100);
    }


    public GameObject square;
    public GameObject cliffCheckerLeft;
    public GameObject cliffCheckerRight;
    private player Player;
    private enemySpawner spawner;

    public float currentHealth = 100f;
    public float walkspeed = 0.5f;
    public float runSpeed = 6f;
    public float knockback = 7f;
    public float jumpForce = 4.5f;
    public float attackRangeX = 1f;
    public float attackRangeY = 1f;
    public float attackSpeed = 0.4f;
    public float approximateDmg = 10f;

    
    Rigidbody2D rb;
    SpriteRenderer spr;
    Animator ani;

    private bool justGotHit = false;
    private bool grounded = false;
    private float timer = 0f;
    private float attackTimer = 0f;
    private float NPCtimer = 0f;
    private float decision;
    private float NPCtime;
    private int cliffClose;
    private SpriteRenderer healthIndicator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        healthIndicator = square.GetComponent<SpriteRenderer>();
        Player = FindObjectOfType<player>();
        spawner = FindObjectOfType<enemySpawner>();

        // // STATS // //
        float width = Random.Range(1f, 1.4f);
        float height = Random.Range(0.68f, 1.76f);
        transform.localScale = new Vector3(width, height, 0);
        runSpeed = runSpeed * 1 / height;
        NPCtime = Random.value + 1f;
        knockback = 7 - 7 * Player.killCount / 100;
        if (knockback < 0)
        {
            knockback = 0;
        }

        square.transform.localScale = new Vector3(0.2f / transform.localScale.x, 0.2f / transform.localScale.y, 1);
    }


    void Update()
    {
        Vector2 player = Player.transform.position;

        // // PLAYER IS IN VISIBLE RANGE // //
        if (((player - (Vector2)transform.position).magnitude < 10f && ((player.x < transform.position.x && spr.flipX == true) || (player.x > transform.position.x && spr.flipX == false))) || (player - (Vector2)transform.position).magnitude < 1.5f || justGotHit)
        {
            // // PLAYER IS ON LEFT // //
            if (player.x < transform.position.x)
            {
                // // ATTACKING // //
                if ((transform.position.x - player.x) < attackRangeX)
                {
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= attackSpeed)
                    {
                        Attack(true);
                        attackTimer = 0;
                    }
                    ani.SetBool("attacking", true);
                }
                else
                {
                    ani.SetBool("attacking", false);
                }
                // // WALKING // //
                if (!justGotHit)
                {
                    rb.velocity = new Vector2(-runSpeed * currentHealth / 100, rb.velocity.y);
                    spr.flipX = true;
                    ani.SetBool("walking", true);
                }
                // // GOT HIT BY BULLET // //
                else
                {
                    rb.velocity = new Vector2(knockback, rb.velocity.y);
                    timer += Time.deltaTime;

                    if (timer > 0.25f)
                    {
                        justGotHit = false;
                        timer = 0f;
                        ani.SetBool("jumping", true);
                    }
                }
            }
            // // PLAYER IS ON RIGHT // //
            else if (player.x > transform.position.x)
            {
                // // ATTACKING // //
                if ((player.x - transform.position.x) < attackRangeX)
                {
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= attackSpeed)
                    {
                        Attack(false);
                        attackTimer = 0;
                    }
                    ani.SetBool("attacking", true);
                }
                else
                {
                    ani.SetBool("attacking", false);
                }
                // // WALKING // //
                if (!justGotHit)
                {
                    rb.velocity = new Vector2(runSpeed * currentHealth / 100, rb.velocity.y);
                    spr.flipX = false;
                    ani.SetBool("walking", true);
                }

                // // GOT HIT BY BULLET // //
                else
                {
                    rb.velocity = new Vector2(-knockback, rb.velocity.y);
                    timer += Time.deltaTime;

                    if (timer > 0.25f)
                    {
                        justGotHit = false;
                        timer = 0f;
                        ani.SetBool("jumping", true);
                    }

                }
            }

        }
        // // PLAYER NOT IN VISIBLE RANGE // //
        else
        {
            if (NPCtimer > NPCtime)
            {
                NPCtimer = 0;
                NPCtime = Random.value + 1f;
                decision = Random.value;
            }

            NPCtimer += Time.deltaTime;
            int playerDirCoef;
            if (player.x < transform.position.x)
            {
                playerDirCoef = -1;
            }
            else
            {
                playerDirCoef = 1;
            }

            // // PLAYER DIRECTION // //
            if (decision <= 0.4f && !(cliffClose == playerDirCoef))
            {

                rb.velocity = new Vector2(playerDirCoef * walkspeed, rb.velocity.y);
                ani.SetBool("walking", true);
                spr.flipX = playerDirCoef == -1;
            }
            // // OPPOSITE TO PLAYER DIRECTION // //
            else if (decision >= 0.8f && !(cliffClose == -playerDirCoef))
            {
                rb.velocity = new Vector2(-playerDirCoef * walkspeed, rb.velocity.y);
                ani.SetBool("walking", true);
                spr.flipX = playerDirCoef == 1;
            }
            // // IDLE // //
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                ani.SetBool("walking", false);
                ani.SetBool("attacking", false);
            }
        }


    }

    private void FixedUpdate()
    {
        // // ENEMY GROUNDED CHECK // //
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        if (transform.position.y <= -4.4)
        {
            Destroy(this.gameObject);
        }
        if (groundCheck)
        {
            grounded = true;
            ani.SetBool("jumping", false);
        }
        else
        {
            grounded = false;
            ani.SetBool("jumping", true);
        }

        // // CHECKING FOR CLIFFS // //
        RaycastHit2D checkLeft = Physics2D.Raycast(cliffCheckerLeft.transform.position, Vector2.down, 1f , LayerMask.GetMask("Ground"));
        RaycastHit2D checkRight = Physics2D.Raycast(cliffCheckerRight.transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(cliffCheckerLeft.transform.position, Vector2.down * 1f, Color.red);
        Debug.DrawRay(cliffCheckerRight.transform.position, Vector2.down * 1f, Color.red);
        if (grounded)
        {
            if (checkLeft && checkRight)
            {
                cliffClose = 0;
            }
            else if (!checkLeft)
            {
                cliffClose = -1;
            }
            else
            {
                cliffClose = 1;
            }

        }
        else
        {
            cliffClose = 2;
        }

    }
        

    private void Attack(bool left)
    {
        float dmg = Random.Range(-5, 5) + approximateDmg;

        if (left)
        {
            spr.flipX = true;
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position + 0.3f * Vector3.down, Vector2.left, attackRangeX, LayerMask.GetMask("Player"));
            Debug.DrawRay(transform.position + 0.3f * Vector3.down, Vector2.left * attackRangeX, Color.red);
            if (playerHit)
            {
                playerHit.collider.GetComponent<player>().gotHit(dmg, -1);
            }
        }
        else
        {
            spr.flipX = false;
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position + 0.3f * Vector3.down, Vector2.right, attackRangeX, LayerMask.GetMask("Player")); ;
            Debug.DrawRay(transform.position + 0.3f * Vector3.down, Vector2.right * attackRangeX, Color.red);
            if (playerHit)
            {
                playerHit.collider.GetComponent<player>().gotHit(dmg, 1);
            }
        }
    }


}
