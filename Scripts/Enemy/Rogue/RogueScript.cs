using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueScript : MonoBehaviour
{ 
    /* Animator States:
     * Idle : 0
     * Run : 1
     * Attack : 2
     * Die : 3
     * Jump attack : 4
     */
    private float distanceBetweenPlayer;
    private float EnemySpeed = 5f;
    private int EnemyHealth = 150;
    private int damage = 5;
    private float enemyNextAttack;
    private float enemyAtackRate = 0.68f;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private BoxCollider2D enemyFootCollider;
    private ParticleSystem spellEffect;
    private bool isEnemyAlive;
    private bool isJumped = false;
    public bool isSkilled = false;
    private bool isFacingRight = false;


    void Start()
    {
        playerObjectTransform = GameObject.FindWithTag("Player").transform;
        isEnemyAlive = true;
        spellEffect = GetComponentInChildren<ParticleSystem>();
        enemyAnimator = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyFootCollider = GetComponent<BoxCollider2D>();

    }

    void Update()
    {
       

        if (EnemyHealth <= 0 || enemyFootCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
        {
            Die();
        }
        
        if (isEnemyAlive && playerScript.isAlive && !isSkilled)
        {
                SpriteDirectionControl();
                SearchForPlayer();
        }
        else if(isEnemyAlive && isSkilled)
        {
            enemyAnimator.SetInteger("EnemyState", 0);
        }
        else if (!playerScript.isAlive)
        {
                // TODO : you can do some victory effects for enemy.
                enemyAnimator.SetInteger("EnemyState", 0);
        }
        

    }

    public void GetSkill(int damage)
    {
        spellEffect.Play();
        EnemyHealth -= damage;
    }
    public void GetDamage(int damage)
    {
   
        EnemyHealth -= damage;
        // TODO : add take damage effect and sounds

    }
    private void Die()
    {
        if (isEnemyAlive == true)
        {
         
        }
        isEnemyAlive = false;
        enemyAnimator.SetInteger("EnemyState", 3);
        StartCoroutine(ClearTheEnemy());
    }

    private void Attack()
    {
        if (Time.time > enemyNextAttack)
        {
            enemyNextAttack = Time.time + enemyAtackRate;
            enemyAnimator.SetInteger("EnemyState", 2);
            StartCoroutine(attackWithDelay());
        }
    }

    private void JumpAttack()
    {
        if(playerObjectTransform.position.x-transform.position.x < 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), 100 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), -100* Time.deltaTime);
        }
        enemyAnimator.SetInteger("EnemyState", 4);
        isJumped = true;
        if (CanGiveDamage())
        {

            playerScript.TakeDamage(damage*2);
        }
        else
        {
            playerScript.dodgeParticle.Play();
            playerScript.dodgeSound.Play();
        }
    }

    private void MoveToPlayer()
    {
        enemyAnimator.SetInteger("EnemyState", 1);
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), EnemySpeed * Time.deltaTime);

    }

    private void SearchForPlayer()
    {
        distanceBetweenPlayer = Mathf.Abs(playerObjectTransform.position.x - transform.position.x);
        if (distanceBetweenPlayer <= 4 && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2 && isJumped == false)
        {
            JumpAttack();
        }

        else if (distanceBetweenPlayer <= 1.2f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2)
        {
            Attack();
        }
        else if (distanceBetweenPlayer <= 10f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2 && distanceBetweenPlayer > 1.2f)
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
    private IEnumerator attackWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (CanGiveDamage())
        {
           
            playerScript.TakeDamage(damage);
        }
        else
        {
            playerScript.dodgeParticle.Play();
            playerScript.dodgeSound.Play();
        }
    }


    private IEnumerator ClearTheEnemy()
    {
        enemyCollider.enabled = false;
        yield return new WaitForSeconds(1);
        isEnemyAlive = true;
        enemyCollider.enabled = true;
        EnemyHealth = 75;
        isJumped = true;
        yield return new WaitForSeconds(0.1f);
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

    }
}
