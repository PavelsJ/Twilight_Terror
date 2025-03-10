using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Interaction : MonoBehaviour
{
    public float invulnerabilityDistance = 2f;
    
    private bool isActive = false;
    private Transform player;
    
    private FOD_Agent agent;

    private void Awake()
    {
        agent = GetComponent<FOD_Agent>();

        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            player = other.transform;
            ActivateCheckpoint();
        }
    }

    private void Update()
    {
        if (isActive && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Player_Movement_Manager.Instance.SetInvulnerability(distanceToPlayer <= invulnerabilityDistance);
        }
    }

    private void ActivateCheckpoint()
    {
        isActive = true;
        if (agent != null)
        {
            agent.enabled = true;
        }
        Debug.Log("Checkpoint Activated!");
    }
}
