using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Centipede_Movement : MonoBehaviour, IEnemy
{
    public float speed = 5f; 
    public bool isMoving = false;
  
    public Transform player;
    private Transform target;
    
    public Transform movePoint; 
    public LayerMask groundLayer;
    public Copy_Past_Movement copy;
    
    public float detectionRange = 2f; 
    public SpriteMask spriteMask;
    
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    
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
                
                if (distanceToPlayer <= detectionRange)
                {
                    if (spriteMask != null)
                    {
                        spriteMask.enabled = true; 
                    }
                }
                else
                {
                    if (spriteMask != null)
                    {
                        spriteMask.enabled = false; 
                    }
                }
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
            List<Vector3> path = CalculatePath(movePoint.position, target.position);
            
            if (path != null && path.Count > 1) 
            {
                copy.UpdateSegmentPosition(movePoint.position);
                movePoint.position = path[1];
                isMoving = true;
            }
        }
        else
        {
            Debug.LogWarning("No target available. Pathfinding skipped.");
        }
    }

    private Transform FindPriorityTarget()
    {
        GameObject[] priorityObjects = GameObject.FindGameObjectsWithTag("Interactable");
        
        if (priorityObjects.Length > 0)
        {
            float shortestDistance = float.MaxValue;
            Transform closestObject = null;
        
            foreach (var obj in priorityObjects)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestObject = obj.transform;
                }
            }
        
            return closestObject;
        }

        return player; 
    }


    private List<Vector3> CalculatePath(Vector3 start, Vector3 goal)
    {
        Queue<Vector3> frontier = new Queue<Vector3>();
        frontier.Enqueue(start);

        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        cameFrom[start] = start;

        List<Vector3> visited = new List<Vector3>();

        while (frontier.Count > 0)
        {
            Vector3 current = frontier.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                    visited.Add(neighbor);
                }
            }
        }
        
        Vector3 closestPoint = GetClosestPoint(visited, goal);
        if (closestPoint != start) 
        {
            return new List<Vector3> { start, closestPoint };
        }

        return null;
    }

    private Vector3 GetClosestPoint(List<Vector3> points, Vector3 goal)
    {
        float shortestDistance = float.MaxValue;
        Vector3 closestPoint = goal;

        foreach (var point in points)
        {
            float distance = Vector3.Distance(point, goal);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
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
    
}