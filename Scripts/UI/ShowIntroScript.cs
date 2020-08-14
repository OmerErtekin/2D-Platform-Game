using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ShowIntroScript : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(Wait());
        
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
