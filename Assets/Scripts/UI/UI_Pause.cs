using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    public GameObject pause;
    
    void Start()
    {
        pause.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        pause.SetActive(!pause.activeSelf);

        if (pause.activeSelf)
        {
            FreezeGame();
        }
        else
        {
            UnfreezeGame();
        }
    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    private void UnfreezeGame()
    {
        Time.timeScale = 1f;
    }
}
