using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed_Interaction : MonoBehaviour
{
    public bool endScene = false;
    
    public GameObject endScreen;
    
    private FOD_Manager manager;
    
    private void Start()
    {
        endScreen.SetActive(false);
        manager = FindObjectOfType<FOD_Manager>(true).GetComponent<FOD_Manager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (endScene && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(OnNextScene());
        }
    }
    
    private IEnumerator OnNextScene()
    {
        if (manager != null)
        {
            manager.SetFogVisibility(true);
            yield return new WaitForSeconds(0.8f);
            
            endScreen.SetActive(true);
        }
    }
}
