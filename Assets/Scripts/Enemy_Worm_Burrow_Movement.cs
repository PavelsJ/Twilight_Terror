using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Worm_Burrow_Movement : MonoBehaviour, IEnemy
{
    public GameObject worm;
    public float wormSpeed = 7;
    
    public Transform firstBurrow;
    public Transform secondBurrow;
    
    private int count;
    private bool movingToSecond = true;
    private Coroutine movementCoroutine;

    private void Start()
    {
        if (worm != null)
        {
            Player_Steps.Instance.RegisterEnemy(this);
            
            worm.SetActive(false);
            worm.transform.position = firstBurrow.position; 
        }
    }
    
    public void OnPlayerMoved()
    {
        count++;
        
        if (count % 2 == 0)
        {
            worm.SetActive(true);
            
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
        while (worm != null && Vector3.Distance(worm.transform.position, target.position) > 0.05f)
        {
            worm.transform.position = Vector3.MoveTowards(
                worm.transform.position, target.position, wormSpeed * Time.deltaTime);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.2f);
        
        worm.SetActive(false);
    }
}
