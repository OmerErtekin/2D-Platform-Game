using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chest : MonoBehaviour
{
    Animator chestAnimator;
    public GameObject Sword;
    public GameObject HealthPot;
    private int itemNumber = 1;
    void Start()
    {
        itemNumber = Random.Range(1, 3);
        chestAnimator = GetComponent<Animator>();
        chestAnimator.SetBool("isOpened", false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        chestAnimator.SetBool("isOpened", true);
        if (itemNumber == 1) StartCoroutine(ChestDestroy(Sword));
        else if (itemNumber == 2) StartCoroutine(ChestDestroy(HealthPot));


    }

    IEnumerator ChestDestroy(GameObject willInstantiated)
    {
        yield return new WaitForSeconds(2);
        Instantiate(willInstantiated, new Vector2(transform.position.x + 2, transform.position.y), Quaternion.identity);
        Destroy(gameObject);
    }
  
}
