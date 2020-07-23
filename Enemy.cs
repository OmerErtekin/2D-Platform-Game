using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    /* Animator States:
     * Idle : 0
     * Run : 1
     * Attack 2
     * Die 3
     */
    private float distanceBetweenPlayer;
    private float EnemySpeed = 5f;
    private int EnemyHealth = 100;
    private int damage = 10;
    private float enemyNextAttack;
    private float enemyAtackRate = 1f;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private Rigidbody2D enemyRB;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private bool isEnemyAlive;
    private bool isFacingRight = false;

    void Start()
    {
        playerObjectTransform = GameObject.FindWithTag("Player").transform;
        isEnemyAlive = true;
        enemyAnimator = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (EnemyHealth <= 0)
        {
            Die();
        }

        if (isEnemyAlive && playerScript.isAlive)
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

    public void GetDamage(int damage)
    {
        EnemyHealth -= damage;
        // TODO : add take damage effect and sounds
       
    }
    private void Die()
    {
        isEnemyAlive = false;
        enemyAnimator.SetInteger("EnemyState", 3);
        enemyCollider.enabled = false;
        StartCoroutine(ClearTheEnemy());
    }

    private void Attack()
    {
        if (Time.time > enemyNextAttack)
        {
            enemyNextAttack = Time.time + enemyAtackRate;
            enemyAnimator.SetInteger("EnemyState", 2);
            if(CanGiveDamage())
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    private IEnumerator ClearTheEnemy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private void MoveToPlayer()
    {
        enemyAnimator.SetInteger("EnemyState", 1);
        transform.position =Vector2.MoveTowards(transform.position,new Vector2(playerObjectTransform.position.x,transform.position.y),EnemySpeed*Time.deltaTime);

    }

    private void SearchForPlayer()
    {
        distanceBetweenPlayer = Mathf.Abs(playerObjectTransform.position.x - transform.position.x);
        if(distanceBetweenPlayer <= 2f)
        {
            Attack();
        }
        else if (distanceBetweenPlayer <= 10f)
        {
            MoveToPlayer();
        }
        else
        {
            enemyAnimator.SetInteger("EnemyState", 0);
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
    bool CanGiveDamage()
    { // check for enemy dodge to which way. If he is turn his back and press dodging, enemy can still damage because the sword doesn't block him.
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
}
