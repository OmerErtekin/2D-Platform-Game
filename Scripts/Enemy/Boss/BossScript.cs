using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private int enemyHealth ;
    private int enemyMaxHealth = 500;
    private int damage = 25;
    private float enemyNextAttack;
    private float enemyAtackRate = 0.75f;
    private float healthPercent;
    private Animator enemyAnimator;
    private Transform playerObjectTransform;
    private PlayerMove playerScript;
    private CapsuleCollider2D enemyCollider;
    private BoxCollider2D teleportCollider;
    private SpriteRenderer bossRenderer;
    public ParticleSystem disappearEffect;
    public ParticleSystem spellEffect;
    private AudioSource bossLaugh;
    public Image healthFill;
    public GameObject barObject;
    public bool isEnemyAlive = true;
    public bool isSkilled = false;
    private bool isFacingRight = false;
    private bool isAtacking = false;
    private bool isTeleported = false;
    




    void Start()
    {
        playerObjectTransform = GameObject.FindWithTag("Player").transform;
        enemyAnimator = GetComponent<Animator>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        enemyCollider = GetComponent<CapsuleCollider2D>();
        teleportCollider = GetComponent<BoxCollider2D>();
        bossRenderer = GetComponent<SpriteRenderer>();
        bossLaugh = GetComponent<AudioSource>();
        enemyHealth = enemyMaxHealth;

    }

    void Update()
    {
        if (isEnemyAlive == true)
        {
            if (enemyHealth <= 0)
            {
                Die();
            }
                if (enemyHealth <= enemyMaxHealth / 2 && isSkilled == false)
                {
                    StealthBoss();
                }

                if (playerScript.isAlive && isAtacking == false && isSkilled == false)
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
            if (playerScript.isAlive == false)
            {
                bossRenderer.enabled = true;
                enemyHealth = enemyMaxHealth;
                healthFill.fillAmount = 1;

            }
            if (isEnemyAlive == false)
            {
                enemyAnimator.SetInteger("EnemyState", 3);
                barObject.SetActive(false);
            }
        

    }

    public void GetDamage(int damage) 
    {
        enemyHealth -= damage;
        healthPercent = (float)enemyHealth / enemyMaxHealth;
        healthFill.fillAmount = healthPercent;
       

    }

    public void GetSkill(int damage)
    {
        enemyHealth -= damage;
        healthPercent = (float)enemyHealth / enemyMaxHealth;
        healthFill.fillAmount = healthPercent;
        spellEffect.Play();
    }
    private void Die()
    {
        isEnemyAlive = false;
        enemyAnimator.SetInteger("EnemyState", 3);

        StartCoroutine(ClearTheEnemy());
    }

    private void Attack()
    {
        if (Time.time > enemyNextAttack)
        {
            StartCoroutine(SetAttackActive());
            enemyNextAttack = Time.time + enemyAtackRate;

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
    }

    private IEnumerator ClearTheEnemy()
    {
        enemyCollider.enabled = false;
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
        if (distanceBetweenPlayer <= 1f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 5 )
        {
            StartCoroutine(SetAttackActive());
            Attack();
        }
        else if (distanceBetweenPlayer <= 10f && Mathf.Abs(playerObjectTransform.position.y - transform.position.y) < 1)
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
        enemyAnimator.SetInteger("EnemyState", 2);
        yield return new WaitForSeconds(0.5f);
        isAtacking = false;
        enemyAnimator.SetInteger("EnemyState", 0);
    }

    void StealthBoss()
    {
        if (isAtacking == false)
        {
            if (isTeleported == false)
            {
                disappearEffect.Play();
                bossLaugh.Play();
                StartCoroutine(Teleport());
            }
            barObject.SetActive(false);
            bossRenderer.enabled = false;
        }
        else
        {
            barObject.SetActive(true);
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
        if (!teleportCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            transform.position = new Vector2(transform.position.x + Random.Range(-5, 5), transform.position.y);
        }
        yield return new WaitForSeconds(3);
        isTeleported = false;
    }


}
