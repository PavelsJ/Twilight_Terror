using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hints : MonoBehaviour
{
    public Image hint;
    public TextMeshProUGUI hintText;
    
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void ShowHint(Sprite newSprite, string newText)
    {
        gameObject.SetActive(true);
        hint.sprite = newSprite;
        hintText.text = newText;
        
        animator.SetTrigger("ShowHint");
    }

    public void HideHint()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(HideHintCoroutine());
        }
    }

    private IEnumerator HideHintCoroutine()
    {
        animator.SetTrigger("HideHint");
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            
        hint.sprite = null;
        gameObject.SetActive(false);
    }
}
