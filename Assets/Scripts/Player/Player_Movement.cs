using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using TMPro;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public static Player_Movement Instance { get; private set; }
    
    [Header("Player Settings")]
    public float speed = 5f; 
    
    public bool isMoving = true; 
    public bool isDisable  = false;
    public bool isDead = false; 
    
    public Transform movePoint;
    public Transform arrowPoint;

    [Header("Layer Settings")] 
    public LayerMask groundLayer;
    public LayerMask voidLayer;  
    public LayerMask iceLayer;
    public LayerMask boxLayer;
    
    [Header("Trap Settings")]
    public float trapCooldown = 0.5f;
    private float trapTimer = 0f;
    
    [Header("Compounds")]
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        movePoint.parent = null;
        isDisable = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

   

    void Update()
    {
        if (!isDead)
        {
            if (trapTimer > 0f)
            {
                trapTimer -= Time.deltaTime; 
                return;
            }
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                Vector3 direction = Vector3.zero;
                
                if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.up;
                else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.down;
                else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
                else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;
                
                if (direction != Vector3.zero && !isDisable)
                {
                    HandleMove(direction);
                }
            }
        }
        
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
    }
    
    private void HandleMove(Vector3 direction)
    {
        if (!isMoving)
        {
            isMoving = true;
            Player_Steps.Instance.NotifyEnemiesOfPlayerMove();
            trapTimer = trapCooldown;
            return;
        }

        TryMove(direction);
    }

    private void TryMove(Vector3 direction)
    {
        Vector3 targetPosition = movePoint.position + direction;
        
        Collider2D boxHit = Physics2D.OverlapPoint(targetPosition, boxLayer);
            
        if (boxHit)
        {
            Box_Interaction box = boxHit.GetComponent<Box_Interaction>();
            
            if (box != null && box.TryPush(direction))
            {
                Player_Steps.Instance.NotifyEnemiesOfPlayerMove();
                
                Move(targetPosition);
            }
        }
        else
        {
            if (Physics2D.OverlapPoint(targetPosition, groundLayer))
            {
                Player_Steps.Instance.NotifyEnemiesOfPlayerMove();

                if (isMoving)
                {
                    Move(targetPosition);
                }
            }
            else if (Physics2D.OverlapPoint(targetPosition, voidLayer))
            {
                if (isMoving)
                {
                    Move(targetPosition);
                    isDead = true;

                    StartCoroutine(FallToTheVoid());
                   
                }
            }
            else if (Physics2D.OverlapPoint(targetPosition, iceLayer))
            {
                Player_Steps.Instance.NotifyEnemiesOfPlayerMove();

                if (isMoving)
                {
                    MoveOnIce(targetPosition);
                }
            }
        }
    }

    private void MoveOnIce(Vector3 direction)
    {
        Vector3 slideDirection = direction - movePoint.position;
        Vector3 currentPosition = movePoint.position;

        int count = 0;
        while (Physics2D.OverlapPoint(currentPosition + slideDirection, iceLayer) && count < 100)
        {
            currentPosition += slideDirection;
            count++;
        }
        
        if (Physics2D.OverlapPoint(currentPosition + slideDirection, boxLayer))
        {
            Move(currentPosition);
            return;
        }
        
        if (Physics2D.OverlapPoint(currentPosition + slideDirection, groundLayer))
        {
            Move(currentPosition + slideDirection);
        }
        else if (Physics2D.OverlapPoint(currentPosition + slideDirection, voidLayer))
        {
            Move(currentPosition + slideDirection);
            isDead = true;
            
            StartCoroutine(FallToTheVoid());
        }
        else
        {
            Move(currentPosition);
        }
    }

    private IEnumerator FallToTheVoid()
    {
        yield return new WaitForSeconds(0.6f);
        
        GetComponent<FOD_Agent>().EndAgent();
        GetComponent<SpriteRenderer>().sortingOrder = -5;
       
        yield return new WaitForSeconds(0.5f);
        
        movePoint.position += Vector3.down;
        
        yield return new WaitForSeconds(0.2f);
        
        FOD_Manager manager = FindObjectOfType<FOD_Manager>(true);
        
        if (manager != null)
        {
            manager.RemoveAgentsGradually();
        }
       
        gameObject.SetActive(false);
    }
    
    public void MovePlayerTo(Vector2 newPos)
    {
        isDisable = false;
        Move(newPos);
    }
    
    private void Move(Vector3 pos)
    {
        movePoint.position =  pos;
        
        Vector3 direction = (pos - transform.position).normalized;
        
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
           spriteRenderer.flipX = true; 
        }
        
        // RotateArrow(direction);
    }

    private void RotateArrow(Vector3 dir)
    {
        Quaternion targetRotation = arrowPoint.rotation;
        
        if (dir == Vector3.up)
        {
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir == Vector3.down)
        {
            targetRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (dir == Vector3.left)
        {
            targetRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dir == Vector3.right)
        {
            targetRotation = Quaternion.Euler(0, 0, -90);
        }

        arrowPoint.rotation = targetRotation;
    }
    
    public void HitByTrap()
    {
        isMoving = false; 
    }
}