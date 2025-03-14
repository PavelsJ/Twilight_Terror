using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Interaction : MonoBehaviour
{
    private Coroutine coroutine;
    
    public float speed = 5f; 
    
    public LayerMask wallLayer;
    public LayerMask voidLayer;
    public LayerMask boxLayer;
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    public bool TryPush(Vector3 direction)
    {
        if (gameObject.activeSelf)
        {
            Vector3 targetPosition = transform.position + direction;
        
            if (!Physics2D.OverlapPoint(targetPosition, wallLayer) &&
                !Physics2D.OverlapPoint(targetPosition, boxLayer))
            {
                Move(targetPosition);
                return true;
            }
            else if (Physics2D.OverlapPoint(targetPosition, voidLayer))
            {
                Move(targetPosition);
                StartCoroutine(ToggleBox());
                return true;
            }
        }
        
        return false;
    }

    private void Move(Vector3 newPosition)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(MoveSmoothly(newPosition));
    }

    private IEnumerator MoveSmoothly(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; 
    }

    private IEnumerator ToggleBox()
    {
        GetComponent<Collider2D>().enabled = false;
        
        animator.SetTrigger("FadeOut");
        
        yield return new WaitUntil(() => 
            animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        
        gameObject.SetActive(false);
    }
}
