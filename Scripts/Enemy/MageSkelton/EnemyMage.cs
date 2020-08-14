using System.Collections;
using UnityEngine;

public class EnemyMage : MonoBehaviour
{

    /* Animator States:
     * Idle : 0
     * Run : 1
     * Attack 2
     * Die 3
     */
    private float distanceBetweenPlayer;
    private float EnemySpeed = 3f;
    private int EnemyHealth = 100;
    private float enemyNextAttack;
    private float enemyNextSpell;
    private float enemyAtackRate = 1.5f;
    private float enemySpellRate = 6f;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private BoxCollider2D enemyFootCollider;
    private ParticleSystem spellEffect;
    public GameObject FireBallObject;
    private AudioSource attackSound;
    private AudioSource deathSound;
    private AudioSource takeDamageSound;
    private bool isEnemyAlive;
    private bool isFacingRight = false;
    public bool isSkilled = false;

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
        attackSound = soundSources[0];
        deathSound = soundSources[2];
        takeDamageSound = soundSources[1];
    }
        
    void Update()
    {
        if (isEnemyAlive == true)
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
        enemyCollider.enabled = false;
        StartCoroutine(ClearTheEnemy());
    }

    private void Attack()
    {
        if(Time.time > enemyNextSpell)
        {
            enemyNextSpell = Time.time + enemySpellRate;
            enemyNextAttack = Time.time + enemyAtackRate;
            playerScript.TakeSpell();
        }
        else if (Time.time > enemyNextAttack)
        {
            attackSound.Play();
            enemyNextAttack = Time.time + enemyAtackRate;
            Instantiate(FireBallObject, transform.position, Quaternion.identity);
            enemyAnimator.SetInteger("EnemyState", 2);
        }
    }



    private IEnumerator ClearTheEnemy()
    {
        enemyCollider.enabled = false;
        yield return new WaitForSeconds(1);
        isEnemyAlive = true;
        EnemyHealth = 100;
        enemyCollider.enabled = true;
        yield return new WaitForSeconds(0.01f);
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
    }

    private void MoveToPlayer()
    {
        enemyAnimator.SetInteger("EnemyState", 1);
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerObjectTransform.position.x, transform.position.y), EnemySpeed * Time.deltaTime);

    }

    private void SearchForPlayer()
    {
        distanceBetweenPlayer = Mathf.Abs(playerObjectTransform.position.x - transform.position.x);
        if (distanceBetweenPlayer <= 7f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 2)
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

}
