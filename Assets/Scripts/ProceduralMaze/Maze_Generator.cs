using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze_Generator : MonoBehaviour
{
    [Header("Segment_Settings")] 
    public int segmentSize = 5;
    public int direction = 0;
    public bool clockwise = true;

    private Dictionary<Vector2Int, int[,]> segments = new Dictionary<Vector2Int, int[,]>();
    // private List<Vector3> pathPoints = new List<Vector3>();
    private Vector2Int currentSegment = Vector2Int.zero;
    
    [Header("TileMap_Settings")] public Tilemap tilemap;
    public TileBase wallTile;
    public TileBase pathTile;
    public TileBase entityTile;

    [Header("Entities")] 
    public Entities_Spawn entities;
    private int lightIndex = -1; 
    private int trapIndex = -1;

    [Header("UI_Settings")] public TextMeshProUGUI stageText;
    private int stageCount = 0;
    

    void Start()
    {
        currentSegment = GetPlayerSegment(entities.playerTransform.position);
        ExtractCurrentSegment();
        
        Vector2Int nextSegment = GetNextSegmentPosition(currentSegment, direction);
        
        if (!segments.ContainsKey(nextSegment)) {
            GenerateSegment(nextSegment);
            ConnectSegments(currentSegment, nextSegment);
            RenderSegment(nextSegment);
        }
        
        direction = (clockwise ? (direction + 1) % 4 : (direction + 3)) % 4;
    }
    
    private void ExtractCurrentSegment()
    {
        int[,] segment = new int[segmentSize, segmentSize];
        
        Vector3Int offset = new Vector3Int(currentSegment.x * segmentSize, currentSegment.y * segmentSize, 0);

        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                Vector3Int tilePosition = offset + new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePosition);
                
                segment[x, y] = tile == wallTile ? 1 : 0;
            }
        }
        
        segments[currentSegment] = segment;
    }

    void Update()
    {
        Vector2Int playerSegment = GetPlayerSegment(entities.playerTransform.position);
        
        if (playerSegment != currentSegment)
        {
            HandleSegmentChange(playerSegment, currentSegment);
        }
    }

    private void HandleSegmentChange(Vector2Int playerSegment, Vector2Int previousSegment)
    {
        currentSegment = playerSegment;

        Vector2Int movementDirection = currentSegment - previousSegment;
        if (IsOppositeDirection(movementDirection, direction))
        {
            clockwise = !clockwise;
            Debug.Log($"Direction is opposite. Switching clockwise to: {clockwise}");
        }

        Vector2Int nextSegment = GetNextSegmentPosition(currentSegment, direction);

        ClearSegment(nextSegment);

        if (!segments.ContainsKey(nextSegment))
        {
            GenerateSegment(nextSegment);
            ConnectSegments(currentSegment, nextSegment);
            RenderSegment(nextSegment);
        }

        direction = (clockwise ? (direction + 1) % 4 : (direction + 3)) % 4;

        if (direction == 1)
        {
            stageCount += clockwise ? 1 : -1;
            
            UpdateRandomSegments();
            UpdateStageText();
        }
        
        if (direction == lightIndex)
        {
            PlaceEntitiesInSegment(entities.lightPrefab, nextSegment);
        }
            
        if (direction == trapIndex )
        {
            PlaceEntitiesInSegment(entities.bearTrap, nextSegment);
        }
    }
    
    void UpdateRandomSegments()
    {
        lightIndex = Random.Range(0, 4);
        
        trapIndex = Random.Range(0, 4);

        Debug.Log($"Light will appear in segment {lightIndex}");
        Debug.Log($"Trap will appear in segment {trapIndex}");
    }

    private bool IsOppositeDirection(Vector2Int movement, int currentDirection)
    {
        Vector2Int expectedOffset = clockwise 
            ? ClockwiseOffsets[currentDirection] 
            : CounterClockwiseOffsets[currentDirection];

        return movement == -expectedOffset; 
    }

    private void ClearSegment(Vector2Int position)
    {
        if (!segments.ContainsKey(position)) return; 

        Vector3Int offset = new Vector3Int(position.x * segmentSize, position.y * segmentSize, 0);

        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                Vector3Int tilePosition = offset + new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, null);
            }
        }

        segments.Remove(position); 
    }

    private void GenerateSegment(Vector2Int position)
    {
        int[,] segment = new int[segmentSize, segmentSize];

        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                segment[x, y] = Random.value > 0.5f ? 1 : 0; // 1 - стена, 0 - путь
            }
        }
        
        SetSegmentCorner(segment, position);   
        
        ConnectIsolatedZones(segment);
        
        segments[position] = segment;
    }

    void SetSegmentCorner(int[,] segment, Vector2Int position)
    {
        if (position.x >= 0)
        {
            if (position.y >= 0)
            {
                segment[0, 0] = 1; // Верхний левый угол
            }
            else
            {
                segment[0, segmentSize - 1] = 1; // Нижний левый угол
            }
        }
        else
        {
            if (position.y >= 0)
            {
                segment[segmentSize - 1, 0] = 1; // Верхний правый угол
            }
            else
            {
                segment[segmentSize - 1, segmentSize - 1] = 1; // Нижний правый угол
            }
        }
    }

    void ConnectIsolatedZones(int[,] segment)
    {
        int[,] visited = new int[segmentSize, segmentSize];
        List<List<Vector2Int>> zones = new List<List<Vector2Int>>();

        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                if (segment[x, y] == 0 && visited[x, y] == 0)
                {
                    List<Vector2Int> zone = new List<Vector2Int>();
                    DFS(segment, visited, x, y, zone);
                    zones.Add(zone);
                }
            }
        }

        for (int i = 1; i < zones.Count; i++)
        {
            Vector2Int start = zones[0][0];
            Vector2Int end = zones[i][0];
            CreatePath(segment, start, end);
        }
    }

    void DFS(int[,] segment, int[,] visited, int x, int y, List<Vector2Int> zone)
    {
        if (x < 0 || y < 0 || x >= segmentSize || y >= segmentSize) return;
        if (segment[x, y] == 1 || visited[x, y] == 1) return;

        visited[x, y] = 1;
        zone.Add(new Vector2Int(x, y));

        DFS(segment, visited, x + 1, y, zone);
        DFS(segment, visited, x - 1, y, zone);
        DFS(segment, visited, x, y + 1, zone);
        DFS(segment, visited, x, y - 1, zone);
    }

    void CreatePath(int[,] segment, Vector2Int start, Vector2Int end)
    {
        while (start.x != end.x)
        {
            segment[start.x, start.y] = 0;
            if (start.x < end.x) start.x++;
            else if (start.x > end.x) start.x--;
        }

        while (start.y != end.y)
        {
            segment[start.x, start.y] = 0;
            if (start.y < end.y) start.y++;
            else if (start.y > end.y) start.y--;
        }
    }

    private void ConnectSegments(Vector2Int from, Vector2Int to)
    {
        if (!segments.ContainsKey(from) || !segments.ContainsKey(to))
        {
            Debug.LogWarning($"Cannot connect segments: {from} or {to} is missing.");
            return;
        }

        int[,] fromSegment = segments[from];
        int[,] toSegment = segments[to];

        List<Vector2Int> currentPathPoints = GetPathPoints(fromSegment);
        List<Vector2Int> nextPathPoints = GetPathPoints(toSegment);

        if (currentPathPoints.Count == 0 || nextPathPoints.Count == 0)
        {
            Debug.LogWarning("No path points to connect between segments!");
            return;
        }
        
        Vector2Int fromOffset = new Vector2Int(from.x * segmentSize, from.y * segmentSize);
        Vector2Int toOffset = new Vector2Int(to.x * segmentSize, to.y * segmentSize);

        Vector2Int closestPointA = Vector2Int.zero;
        Vector2Int closestPointB = Vector2Int.zero;
        float minDistance = float.MaxValue;

        foreach (var pointA in currentPathPoints)
        {
            foreach (var pointB in nextPathPoints)
            {
                Vector2Int worldPointA = pointA + fromOffset;
                Vector2Int worldPointB = pointB + toOffset;

                float distance = (worldPointA - worldPointB).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPointA = worldPointA;
                    closestPointB = worldPointB;
                }
            }
        }

        CreatePathBetweenPoints(
            closestPointA, closestPointB,
            fromSegment, toSegment, 
            fromOffset, toOffset
            );
        
        RenderSegment(from);
    }

    private List<Vector2Int> GetPathPoints(int[,] segment)
    {
        List<Vector2Int> pathPoints = new List<Vector2Int>();
        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                if (segment[x, y] == 0) // Path point
                {
                    pathPoints.Add(new Vector2Int(x, y));
                }
            }
        }

        return pathPoints;
    }

    private void CreatePathBetweenPoints(
        Vector2Int start, Vector2Int end,
        int[,] fromSegment, int[,] toSegment,
        Vector2Int fromOffset, Vector2Int toOffset)
    {
        //pathPoints.Clear();
        
        Vector2Int current = start;
        while (current != end)
        {
            if (current.x != end.x)
                current.x += current.x < end.x ? 1 : -1;
            else if (current.y != end.y)
                current.y += current.y < end.y ? 1 : -1;

            // Определяем, в какой сегмент попадает текущая точка
            Vector2Int localInFromSegment = current - fromOffset;
            Vector2Int localInToSegment = current - toOffset;

            if (localInFromSegment.x >= 0 && localInFromSegment.x < segmentSize &&
                localInFromSegment.y >= 0 && localInFromSegment.y < segmentSize)
            {
                fromSegment[localInFromSegment.x, localInFromSegment.y] = 0;
            }

            if (localInToSegment.x >= 0 && localInToSegment.x < segmentSize &&
                localInToSegment.y >= 0 && localInToSegment.y < segmentSize)
            {
                toSegment[localInToSegment.x, localInToSegment.y] = 0;
            }

            //pathPoints.Add(new Vector3(current.x + 0.5f, current.y + 0.5f, 0));
        }
    }

    private void RenderSegment(Vector2Int position)
    {
        int[,] segment = segments[position];
        Vector3Int offset = new Vector3Int(position.x * segmentSize, position.y * segmentSize, 0);

        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                Vector3Int tilePosition = offset + new Vector3Int(x, y, 0);
                
                if (tilemap.GetTile(tilePosition) == entityTile)
                    continue;
                
                tilemap.SetTile(tilePosition, segment[x, y] == 1 ? wallTile : pathTile);
            }
        }
    }
    
    void PlaceEntitiesInSegment(GameObject entityPrefab, Vector2Int segmentPosition)
    {
        if (!segments.ContainsKey(segmentPosition))
            return;
        
        int[,] segment = segments[segmentPosition];
        Vector3Int offset = new Vector3Int(segmentPosition.x * segmentSize, segmentPosition.y * segmentSize, 0);
        
        List<Vector3Int> pathPositions = new List<Vector3Int>();
        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                if (segment[x, y] == 0) 
                {
                    pathPositions.Add(offset + new Vector3Int(x, y, 0));
                }
            }
        }
        
        if (pathPositions.Count > 0)
        {
            Vector3Int randomPosition = pathPositions[Random.Range(0, pathPositions.Count)];
            Vector3 worldPosition = tilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0); 
            
            tilemap.SetTile(randomPosition, entityTile);
            Instantiate(entityPrefab, worldPosition, Quaternion.identity);
        }
    }

    private static readonly Vector2Int[] ClockwiseOffsets = 
    {
        Vector2Int.right, 
        Vector2Int.down, 
        Vector2Int.left, 
        Vector2Int.up
    };

    private static readonly Vector2Int[] CounterClockwiseOffsets = 
    {
        Vector2Int.down, 
        Vector2Int.left, 
        Vector2Int.up, 
        Vector2Int.right
    };

    private Vector2Int GetNextSegmentPosition(Vector2Int current, int direction)
    {
        Vector2Int offset = clockwise 
            ? ClockwiseOffsets[direction] 
            : CounterClockwiseOffsets[direction];

        return current + offset;
    }

    private Vector2Int GetPlayerSegment(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / segmentSize),
            Mathf.FloorToInt(position.y / segmentSize));
    }

    void UpdateStageText()
    {
        if (stageText != null)
        {
            stageText.text = "Stage: " + stageCount;
        }
    }

    void OnDrawGizmos()
    {
        /*if (segments.Count > 0)
        {
            foreach (var segmentEntry in segments)
            {
                Vector2Int segmentPos = segmentEntry.Key;
                int[,] segment = segmentEntry.Value;
                Vector3Int offset = new Vector3Int(segmentPos.x * segmentSize, segmentPos.y * segmentSize, 0);
                
                for (int x = 0; x < segmentSize; x++)
                {
                    for (int y = 0; y < segmentSize; y++)
                    {
                        if (segment[x, y] == 0) 
                        {
                            Gizmos.color = Color.green; // Path points
                            Gizmos.DrawSphere(offset + new Vector3Int(x , y, 0) + new Vector3(0.5f,0.5f), 0.2f);
                        }
                    }
                }
            }
        }*/
        
        /*if (pathPoints.Count > 0)
        {
            Gizmos.color = Color.red;
            
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            }
            
            Gizmos.color = Color.blue;
            foreach (var point in pathPoints)
            {
                Gizmos.DrawSphere(point, 0.1f); 
            }
        }*/
    }
}