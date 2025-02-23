using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Interaction : MonoBehaviour
{
    public float distance = 1;
    private bool isActive = false;
    
    private FOD_Agent agent;

    private void Awake()
    {
        agent = GetComponent<FOD_Agent>();

        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    void LateUpdate()
    {
        
    }

    private void ActivateCheckpoint()
    {
        if (agent != null)
        {
            agent.enabled = true;
        }
    }
}
