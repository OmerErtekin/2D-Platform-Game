using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private PauseButton buttonScript;
   // public GameObject optionsMenu;
    void Start()
    {
        buttonScript = GameObject.Find("PauseButton").GetComponent<PauseButton>();     
    }

    // Update is called once per frame
    public void Resume()
    {
        buttonScript.isPaused = true;
        buttonScript.ButtonState();
    }

    public void Options()
    {
        
    }

    public void BackToMenu()
    {
        buttonScript.isPaused = true;
        buttonScript.ButtonState();
        SceneManager.LoadScene("MainMenu");
    }
}
