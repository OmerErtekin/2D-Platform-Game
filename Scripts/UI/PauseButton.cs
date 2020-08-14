using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject UIElements;
    void Start()
    {
        pauseMenu.SetActive(false);
    }

   

    public void ButtonState()
    {
        if(isPaused == false)
        {
            Time.timeScale = 0f;
            UIElements.SetActive(false);
            pauseMenu.SetActive(true);
            isPaused = true;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;
            UIElements.SetActive(true);
            pauseMenu.SetActive(false);
        }
    }
}
