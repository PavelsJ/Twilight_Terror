using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Interaction : MonoBehaviour
{
    public float activationDistance = 1f;
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

    private void Start()
    {
        player = FindObjectOfType<Player_Movement>().transform;
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            if (!isActive)
            {
                if (distanceToPlayer <= activationDistance)
                {
                    ActivateCheckpoint();
                }
            }
            else
            {
                if (distanceToPlayer <= invulnerabilityDistance)
                {
                    Player_Steps.Instance.SetInvulnerability(true);
                }
                else
                {
                    Player_Steps.Instance.SetInvulnerability(false);
                }
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
