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
        FadeOut();
    }

    private void FadeOut()
    {
        GetComponent<Collider2D>().enabled = false;
        
        anim.SetTrigger("FadeOut");
        
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        agent.deactivateOnEnd = true;
        agent.EndAgent();
    }
}
