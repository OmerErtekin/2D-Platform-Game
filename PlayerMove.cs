using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    /* Animator States :
     * Idle : 0
     * Attack : 1
     * Walk : 2 
     * Jump : 3
     * Dodge : 4
     * Die : 5
     * Take Damage : 6
     * Crouch : 7
     */

    private float jumpSpeed = 800f;
    private float movementSpeed = 10f;
    private int damage = 25;
    private float attackRange = 0.7f;
    private float attackCoolDown = 0.5f;
    private float lastAttackTime = 0;
    private float lastCrouchTime = 0;
    private float crouchCoolDown = 5f;
    private LayerMask EnemyLayer;
    private LayerMask MageLayer;
    private Rigidbody2D playerRB;
    private Animator PlayerAnimator;
    private BoxCollider2D footCollider;
    public int PlayerHealth;
    public Transform attackPosition;
    private bool isAtacking;
    public bool isDodging ;
    public bool isCrouching;
    private bool isFacingRight = true;
    public bool isAlive = true;

    void Start()
    {
        PlayerHealth = 100;
        EnemyLayer = LayerMask.GetMask("Enemy");
        MageLayer = LayerMask.GetMask("MageEnemy");
        PlayerAnimator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        footCollider = GetComponent<BoxCollider2D>();
        
    }

    void Update()
    {
        //instantiate bool variables for move, if not the character couldn't move until he attack and dash.
        isAtacking = false;
        isDodging = false;

        if (isAlive == true)
        {

            if(PlayerHealth <= 0 || footCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
            {
                Die();
            }
            if (Input.GetMouseButtonDown(0) && Time.time > lastAttackTime)
            {
                lastAttackTime = Time.time + attackCoolDown;
                Attack();

            }



            
            Dodge();
            //player cant move when he is attacking or dashing, however he can break the movement statement by doing attack or dash.
            if (isAtacking == false && isDodging == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }
                if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time > lastCrouchTime)
                {
                    lastCrouchTime = Time.time + crouchCoolDown;
                    StartCoroutine(ActiveCrouch());
                }
                Run();
            }

        }
            
    }
    private void FixedUpdate()
    {
        if (isCrouching == false)
        {
            PlayerAnimator.SetInteger("State", 0);
        }
    }

    public void GainDamage(int extra)
    {
        damage += extra;
        //TODO : Add some gaind damage effects and sounds
    }
    public void Heal(int heal)
    {
        PlayerHealth += heal;
        // TODO : Add some heal effects and sounds
    }

    void Run()
    {
        float moveInput =Input.GetAxis("Horizontal");
        playerRB.velocity = new Vector2(moveInput * movementSpeed, playerRB.velocity.y);
        
        if(playerRB.velocity.x != 0 && isCrouching == false)
        {
            PlayerAnimator.SetInteger("State", 2);

        }
         // changing sprites direction with FlipCharacter();
            if (moveInput > 0 && !isFacingRight) FlipCharacter();
            else if (moveInput < 0 && isFacingRight) FlipCharacter();
        

    }
    void Jump()
    {
            //multi jump skill disabled.
            if (!footCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
            playerRB.AddForce(new Vector2(0f, jumpSpeed));
            PlayerAnimator.SetInteger("State", 3);
        
        
    }

    void Dodge()
    {
        if (Input.GetMouseButton(1)){
            // TODO : use isDodging bool controller for blocking or reducing the received damage while player is dodging.
            isDodging = true;
            PlayerAnimator.SetInteger("State", 4);
        }
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAtacking = true;
            PlayerAnimator.SetInteger("State", 1);
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position,attackRange,EnemyLayer);
            Collider2D[] magesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, MageLayer);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<Enemy>().GetDamage(damage);
            }
            for(int j = 0; j<magesToDamage.Length; j++)
            {
                magesToDamage[j].GetComponent<EnemyMage>().GetDamage(damage);
            }

        }
    }

    void FlipCharacter()
    {
        // for understand that check  https://www.youtube.com/watch?v=Xnyb2f6Qqzg&feature=youtu.be&t=38m39s 
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }



    private void OnDrawGizmosSelected() // draw the radius of attack
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }


    public void TakeDamage(int damage)
    {
        PlayerAnimator.SetInteger("State", 6);
        PlayerHealth -= damage;
        Debug.Log(PlayerHealth);
        
    }

    private void Die()
    {
        PlayerAnimator.SetInteger("State", 5);
        isAlive = false;
        StartCoroutine(ClearPlayer());
    }
    

    IEnumerator ClearPlayer()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }

    IEnumerator ActiveCrouch()
    {
        isCrouching = true;
        PlayerAnimator.SetInteger("State", 7);
        yield return new WaitForSeconds(1);
        isCrouching = false;
    }
}
