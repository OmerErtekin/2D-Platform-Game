using System.Collections;
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
    private int EnemyHealth = 150;
    private int damage = 5;
    private float enemyNextAttack;
    private float enemyAtackRate = 1f;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private BoxCollider2D enemyFootCollider;
    private ParticleSystem spellEffect;
    private bool isEnemyAlive;
    private bool isFacingRight = false;
    public bool isSkilled = false;
    private AudioSource attackSound;
    private AudioSource deathSound;
    private AudioSource takeDamageSound;

    void Start()
    {
        playerObjectTransform = GameObject.FindWithTag("Player").transform;
        isEnemyAlive = true;
        enemyAnimator = GetComponent<Animator>();
        spellEffect = GetComponentInChildren<ParticleSystem>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyFootCollider = GetComponent<BoxCollider2D>();
        AudioSource[] soundSources = GetComponents<AudioSource>();
        attackSound = soundSources[2];
        deathSound = soundSources[0];
        takeDamageSound = soundSources[1];
    }

    void Update()
    {

        if (EnemyHealth <= 0 || enemyFootCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
        {
            Die();
        }
        if (isSkilled == false)
        {
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

    }

    public void GetDamage(int damage)
    {
        takeDamageSound.Play();
        EnemyHealth -= damage;

    }
    public void GetSkill(int damage)
    {
        spellEffect.Play();
        EnemyHealth -= damage;

    }
    private void Die()
    {
        if (isEnemyAlive == true)
        {
            deathSound.Play();
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

    private void MoveToPlayer()
    {
        enemyAnimator.SetInteger("EnemyState", 1);
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), EnemySpeed * Time.deltaTime);

    }

    private void SearchForPlayer()
    {
        distanceBetweenPlayer = Mathf.Abs(playerObjectTransform.position.x - transform.position.x);
        if (distanceBetweenPlayer <= 1f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2 )
        {
            Attack();
        }
        else if (distanceBetweenPlayer <= 10f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2)
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
        yield return new WaitForSeconds(0.4f);
        if (CanGiveDamage())
        {
            attackSound.Play();
            playerScript.TakeDamage(damage);
        }
        else
        {
            playerScript.dodgeParticle.Play();
            playerScript.dodgeSound.Play();
        }
        yield return new WaitForSeconds(0.7f);
        enemyAnimator.SetInteger("EnemyState", 0);
    }


    private IEnumerator ClearTheEnemy()
    {
        enemyCollider.enabled = false;
        yield return new WaitForSeconds(1);
        isEnemyAlive = true;
        enemyCollider.enabled = true;
        EnemyHealth = 75;
        yield return new WaitForSeconds(0.1f);
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

    }
}
