using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Interaction : MonoBehaviour
{
    public float speed = 5f; 
    
    public LayerMask groundLayer;
    public LayerMask voidLayer;
    public LayerMask boxLayer;
    
    public bool TryPush(Vector3 direction)
    {
        if (gameObject.activeSelf)
        {
            Vector3 targetPosition = transform.position + direction;
        
            if (Physics2D.OverlapPoint(targetPosition, groundLayer) &&
                !Physics2D.OverlapPoint(targetPosition, boxLayer))
            {
                Move(targetPosition);
                return true;
            }
            else if (Physics2D.OverlapPoint(targetPosition, voidLayer))
            {
                Move(targetPosition);
                ToggleBox();
                return true;
            }
        }
        
        return false;
    }

    private void Move(Vector3 newPosition)
    {
        StartCoroutine(MoveSmoothly(newPosition));
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

    private void ToggleBox()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
