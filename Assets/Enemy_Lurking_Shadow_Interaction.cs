using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lurking_Shadow_Interaction : MonoBehaviour
{
    private Transform player;
    public Transform shadow;
    public bool isNearPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isNearPlayer)
        {
            player = other.transform;
            FOD_Agent agent = player.GetComponent< FOD_Agent>();
            agent.SetMinRadiusValue();

            ActivateShadow();
            isNearPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isNearPlayer)
        {
            FOD_Agent agent = player.GetComponent< FOD_Agent>();
            agent.SetMaxRadiusValue();
            
            isNearPlayer = false;
        }
    }

    private void ActivateShadow()
    {
        if (shadow != null)
        {
            Enemy_Lurking_Shadow_Movement shadowMovement = shadow.GetComponent<Enemy_Lurking_Shadow_Movement>();
            shadowMovement.Activate();
            
            shadow.SetParent(null);
        }
    }
}
