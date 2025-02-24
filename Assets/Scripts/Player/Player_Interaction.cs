using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player_Interaction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable") && !UI_Inventory.Instance.isInventoryFull())
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.DestroyObject();
            }
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            IEnemy enemy = other.GetComponent< IEnemy>();
            if (enemy != null)
            {
                Player_Movement.Instance.isDead = true;
                StartCoroutine(KillPlayer());
            }
        }
    }

    private IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        
        GetComponent<FOD_Agent>().EndAgent();
       
        yield return new WaitForSeconds(0.7f);
        
        FOD_Manager manager = FindObjectOfType<FOD_Manager>(true);
        
        if (manager != null)
        {
            manager.RemoveAgentsGradually();
        }
       
        gameObject.SetActive(false);
    }
}
