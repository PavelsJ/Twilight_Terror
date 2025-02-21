using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Light_Bulb : MonoBehaviour, IInteractable
{
    public GameObject UIprefab;
    
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("FadeIn");
    }

    public void DestroyObject()
    {
        UI_Inventory.Instance.AddItem(UIprefab);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        anim.SetTrigger("FadeOut");
        
        GetComponent<Collider2D>().enabled = false;
        GetComponent<FOD_Agent>().EndAgent();
        
        yield return new WaitForSeconds(2);
        
        gameObject.SetActive(false);
    }
}
