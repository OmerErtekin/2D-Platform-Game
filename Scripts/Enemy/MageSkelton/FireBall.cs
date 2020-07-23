using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private float speed = 10f;
    private Transform PlayerTransform;
    private Vector2 target;
    private PlayerMove playerScript;
    private int damage = 5;
    private bool isFacingRight = false;
 
    void Start()
    {
 
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        PlayerTransform = GameObject.Find("Player").transform;
        target = new Vector2(PlayerTransform.position.x, transform.position.y);
    }


    void Update()
    {
        SpriteDirectionControl();
        transform.position = Vector2.MoveTowards(transform.position,target, speed * Time.deltaTime);
        StartCoroutine(DestroyFireBall());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerScript.isCrouching == false)
        {
            playerScript.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    void FlipSprite()
    {
        // for understand that check  https://www.youtube.com/watch?v=Xnyb2f6Qqzg&feature=youtu.be&t=38m39s 
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void SpriteDirectionControl()
    {
        if (PlayerTransform.position.x - transform.position.x < 0 && isFacingRight == false ) FlipSprite();
        else if (PlayerTransform.position.x - transform.position.x > 0 && isFacingRight == true) FlipSprite();
    }

    IEnumerator DestroyFireBall()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
