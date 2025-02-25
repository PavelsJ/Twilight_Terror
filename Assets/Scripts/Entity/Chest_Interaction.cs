using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Interaction : MonoBehaviour, IInteractable
{
    public GameObject UIprefab;
    public Sprite chestOpenSprite;

    public void DestroyObject()
    {
        UI_Inventory.Instance.AddItem(UIprefab);
        
        GetComponent<SpriteRenderer>().sprite = chestOpenSprite;
        GetComponent<Collider2D>().enabled = false;
    }
}
