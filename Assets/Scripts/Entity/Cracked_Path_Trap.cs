using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cracked_Path_Trap : MonoBehaviour
{
    public bool isActive = false;
    public Sprite voidSprite;
    private Collider2D col;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Box")))
        {
            GameObject player = other.gameObject;
            
            if (player != null)
            {
               SetTrap();
            }
        }
    }
    
    private void SetTrap()
    {
        isActive = true;
        GetComponent<SpriteRenderer>().sprite = voidSprite;
        gameObject.layer = LayerMask.NameToLayer("Void");
    }
}
