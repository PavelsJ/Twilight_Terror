using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using TMPro;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private static Player_Movement instance;
    
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
    
    public static Player_Movement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player_Movement>();
            }
            return instance;
        }
    }
    
    void Start()
    {
        movePoint.parent = null;
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
                Vector3 direction = HandleInput();
                if (direction != Vector3.zero && !isDisable)
                {
                    HandleMove(direction);
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
    }
    
    private Vector3 HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) return Vector3.up;
        if (Input.GetKeyDown(KeyCode.S)) return Vector3.down;
        if (Input.GetKeyDown(KeyCode.A)) return Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) return Vector3.right;
        
        return Vector3.zero;
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
        else if (CanMoveTo(targetPosition, groundLayer))
        {
            Player_Steps.Instance.NotifyEnemiesOfPlayerMove();
            
            Move(targetPosition);
        }
        else if (CanMoveTo(targetPosition, voidLayer))
        {
            Move(targetPosition);
            
            isDead = true;
            StartCoroutine(FallToTheVoid());
        }
        else if (CanMoveTo(targetPosition, iceLayer))
        {
            Player_Steps.Instance.NotifyEnemiesOfPlayerMove();
            
            MoveOnIce(targetPosition);
        }
    }

    private void MoveOnIce(Vector3 direction)
    {
        Vector3 slideDirection = direction - movePoint.position;
        Vector3 targetPosition = movePoint.position;
        
        int count = 0;
        while (true && count < 100)
        {
            Vector3 nextPosition = targetPosition + slideDirection;
            
            if (Physics2D.OverlapPoint(nextPosition, boxLayer))
                break; 
            
            if (!Physics2D.OverlapPoint(nextPosition, iceLayer))
            {
                if (Physics2D.OverlapPoint(nextPosition, groundLayer) 
                    || Physics2D.OverlapPoint(nextPosition, voidLayer))
                {
                    targetPosition = nextPosition;
                }
                break;
            }

            targetPosition = nextPosition;
            count++;
        }
        
        Move(targetPosition);
        
        if (Physics2D.OverlapPoint(targetPosition, voidLayer))
        {
            isDead = true;
            StartCoroutine(FallToTheVoid());
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
    }
    
    private bool CanMoveTo(Vector3 position, LayerMask layer)
    {
        return Physics2D.OverlapPoint(position, layer);
    }
    
    public void HitByTrap()
    {
        isMoving = false; 
    }
}