using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint2 : MonoBehaviour
{
    private CapsuleCollider2D CheckPoint1Collider;
    private PlayerMove playerScript;
    public GameObject EnemysPart2;

    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerScript.checkPointControl = 2;
        EnemysPart2.SetActive(true);
        playerScript.setCheckpoint(transform);
    }
}
