using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{

    /* Animator States:
     * Idle : 0
     * Run : 1
     * Attack : 2
     * Die 3
     */
    private float distanceBetweenPlayer;
    private float EnemySpeed = 5f;
    private int EnemyHealth = 200;
    private int damage = 10;
    private float enemyNextAttack;
    private float enemyAtackRate = 1f;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private Rigidbody2D enemyRB;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private bool isEnemyAlive = true;
    private bool isFacingRight = false;
    private bool isAtacking = false;
    private bool isTeleported = false;
    private SpriteRenderer bossRenderer;

    void Start()
    {
        playerObjectTransform = GameObject.FindWithTag("Player").transform;
        enemyAnimator = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRB = GetComponent<Rigidbody2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isEnemyAlive == true)
        {
            if (EnemyHealth <= 0)
            {
                Die();
            }
            if (EnemyHealth <= 100 && playerScript.isAlive == true)
            {
                StealthBoss();
            }

            if (isEnemyAlive && playerScript.isAlive && isAtacking == false)
            {
                SpriteDirectionControl();
                SearchForPlayer();
            }
            else if (!playerScript.isAlive)
            {
                // TODO : you can do some victory effects for enemy.
                enemyAnimator.SetInteger("EnemyState", 0);
            }
        }
        if(playerScript.isAlive == false){
            bossRenderer.enabled = true;
        }
      
    }

    public void GetDamage(int damage)
    {
        EnemyHealth -= damage;
        // TODO : add take damage effect and sounds

    }
    private void Die()
    {
        isEnemyAlive = false;
        enemyCollider.enabled = false;
        enemyAnimator.SetInteger("EnemyState", 3);
        enemyCollider.enabled = false;
        StartCoroutine(ClearTheEnemy());
    }

    private void Attack()
    {
        if (Time.time > enemyNextAttack)
        {
            enemyAnimator.SetInteger("EnemyState", 2);
            StartCoroutine(SetAttackActive());
            enemyNextAttack = Time.time + enemyAtackRate;

            if (CanGiveDamage())
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    private IEnumerator ClearTheEnemy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private void MoveToPlayer()
    {

            enemyAnimator.SetInteger("EnemyState", 1);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), EnemySpeed * Time.deltaTime);
        
    }

    private void SearchForPlayer()
    {
        distanceBetweenPlayer = Mathf.Abs(playerObjectTransform.position.x - transform.position.x);
        if (distanceBetweenPlayer <= 1f)
        {
            StartCoroutine(SetAttackActive());
            Attack();
        }
        else if (distanceBetweenPlayer <= 10f)
        {
            if (isAtacking == false)
            {
                MoveToPlayer();
            }
        }
        else
        {
            if (isAtacking == false)
            {
                enemyAnimator.SetInteger("EnemyState", 0);
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

    void SpriteDirectionControl()
    {
        if (playerObjectTransform.position.x - transform.position.x < 0 && isFacingRight == false) FlipCharacter();
        else if (playerObjectTransform.position.x - transform.position.x > 0 && isFacingRight == true) FlipCharacter();
    }

    IEnumerator SetAttackActive()
    {
        isAtacking = true;
        yield return new WaitForSeconds(0.5f);
        isAtacking = false;
    }

    void StealthBoss()
    {
       // isStealth = true; // use it for add laugh effects
        if (isAtacking == false)
        {
            if (isTeleported == false)
            {
                StartCoroutine(Teleport());
            }
            bossRenderer.enabled = false;
        }
        else
        {
            bossRenderer.enabled = true;
        }
    }

    bool CanGiveDamage()
    {
        if (playerScript.isDodging == true && playerScript.isFacingRight == true && playerObjectTransform.position.x - transform.position.x < 0)
        {
            return false;
        }
        else if (playerScript.isDodging == true && playerScript.isFacingRight == false && playerObjectTransform.position.x - transform.position.x >= 0)
        {
            return false;
        }

        else return true;
        
    }

    IEnumerator Teleport()
    {
        isTeleported = true;
        transform.position = new Vector2(transform.position.x + Random.Range(-5, 5), transform.position.y);
        yield return new WaitForSeconds(3);
        isTeleported = false;
    }


}
