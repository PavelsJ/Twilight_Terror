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

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = chestOpenSprite;
        spriteRenderer.color = Color.gray;
        
        GetComponent<Collider2D>().enabled = false;
    }
}
