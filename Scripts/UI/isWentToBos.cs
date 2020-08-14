using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isWentToBos : MonoBehaviour
{
    private CapsuleCollider2D passageCollider;
    private AudioSource passageSound;
    private AudioSource whereAmISound;
    private PlayerMove playerScript;
    
    void Start()
    {
        passageCollider = GetComponent<CapsuleCollider2D>();
        AudioSource[] sources = GetComponents<AudioSource>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        passageSound = sources[0];
        whereAmISound = sources[1];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(playSounds());
        }
    }


    IEnumerator playSounds()
    {
        playerScript.canMove = false;
        playerScript.isDizzy = true;
        yield return new WaitForSeconds(1.5f);
        whereAmISound.Play();
        yield return new WaitForSecondsRealtime(17f);
        passageSound.Play();
        playerScript.isDizzy = false;
        playerScript.canMove = true;

    }
}
