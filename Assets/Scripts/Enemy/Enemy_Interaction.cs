using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Interaction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.DestroyObject();
            }
        }
    }
}
