using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private PlayerMove playersSript;
    void Start()
    {
        playersSript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playersSript.isCrouching == false)
        {
            playersSript.TakeDamage(500);
        }
    }
}

