using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Worm_Burrow : MonoBehaviour, IEnemy
{
    public GameObject worm;
    public float wormSpeed = 5;
    public float riseHeight = 0.5f;
    
    private int count;
    private Coroutine movementCoroutine;
    private Vector3 initialPosition; 

    private void Start()
    {
        if (worm != null)
        {
            Player_Movement_Manager.Instance.RegisterEnemy(this);
            
            worm.SetActive(false);
            
            initialPosition = transform.position;
            worm.transform.position = initialPosition;
        }
    }
    
    public void OnPlayerMoved()
    {
        count++;
        
        if (count % 4 == 1)
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            movementCoroutine = StartCoroutine(RaiseWorm());
        }
        else if (count % 4 == 3)
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            movementCoroutine = StartCoroutine(LowerWorm());
        }
    }
    
    private IEnumerator RaiseWorm()
    {
        worm.SetActive(true);
        Vector3 targetPosition = initialPosition + Vector3.up * riseHeight;

        while (Vector3.Distance(worm.transform.position, targetPosition) > 0.05f)
        {
            worm.transform.position = Vector3.MoveTowards(worm.transform.position, targetPosition, wormSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator LowerWorm()
    {
        Vector3 targetPosition = initialPosition;

        while (Vector3.Distance(worm.transform.position, targetPosition) > 0.05f)
        {
            worm.transform.position = Vector3.MoveTowards(worm.transform.position, targetPosition, 5 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        worm.SetActive(false);
    }
}
