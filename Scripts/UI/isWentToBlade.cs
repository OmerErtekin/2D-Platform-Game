using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isWentToBlade : MonoBehaviour
{
    public GameObject blade1;
    public GameObject blade2;
    public GameObject blade3;
    public GameObject blade4;


    private void Start()
    {
        blade1.gameObject.SetActive(false);
        blade2.gameObject.SetActive(false);
        blade3.gameObject.SetActive(false);
        blade4.gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(BladeRain());
        }
    }

    IEnumerator BladeRain()
    {
        blade1.SetActive(true);
        yield return new WaitForSeconds(1f);
        blade2.SetActive(true);
        yield return new WaitForSeconds(1f);
        blade3.SetActive(true);
        yield return new WaitForSeconds(1f);
        blade4.SetActive(true);
        yield return new WaitForSeconds(1f);
    }
}
