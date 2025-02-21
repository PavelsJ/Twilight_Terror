using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Room_Script : MonoBehaviour
{
    public int nextRoomIndex;
    public Grid_Manager gridInteraction;
    public bool isStart = false;
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isActive)
        {
            if (isStart)
            {
                gridInteraction.OnStart(0);
            }
            else
            {
                gridInteraction.OnActive(nextRoomIndex);
            }
            isActive = true;
        }
    }
}
