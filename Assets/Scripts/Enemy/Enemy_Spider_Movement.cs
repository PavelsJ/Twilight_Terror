using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider_Movement : MonoBehaviour, IEnemy, IInteractable
{
    public Vector2 firstDirection = Vector2.right;
    private Vector3 currentDirection;
    
    public float speed = 5;
    public bool isMoving = false;
    public bool isChasingPlayer = false;
    
    public Transform movePoint; 
    public Transform player;
    
    public LayerMask groundLayer;
    public LayerMask boxLayer;
    
    public SpriteMask spriteMask;
    public GameObject bloodSplash;
    
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    
    void Start()
    {
        Player_Steps.Instance.RegisterEnemy(this);

        if (movePoint != null)
        {
            movePoint.parent = null;
        }
        
        currentDirection = firstDirection;
        
        spriteMask.enabled = false;
    }
    
    void Update()
    {
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
    }
    
    public void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;
        
        if (isChasingPlayer && player != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            PatrolMovement();
        }

        isMoving = true;
    }
    
    private void MoveTowardsPlayer()
    {
        List<Vector3> path = CalculatePath(movePoint.position, player.position);
        
        if (path != null && path.Count > 1) 
        {
            pathQueue.Clear();
            for (int i = 1; i < path.Count; i++) // Заполняем очередь (игнорируем 0-й элемент, так как это текущая позиция)
            {
                pathQueue.Enqueue(path[i]);
            }

            if (pathQueue.Count > 0)
            {
                movePoint.position = pathQueue.Dequeue();
            }
        }
    }

    private void PatrolMovement()
    {
        if (CanMove(currentDirection))
        {
            movePoint.position += currentDirection;
        }
        else
        {
            currentDirection = -currentDirection;
            if (CanMove(currentDirection))
            {
                movePoint.position += currentDirection;
            }
        }
    }

    private List<Vector3> CalculatePath(Vector3 start, Vector3 goal)
    {
        Queue<Vector3> frontier = new Queue<Vector3>();
        frontier.Enqueue(start);

        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        cameFrom[start] = start;

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
                }
            }
        }

        return null; // Если пути нет
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
        Vector3[] possibleMoves = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<Vector3> neighbors = new List<Vector3>();

        foreach (Vector3 move in possibleMoves)
        {
            Vector3 targetPosition = position + move;
            if (!Physics2D.OverlapPoint(targetPosition, boxLayer) && Physics2D.OverlapPoint(targetPosition, groundLayer))
            {
                neighbors.Add(targetPosition);
            }
        }

        return neighbors;
    }
    
    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPosition = movePoint.position + direction;
        return !Physics2D.OverlapPoint(targetPosition, boxLayer) && Physics2D.OverlapPoint(targetPosition, groundLayer);
    }

    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Player_Steps.Instance.DeregisterEnemy(this);
        
        Destroy(gameObject);
    }
}
