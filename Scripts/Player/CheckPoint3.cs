using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint3 : MonoBehaviour
{
    private CapsuleCollider2D CheckPoint1Collider;
    private PlayerMove playerScript;
    public GameObject EnemysPart3;

    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerScript.checkPointControl = 3;
        EnemysPart3.SetActive(true);
        playerScript.setCheckpoint(transform);
    }
}
