using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPot : MonoBehaviour
{
    PlayerMove playerScript;
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerScript.Heal(25);
        Destroy(gameObject);
    }
}
