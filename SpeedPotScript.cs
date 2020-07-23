using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotScript : MonoBehaviour
{
    private PlayerMove playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerScript.movementSpeed <= 8f)
        {
            playerScript.movementSpeed += 2f;
        }
        else
        {
            playerScript.movementSpeed = 10f;
        }
        Destroy(gameObject);
    }
}
