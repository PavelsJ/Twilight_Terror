using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dung_Eater_Movement : MonoBehaviour, IEnemy, IInteractable
{
    [Header("Dung Eater Settings")]
    public Vector2 firstDirection = Vector2.up;
        
    public float speed = 5;
    
    public bool isMoving = false;
    public bool isDead = false;
    
    [Header("Transform References")] 
    public Transform movePoint;
    
    [Header("Layer Settings")] 
    public LayerMask wallLayer;
    public LayerMask boxLayer;
    
    [Header("Compounds")]
    public GameObject lightBulb;
    public GameObject bloodSplash;
    
    private Vector3 currentDirection;
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        currentDirection = firstDirection;
        
        if (movePoint != null)
        {
            movePoint.parent = null;
        }
        
        StartCoroutine(DelayedStart());
    }
    
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);

        ChooseAvailableDirection();
    }

    void Update()
    {
        if (isMoving && movePoint != null && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            Vector3 direction = movePoint.position - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
    }
    
    private void RotateSprite(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); 
    }

    public void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;
        
        if (CanMove(currentDirection))
        {
            movePoint.position += currentDirection;
        }
        else
        {
            ChooseAvailableDirection(); 
            
            if (CanMove(currentDirection))
            {
                movePoint.position += currentDirection;
            }
        }
        
        isMoving = true;
    }
    
    private void ChooseAvailableDirection()
    {
        List<Vector3> availableDirections = GetAvailableDirections();

        if (availableDirections.Count > 0)
        {
            currentDirection = availableDirections[Random.Range(0, availableDirections.Count)];
        }
        else
        {
            currentDirection = Vector3.zero; 
        }
    }

    private List<Vector3> GetAvailableDirections()
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<Vector3> availableDirections = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            if (CanMove(direction))
            {
                availableDirections.Add(direction);
            }
        }

        return availableDirections;
    }
    
    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPosition = movePoint.position + direction;

        if (!Physics2D.OverlapPoint(targetPosition, boxLayer))
        {
            return !Physics2D.OverlapPoint(targetPosition, wallLayer);
        }

        return false;
    }

    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Instantiate(lightBulb, transform.position, Quaternion.identity);
        
        isDead = true;
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        agent.deactivateOnEnd = true;
        agent.EndAgent();
    }
}
