using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lurking_Shadow_Movement : MonoBehaviour, IEnemy, IInteractable
{
    [Header("Spider Settings")]
    private Vector3 currentDirection;
    public float chaseDistance = 1f;
    
    public float speed = 5;
    public bool isNear = false;
    
    private bool isMoving = false;
    private bool isChasingPlayer = false;
    
    [Header("Transform References")] 
    public Transform movePoint; 
    private Transform player;
    
    private Transform startPosition;
    
    [Header("Layer Settings")] 
    public LayerMask groundLayer;
    public LayerMask boxLayer;
    
    [Header("Compounds")]
    public SpriteMask spriteMask;
    public GameObject bloodSplash;
    
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        
        player = FindObjectOfType<Player_Movement>(true).transform;
        
        startPosition = transform;

        if (movePoint != null)
        {
            movePoint.parent = null;
        }
    }
    
    public void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;

        if (isChasingPlayer)
        {
            if (isNear && player != null)
            {
                MoveTowardsTarget(player);
            }
            else
            {
                MoveTowardsTarget(startPosition);
            }
        
            isMoving = true;
        }
    }

    public void Activate()
    {
        isChasingPlayer = true;
    }
    
    private void Update()
    {
        if (!isChasingPlayer) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= chaseDistance)
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }
        
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
    }
    
    private void MoveTowardsTarget(Transform target)
    {
        List<Vector3> path = CalculatePath(movePoint.position, target.position);
        
        if (path != null && path.Count > 1) 
        {
            pathQueue.Clear();
            for (int i = 1; i < path.Count; i++) 
            {
                pathQueue.Enqueue(path[i]);
            }

            if (pathQueue.Count > 0)
            {
                movePoint.position = pathQueue.Dequeue();
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
    
    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        
        Destroy(gameObject);
    }
}
