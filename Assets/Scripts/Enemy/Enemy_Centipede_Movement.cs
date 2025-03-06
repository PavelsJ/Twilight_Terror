using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Centipede_Movement : MonoBehaviour, IEnemy
{
    [Header("Centipede Settings")]
    public float speed = 5f;
    public bool isMoving = false;
  
    [Header("Transform References")] 
    public Transform player;
    private Transform target;
    
    public Transform movePoint;
    
    [Header("Layer Settings")] 
    public LayerMask groundLayer;
    
    [Header("Movement Settings")]
    public float detectionRange = 2f;
    public SpriteMask spriteMask;
    
    [Header("Compounds")]
    public Copy_Past_Movement copy;
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
    
    void Start()
    {
        Player_Steps.Instance.RegisterEnemy(this);
        spriteMask.enabled = false; 
        
        if (movePoint != null)
        {
            movePoint.parent = null; 
        }
        
        copy.UpdateSegmentPosition(movePoint.position);
    }

    void Update()
    {
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            Vector3 direction = movePoint.position - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                if (pathQueue.Count > 0)
                {
                    movePoint.position = pathQueue.Dequeue();
                }
                else
                {
                    isMoving = false;
                }
            }

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                spriteMask.enabled = distanceToPlayer <= detectionRange;
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
        
        target = player;

        if (target != null)
        {
            List<Vector3> path = CalculateAStarPath(movePoint.position, target.position);
            
            if (path != null && path.Count > 1)
            {
                copy.UpdateSegmentPosition(movePoint.position);
                movePoint.position = path[1];
                isMoving = true;
            }
        }
    }

    private List<Vector3> CalculateAStarPath(Vector3 start, Vector3 goal)
    {
        PriorityQueue<Vector3> openSet = new PriorityQueue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float> { { start, 0 } };
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float> { { start, Vector3.Distance(start, goal) } };
        
        openSet.Enqueue(start, fScore[start]);
        cameFrom[start] = start;
        
        while (openSet.Count > 0)
        {
            Vector3 current = openSet.Dequeue();
            if (current == goal) return ReconstructPath(cameFrom, current);
            
            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (visitedPositions.Contains(neighbor)) continue;
                
                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor);
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor, goal);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return null;
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        Vector3[] possibleMoves = new Vector3[]
        {
            position + new Vector3(1, 0, 0),
            position + new Vector3(-1, 0, 0),
            position + new Vector3(0, 1, 0),
            position + new Vector3(0, -1, 0)
        };

        List<Vector3> neighbors = new List<Vector3>();
        foreach (Vector3 move in possibleMoves)
        {
            if (Physics2D.OverlapPoint(move, groundLayer)) 
            {
                neighbors.Add(move);
            }
        }

        return neighbors;
    }

    private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3> { current };
        while (cameFrom[current] != current)
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}
