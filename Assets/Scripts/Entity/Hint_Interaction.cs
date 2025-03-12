using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint_Interaction : MonoBehaviour
{
    public Sprite hint;
    public string hintText;
    
    public UI_Hints hints;
    
    private bool played = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !played)
        {
            played = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
            hints.ShowHint(hint, hintText);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && played)
        {
            hints.HideHint();
        }
    }
}


