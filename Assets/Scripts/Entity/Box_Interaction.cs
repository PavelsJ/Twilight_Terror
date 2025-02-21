using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Interaction : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask voidLayer;
    public LayerMask boxLayer;
    public Transform boxMovePoint;
    
    void Start()
    {
        boxMovePoint.parent = null;
    }
    
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
        transform.position = newPosition;
    }

    private void ToggleBox()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
