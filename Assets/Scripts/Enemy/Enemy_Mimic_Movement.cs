using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Movement : MonoBehaviour, IEnemy, IInteractable
{
    private Coroutine coroutine;
    
    [Header("Mimic Settings")]
    public float speed = 5f;
    private Vector3 targetPosition;
    
    [Header("Compounds")]
    public SpriteMask spriteMask;
    public GameObject bloodSplash;

    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);

        targetPosition = transform.position;
    }

    public void OnPlayerMoved()
    {
        Vector3 oppositeDirection = Player_Movement_Manager.Instance.GetLastMoveDirection(); 
        targetPosition += new Vector3(-oppositeDirection.x, oppositeDirection.y);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(MoveSmoothly(targetPosition));
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
    
    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        
        Destroy(gameObject);
    }
}
