using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    private PlayerMove playerScript;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject UIObject;
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        Time.timeScale = 0;
        UIObject = GameObject.Find("UI");
        UIObject.SetActive(false);
    }

    public void SelectControl1()
    {
        playerScript.selectedControl = 1;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SelectControl2()
    {
        playerScript.selectedControl = 2;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
