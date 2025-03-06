using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy_Past_Movement : MonoBehaviour
{
    public Transform previousPart; 
    public float speed = 5f;
   
    private Vector3 previousPosition; 
    private Vector3 targetPosition;
    
    private Copy_Past_Movement previousSegment; 

    private void Start()
    {
        if (previousPart != null)
        {
            previousSegment = previousPart.GetComponent<Copy_Past_Movement>();
        }

        previousPosition = transform.position;
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            
            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
        }
    }
    
    public void UpdateSegmentPosition(Vector3 newPosition)
    {
        previousPosition = transform.position;
        
        targetPosition = newPosition;
        
        if (previousSegment != null)
        {
            previousSegment.UpdateSegmentPosition(previousPosition);
        }
    }
    
    private void RotateSprite(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); 
    }
}
