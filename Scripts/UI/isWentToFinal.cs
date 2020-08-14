using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isWentToFinal : MonoBehaviour
{
    private CapsuleCollider2D finalCollider;
    private BossScript bossScript;
    private FinalBrother brotherScript;
    private PlayerMove playerScript;
    public ParticleSystem firework1;
    public ParticleSystem firework2;
    public ParticleSystem firework3;


    void Start()
    {
        finalCollider = GetComponent<CapsuleCollider2D>();
        bossScript = GameObject.Find("Boss").GetComponent<BossScript>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        brotherScript = GameObject.Find("FinalBrother").GetComponent<FinalBrother>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bossScript.isEnemyAlive == false)
        {
            firework1.gameObject.SetActive(true);
            firework2.gameObject.SetActive(true);
            firework3.gameObject.SetActive(true);
            playerScript.isWon = true;
            brotherScript.isItTimeToGo = true;
        }
    }
}
