using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint1 : MonoBehaviour
{
    private PlayerMove playerScript;
    public GameObject EnemysPart1;

    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemysPart1.SetActive(true);
        playerScript.checkPointControl = 1;
        playerScript.setCheckpoint(transform);
    }
}
