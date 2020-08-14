using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private Rigidbody2D bladeRB;
    private PlayerMove playerScript;
    private SpriteRenderer bladeRenderer;
    private bool canGiveDamage = true;

    
    void Start()
    {
        bladeRB = GetComponent<Rigidbody2D>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        bladeRenderer = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        transform.Rotate(0, 0, Time.time * -50);
        StartCoroutine(GoToPlayer());
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canGiveDamage == true)
        {
                StartCoroutine(giveDamage());
            
        }
    }
    IEnumerator GoToPlayer()
    {
        bladeRB.velocity = new Vector2(-5, 0);
        yield return new WaitForSeconds(3f);
        bladeRenderer.enabled = false;
        transform.position = new Vector3(transform.position.x +15, transform.position.y, transform.position.z);
        bladeRenderer.enabled = true;
        this.gameObject.SetActive(false);
    }

    IEnumerator giveDamage()
    {
        canGiveDamage = false;
        playerScript.TakeDamage(50);
        yield return new WaitForSeconds(1f);
        canGiveDamage = true;
    }
}
