using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Worm_Movement : MonoBehaviour, IEnemy
{
    public float wormSpeed = 7;
    
    public Transform firstBurrow;
    public Transform secondBurrow;
    
    private int count;
    private bool movingToSecond = true;
    private Coroutine movementCoroutine;
    
    private void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        
        gameObject.SetActive(false);
        transform.position = firstBurrow.position; 
    }
    
    public void OnPlayerMoved()
    {
        count++;
        
        if (count % 2 == 0)
        {
            gameObject.SetActive(true);
            
            Transform targetBurrow = movingToSecond ? secondBurrow : firstBurrow;
            
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            
            movementCoroutine = StartCoroutine(MoveWormTo(targetBurrow));
            
            movingToSecond = !movingToSecond;
        }
    }
    
    private IEnumerator MoveWormTo(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            Vector3 direction = target.position - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
            
            transform.position = Vector3.MoveTowards(
                transform.position, target.position, wormSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        yield return new WaitForSeconds(0.2f);
        
        gameObject.SetActive(false);
    }
    
    private void RotateSprite(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); 
    }

}
