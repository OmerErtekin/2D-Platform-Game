using System.Collections;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
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
     * Dizy : 8
     * Skill 1 : 9
     * Won : 10
     */
    public int selectedControl;
    public int checkPointControl = 0;
    public int PlayerHealth ;
    public int PlayerMaxHealth = 200;
    private int damage = 25;
    private int skillDamage = 25;
    private int enemyCounter;
    private float jumpSpeed = 625f;
    public float movementSpeed = 7f;
    private float attackRange = 0.7f;
    private float skillRange = 5f;
    private float skillCoolDown = 10f;
    private float lastSkillTime = 0;
    private float attackCoolDown = 0.6f;
    private float lastAttackTime = 0;
    private float lastCrouchTime = 0;
    private float crouchCoolDown = 2f;
    private float lastJumpTime = 0;
    private float jumpCoolDown = 0.5f;
    private float moveInputHorizontal;
    private float moveInputVertical;
    private LayerMask LowEnemy;
    private LayerMask EnemyLayer;
    private LayerMask MageLayer;
    private LayerMask BossLayer;
    private LayerMask RogueLayer;
    public Image crouchCoolDownImage;
    public Image skillCoolDownImage;
    public GameObject EnemysPart0;
    public GameObject EnemysPart1;
    public GameObject EnemysPart2;
    public GameObject EnemysPart3;
    public GameObject controlSet1;
    public GameObject controlSet2;
    public GameObject playerObject;
    public ParticleSystem hitEffect;
    public ParticleSystem dizzyEffect;
    public ParticleSystem dodgeParticle;
    public ParticleSystem skillParticle;
    private Rigidbody2D playerRB;
    private Animator PlayerAnimator;
    private BoxCollider2D footCollider;
    private Transform respawnTransform;
    public HealthBar healthBar;
    public Joystick joystick1;
    public Joystick joystick2;
    public Transform attackPosition;
    public Transform skillPosition;
    private BossScript bossScript;
    private bool isAtacking = false;
    public bool isDodging = false ;
    public bool isCrouching;
    private bool isCrouchingCoolDown = false;
    private bool isSkillCoolDown = false;
    private bool isUsedSkill = false;
    public bool isTimeToGo = false;
    public bool isFacingRight = true;
    public bool isAlive = true;
    public bool isDizzy = false;
    public bool canMove = true;
    public bool isWon = false;
    private bool isSkilling = false;
    public bool canGetSpeed = false;
    private AudioSource attackSound;
    public AudioSource dodgeSound;
    private AudioSource jumpSound;
    private AudioSource crouchSound;
    private AudioSource takeDamageSound;
    private AudioSource dieSound;
    private AudioSource shieldSound;
    private AudioSource gainDamageSound;
    private AudioSource skillSound;



    void Start()
    {
        LowEnemy = LayerMask.GetMask("LowEnemy");
        BossLayer = LayerMask.GetMask("Boss");
        EnemyLayer = LayerMask.GetMask("Enemy");
        MageLayer = LayerMask.GetMask("MageEnemy");
        RogueLayer = LayerMask.GetMask("RogueEnemy");
        PlayerAnimator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        footCollider = GetComponent<BoxCollider2D>();
        EnemysPart0 = GameObject.Find("EnemysPart0");
        playerObject = GameObject.Find("Player");
        bossScript = GameObject.Find("Boss").GetComponent<BossScript>();
        respawnTransform = GameObject.Find("CheckPoint0").GetComponent<Transform>();
        AudioSource[] soundSources = GetComponents<AudioSource>();
        skillSound = soundSources[0];
        jumpSound = soundSources[1];
        dodgeSound = soundSources[2];
        attackSound = soundSources[3];
        takeDamageSound = soundSources[4];
        dieSound = soundSources[5];
        crouchSound = soundSources[8];
        shieldSound = soundSources[6];
        gainDamageSound = soundSources[7];
        PlayerHealth = PlayerMaxHealth;
        if(selectedControl == 2)
        {
            crouchCoolDownImage.fillAmount = 0;
        }
        skillCoolDownImage.fillAmount = 0;

    }

    void Update()
    {
        //instantiate bool variables for move, if not the character couldn't move until the attack.

        healthBar.SetHealth(PlayerHealth);

        if (selectedControl == 1)
        {

            moveInputHorizontal = joystick2.Horizontal;
            moveInputVertical = joystick2.Vertical;
            controlSet1.SetActive(false);
            controlSet2.SetActive(true);
            CrouchCoolDownCounter();
            SkillCoolDownCounter();

            if (isAlive == true)
            {
                if (canMove == true && isDizzy == false)

                {

                    if (PlayerHealth <= 0)
                    {
                        healthBar.SetHealth(0);
                        Die();
                    }
                    if (footCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
                    {
                        Die();
                        PlayerHealth = PlayerMaxHealth;
                    }
                    if (CrossPlatformInputManager.GetButton("Attack") && Time.time > lastAttackTime && isDodging == false)
                    {
                        lastAttackTime = Time.time + attackCoolDown;
                        StartCoroutine(ActiveAttack());
                        Attack();

                    }

                    if (CrossPlatformInputManager.GetButton("Skill") && Time.time > lastSkillTime && isDodging == false)
                    {
                        lastSkillTime = Time.time + skillCoolDown;
                        Skill();
                    }

                    if (isAtacking == false)
                    {
                        Dodge();
                    }
                    isItTimeToGo();

                    //player cant move when he is attacking or dashing, however he can break the movement statement by doing attack or dash.

                    if (isAtacking == false && isDodging == false)
                    {
                        if (CrossPlatformInputManager.GetButton("Jump") && Time.time > lastJumpTime)
                        {
                            lastJumpTime = Time.time + jumpCoolDown;
                            Jump();
                        }
                        if (CrossPlatformInputManager.GetButton("Crouch") && Time.time > lastCrouchTime && isCrouchingCoolDown == false)
                        {
                            lastCrouchTime = Time.time + crouchCoolDown;
                            StartCoroutine(ActiveCrouch());
                        }
                        Run();
                    }

                }

            }
           
        }
        else if(selectedControl == 2)
        {
            moveInputHorizontal = joystick1.Horizontal;
            moveInputVertical = joystick1.Vertical;
            controlSet1.SetActive(true);
            controlSet2.SetActive(false);
            SkillCoolDownCounter();

            if (isAlive == true)
            {
                if (canMove == true && isDizzy == false)

                {

                    if (PlayerHealth <= 0)
                    {
                        healthBar.SetHealth(0);
                        Die();
                    }

                    if (footCollider.IsTouchingLayers(LayerMask.GetMask("Spike")))
                    {
                        Die();
                        PlayerHealth = PlayerMaxHealth;
                    }

                    if (CrossPlatformInputManager.GetButton("Attack") && Time.time > lastAttackTime && isDodging == false && isCrouching == false)
                    {
                        lastAttackTime = Time.time + attackCoolDown;
                        StartCoroutine(ActiveAttack());
                        Attack();

                    }
                    if (CrossPlatformInputManager.GetButton("Skill") && Time.time > lastSkillTime && isDodging == false && isCrouching == false)
                    {
                        lastSkillTime = Time.time + skillCoolDown;
                        Skill();
                    }
                    if (isAtacking == false)
                    {
                        Dodge();
                    }
                    isItTimeToGo();

                    //player cant move when he is attacking or dashing, however he can break the movement statement by doing attack or dash.

                    if (isAtacking == false && isDodging == false)
                    {
                        if (moveInputVertical > 0.35f && Time.time > lastJumpTime)
                        {
                            lastJumpTime = Time.time + jumpCoolDown;
                            Jump();
                        }
                        if (moveInputVertical < -0.35f && Time.time > lastCrouchTime)
                        {
                            lastCrouchTime = Time.time + crouchCoolDown;
                            StartCoroutine(ActiveCrouch());
                        }
                        Run();
                    }

                }

            }

        }
            
    }
    private void FixedUpdate()
    {
        healthBar.SetHealth(PlayerHealth);
        if (isWon == true)
        {
            canMove = false;
            PlayerAnimator.SetInteger("State", 9);
        }
        else {
                if (isCrouching == false && isAlive == true && isAtacking == false && canMove == true && isSkilling == false)
                {
                    PlayerAnimator.SetInteger("State", 0);
                }
                else if (canMove == false && isDizzy == true)
                {
                    PlayerAnimator.SetInteger("State", 8);
                }
            }
        
    }
    private void CrouchCoolDownCounter()
    // for got this check out https://www.youtube.com/watch?v=wtrkrsJfz_4
    {
        if (isCrouchingCoolDown == false && isCrouching == true)
        {
            isCrouchingCoolDown = true;
            crouchCoolDownImage.fillAmount = 1;
        }
        if (isCrouchingCoolDown == true)
        {
            crouchCoolDownImage.fillAmount -= 1 / crouchCoolDown * Time.deltaTime;
            if (crouchCoolDownImage.fillAmount <= 0)
            {
                crouchCoolDownImage.fillAmount = 0;
                isCrouchingCoolDown = false;
            }
        }
    }

    private void SkillCoolDownCounter()
    {
        if(isSkillCoolDown == false && isUsedSkill == true)
        {
            isSkillCoolDown = true;
            skillCoolDownImage.fillAmount = 1;
        }

        if(isSkillCoolDown == true)
        {
            skillCoolDownImage.fillAmount -= 1 / skillCoolDown * Time.deltaTime;
        }

        if(skillCoolDownImage.fillAmount <= 0)
        {
            skillCoolDownImage.fillAmount = 0;
            isSkillCoolDown = false;
        }
    }
    public void GainDamage(int extra)
    {
        damage += extra;
        gainDamageSound.Play();
    }
    public void Heal(int heal)
    {
        PlayerHealth += heal;
        shieldSound.Play();
        if (PlayerHealth >= 200)
        {
            PlayerHealth = 200;
        }

    }

    void Skill()
    {
        StartCoroutine(ActiveSkill());
    }

    void Run()
    {
        if (moveInputHorizontal > 0.1f)
        {
            playerRB.velocity = new Vector2(movementSpeed, playerRB.velocity.y);
        }
        else if(moveInputHorizontal < -0.1f)
        {
            playerRB.velocity = new Vector2(-movementSpeed, playerRB.velocity.y);
        }
        
        if(Mathf.Abs(playerRB.velocity.x) >= 0.01 && isCrouching == false && isAlive == true && isAtacking == false)
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
            if (!footCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !footCollider.IsTouchingLayers(LayerMask.GetMask("LowEnemy"))|| footCollider.IsTouchingLayers(LayerMask.GetMask("Train"))) return;
            playerRB.AddForce(new Vector2(0f, jumpSpeed));
            PlayerAnimator.SetInteger("State", 3);
            jumpSound.Play();
        
    }

    void Dodge()
    {
        if (CrossPlatformInputManager.GetButton("Dodge") && footCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            isDodging = true;
            PlayerAnimator.SetInteger("State", 4);
        }
        else
        {
            isDodging = false;
        }
    }
    void Attack()
    {
            attackSound.Play();
            StartCoroutine(AttackWithDelay());      
    }

    void FlipCharacter()
    {
        // for understand that check  https://www.youtube.com/watch?v=Xnyb2f6Qqzg&feature=youtu.be&t=38m39s 
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    public void TakeSpell()
    {
        StartCoroutine(Spell());
    }

    private IEnumerator Spell()
    {
        isDizzy = true;
        canMove = false;
        dizzyEffect.Play();
        yield return new WaitForSeconds(1.5f);
        isDizzy = false;
        canMove = true;
    }
    private void OnDrawGizmosSelected() // draw the radius of attack and skill
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(skillPosition.position, skillRange*3);
    }




    public void TakeDamage(int damage)
    {
        takeDamageSound.Play();
        PlayerHealth -= damage;
        if (PlayerHealth < 0) PlayerHealth = 0;
        healthBar.SetHealth(PlayerHealth);
        if (isAtacking == false && isDizzy == false)
        {
            PlayerAnimator.SetInteger("State", 6);
        }
    }

    private void Die()
    {
        PlayerAnimator.SetInteger("State", 5);
        dieSound.Play();
        isAlive = false;
        StartCoroutine(ClearPlayer());
    }

    private void isItTimeToGo()
    {
        if (footCollider.IsTouchingLayers(LayerMask.GetMask("Train")) && bossScript.isEnemyAlive == false)
        {
            isTimeToGo = true;
            isWon = true;
        }
    }
    

    IEnumerator ClearPlayer()
    {
        yield return new WaitForSeconds(2);
        PlayerHealth = PlayerMaxHealth;
        isAlive = true;
        playerObject.transform.position = respawnTransform.position;
        healthBar.SetHealth(PlayerHealth);
        SetEnemiesBack();

    }

    IEnumerator ActiveCrouch()
    {
        isCrouching = true;
        PlayerAnimator.SetInteger("State", 7);
        crouchSound.Play();
        yield return new WaitForSeconds(1);
        isCrouching = false;
    }
    IEnumerator ActiveAttack()
    {
        isAtacking = true;
        PlayerAnimator.SetInteger("State", 1);
        yield return new WaitForSeconds(0.42f);
        isAtacking = false;
    }
    
    IEnumerator ActiveSkill()
    {
        isUsedSkill = true; // for start the cool down
        isSkilling = true; // for control the animation
        skillParticle.Play();
        yield return new WaitForSeconds(0.15f);
        PlayerAnimator.SetInteger("State", 9);
        yield return new WaitForSeconds(0.1f);
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(skillPosition.position, skillRange, EnemyLayer);
        Collider2D[] magesToDamage = Physics2D.OverlapCircleAll(skillPosition.position, skillRange, MageLayer);
        Collider2D[] bossToDamage = Physics2D.OverlapCircleAll(skillPosition.position, skillRange, BossLayer);
        Collider2D[] lowEnemiesToDamage = Physics2D.OverlapCircleAll(skillPosition.position, skillRange, LowEnemy);
        Collider2D[] roguesToDamage = Physics2D.OverlapCircleAll(skillPosition.position, skillRange, RogueLayer);
        enemyCounter = enemiesToDamage.Length + magesToDamage.Length + bossToDamage.Length + lowEnemiesToDamage.Length + roguesToDamage.Length;
        if (enemyCounter != 0)
        {
            skillSound.Play();
        }
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().GetSkill(skillDamage);
            enemiesToDamage[i].GetComponent<Enemy>().isSkilled = true;
        }
        for (int j = 0; j < magesToDamage.Length; j++)
        {
            magesToDamage[j].GetComponent<EnemyMage>().GetSkill(skillDamage);
            magesToDamage[j].GetComponent<EnemyMage>().isSkilled = true;
        }
        for (int k = 0; k < bossToDamage.Length; k++)
        {
            bossToDamage[k].GetComponent<BossScript>().GetSkill(skillDamage);
            bossToDamage[k].GetComponent<BossScript>().isSkilled = true;
        }
        for (int l = 0; l < lowEnemiesToDamage.Length; l++)
        {
            lowEnemiesToDamage[l].GetComponent<LowSkelton>().GetSkill(skillDamage);
            lowEnemiesToDamage[l].GetComponent<LowSkelton>().isSkilled = true;
        }
        for (int m = 0; m < roguesToDamage.Length; m++)
        {
            roguesToDamage[m].GetComponent<RogueScript>().GetSkill(skillDamage);
            roguesToDamage[m].GetComponent<RogueScript>().isSkilled = true;
        }


        yield return new WaitForSeconds(0.4f);
        PlayerAnimator.SetInteger("State", 0);
        isSkilling = false;
        yield return new WaitForSeconds(1);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().isSkilled = false;
        }
        for (int j = 0; j < magesToDamage.Length; j++)
        {
            magesToDamage[j].GetComponent<EnemyMage>().isSkilled = false;
        }
        for (int k = 0; k < bossToDamage.Length; k++)
        {
            bossToDamage[k].GetComponent<BossScript>().isSkilled = false;
        }
        for (int l = 0; l < lowEnemiesToDamage.Length; l++)
        {
            lowEnemiesToDamage[l].GetComponent<LowSkelton>().isSkilled = false;
        }
        for (int m = 0; m < roguesToDamage.Length; m++)
        {
            roguesToDamage[m].GetComponent<RogueScript>().isSkilled = false;
        }

        yield return new WaitForSeconds(skillCoolDown-1.69f);
        isUsedSkill = false;
    }

    IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, EnemyLayer);
        Collider2D[] magesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, MageLayer);
        Collider2D[] bossToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, BossLayer);
        Collider2D[] lowEnemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, LowEnemy);
        Collider2D[] roguesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, RogueLayer);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().GetDamage(damage);
            hitEffect.Play();
        }
        for (int j = 0; j < magesToDamage.Length; j++)
        {
            magesToDamage[j].GetComponent<EnemyMage>().GetDamage(damage * 2);
            hitEffect.Play();
        }
        for (int k = 0; k < bossToDamage.Length; k++)
        {
            hitEffect.Play();
            bossToDamage[k].GetComponent<BossScript>().GetDamage(damage);
        }
        for (int l = 0; l < lowEnemiesToDamage.Length; l++)
        {
            hitEffect.Play();
            lowEnemiesToDamage[l].GetComponent<LowSkelton>().GetDamage(damage * 2);
        }
        for (int m = 0; m < roguesToDamage.Length; m++)
        {
            hitEffect.Play();
            roguesToDamage[m].GetComponent<RogueScript>().GetDamage(damage);
        }
    }

    public void setCheckpoint(Transform point)
    {
        respawnTransform = point;

    }
    
    public void SetEnemiesBack()
    {
        // for reborn the enemies in die situation, check the checkpoint status
        int i = 0;
        if (checkPointControl == 0)
        {
            for (i = 0; i < EnemysPart0.transform.childCount; i++)
            {
                EnemysPart0.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (checkPointControl == 1)
        {
            for (i = 0; i < EnemysPart1.transform.childCount; i++)
            {
                EnemysPart1.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (checkPointControl == 2)
        {
            for (i = 0; i < EnemysPart2.transform.childCount; i++)
            {
                EnemysPart2.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (checkPointControl == 3)
        {
            for (i = 0; i < EnemysPart3.transform.childCount; i++)
            {
                EnemysPart3.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
