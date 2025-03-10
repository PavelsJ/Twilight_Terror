using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camera_Manager : MonoBehaviour
{
    public GameObject transitionCamera;
    public GameObject mainCamera;

    private int buildIndex;

    private void Awake()
    {
       buildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("StartCutscene_" + buildIndex))
        {
            transitionCamera.SetActive(false);
            
            Player_Movement.Instance.StartAgent(true);
        }
        else
        {
            transitionCamera.SetActive(true);
            mainCamera.SetActive(false);
            
            Player_Movement.Instance.StartAgent(false);
            StartCoroutine(ChangeCamera());
        }
    }

    private IEnumerator ChangeCamera()
    {
        yield return new WaitForSeconds(2f);
        
        transitionCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        PlayerPrefs.SetInt("StartCutscene_" + buildIndex, 1);
        PlayerPrefs.Save();
    }
}
