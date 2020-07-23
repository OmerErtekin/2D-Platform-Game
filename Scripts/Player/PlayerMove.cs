using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

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

    private float jumpSpeed = 650f;
    public float movementSpeed = 5f;
    private int damage = 50;
    private float attackRange = 0.7f;
    private float attackCoolDown = 0.5f;
    private float lastAttackTime = 0;
    private float lastCrouchTime = 0;
    private float crouchCoolDown = 5f;
    private float lastJumpTime = 0;
    private float jumpCoolDown = 0.5f;
    private LayerMask EnemyLayer;
    private LayerMask MageLayer;
    private LayerMask BossLayer;
    private Rigidbody2D playerRB;
    private Animator PlayerAnimator;
    private BoxCollider2D footCollider;
    public int PlayerHealth;
    public Transform attackPosition;
    private bool isAtacking;
    public bool isDodging ;
    public bool isCrouching;
    public bool isFacingRight = true;
    public bool isAlive = true;
    public HealthBar healthBar;
    public Joystick joystick;
    float moveInputHorizontal;
    float moveInputVertical;



    void Start()
    {
        PlayerHealth = 100;
        BossLayer = LayerMask.GetMask("Boss");
        EnemyLayer = LayerMask.GetMask("Enemy");
        MageLayer = LayerMask.GetMask("MageEnemy");
        PlayerAnimator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        footCollider = GetComponent<BoxCollider2D>();
        healthBar.SetMaxHealth(PlayerHealth);
        
    }

    void Update()
    {
        moveInputHorizontal = joystick.Horizontal;
        moveInputVertical = joystick.Vertical;
        
        //instantiate bool variables for move, if not the character couldn't move until he attack and dash.
        isAtacking = false;
        isDodging = false;

        if (isAlive == true)
        {
            healthBar.SetHealth(PlayerHealth);
            if(PlayerHealth <= 0 || footCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
            {
                healthBar.SetHealth(0);
                Die();
            }
            if (CrossPlatformInputManager.GetButton("Attack") && Time.time > lastAttackTime)
            {
                lastAttackTime = Time.time + attackCoolDown;
                StartCoroutine(ActiveAttack());
                Attack();

            }



            
            Dodge();
            //player cant move when he is attacking or dashing, however he can break the movement statement by doing attack or dash.
            if (isAtacking == false && isDodging == false)
            {
                if (moveInputVertical > 0.3f && Time.time > lastJumpTime)
                {
                    lastJumpTime = Time.time + jumpCoolDown;
                    Jump();
                }
                if (moveInputVertical < -0.3f && Time.time > lastCrouchTime)
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
        if (isCrouching == false && isAlive == true && isAtacking == false)
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
        if (moveInputHorizontal > 0.2f)
        {
            playerRB.velocity = new Vector2(movementSpeed, playerRB.velocity.y);
        }
        else if(moveInputHorizontal < -0.2f)
        {
            playerRB.velocity = new Vector2(-movementSpeed, playerRB.velocity.y);
        }
        
        if(playerRB.velocity.x != 0 && isCrouching == false && isAlive == true)
        {
            PlayerAnimator.SetInteger("State", 2);

        }
         // changing sprites direction with FlipCharacter();
            if (moveInputHorizontal > 0 && !isFacingRight) FlipCharacter();
            else if (moveInputHorizontal < 0 && isFacingRight) FlipCharacter();
        

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
        if (CrossPlatformInputManager.GetButton("Dodge") && footCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            isDodging = true;
            PlayerAnimator.SetInteger("State", 4);
        }
    }
    void Attack()
    {

            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position,attackRange,EnemyLayer);
            Collider2D[] magesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, MageLayer);
            Collider2D[] bossToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, BossLayer);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<Enemy>().GetDamage(damage);
            }
            for(int j = 0; j<magesToDamage.Length; j++)
            {
                magesToDamage[j].GetComponent<EnemyMage>().GetDamage(damage*2);
            }
            for(int k = 0; k<bossToDamage.Length; k++)
            {
                bossToDamage[k].GetComponent<BossScript>().GetDamage(damage);
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
        if (isAtacking == false)
        {
            PlayerAnimator.SetInteger("State", 6);
        }
        PlayerHealth -= damage;
        
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
    IEnumerator ActiveAttack()
    {
        isAtacking = true;
        PlayerAnimator.SetInteger("State", 1);
        yield return new WaitForSeconds(0.5f);
        isAtacking = false;
    }
}
