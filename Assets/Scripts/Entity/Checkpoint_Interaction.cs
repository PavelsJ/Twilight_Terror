using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Interaction : MonoBehaviour
{
    public float activationDistance = 1f;
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

    private void Start()
    {
        player = FindObjectOfType<Player_Movement>().transform;
    }

    private void Update()
    {
        if (!isActive && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= activationDistance)
            {
                ActivateCheckpoint();
            }
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
