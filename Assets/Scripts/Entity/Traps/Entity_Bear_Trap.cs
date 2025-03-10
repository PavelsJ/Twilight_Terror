using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Bear_Trap : MonoBehaviour
{
    public bool isActive = true;
    private Collider2D col;
    
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.gameObject.CompareTag("Player"))
        {
            Player_Movement player = other.GetComponent<Player_Movement>();
            
            if (player != null)
            {
                player.HitByTrap();
                Player_Movement_Manager.Instance.ActivateSpiderChase();
                DeactivateTrap();
            }
        }
    }
    
    private void DeactivateTrap()
    {
        isActive = false;
        col.enabled = false;
       
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
