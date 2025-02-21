using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MazeGenerator : MonoBehaviour
{
   [Header("Segment Settings")]
    public int segmentSize = 5; // Размер одного сегмента
    public Tilemap tilemap;
    public TileBase wallTile;
    public TileBase pathTile;

    [Header("Player Settings")]
    public Transform playerTransform;

    private Vector2Int currentSegment = Vector2Int.zero; // Текущий сегмент, в котором находится игрок
    private int direction = 0; // Текущее направление (0 → 1 → 2 → 3 → 0)

    private readonly Dictionary<Vector2Int, int[,]> segments = new Dictionary<Vector2Int, int[,]>();

    void Start()
    {
        // Генерация начального сегмента
        GenerateSegment(currentSegment);
        RenderSegment(currentSegment);
    }

    void Update()
    {
        Vector2Int playerSegment = GetPlayerSegment(playerTransform.position);

        if (playerSegment != currentSegment)
        {
            // Обновление сегментов только при смене текущего сегмента
            HandleSegmentChange(playerSegment);
        }
    }

    private void HandleSegmentChange(Vector2Int newSegment)
    {
        currentSegment = newSegment;

        // Определяем позицию следующего сегмента на основе направления
        Vector2Int nextSegment = GetNextSegmentPosition(currentSegment, direction);

        if (!segments.ContainsKey(nextSegment))
        {
            GenerateSegment(nextSegment);
        }

        // Обновляем только тот сегмент, который будет заменен
        RenderSegment(nextSegment);

        // Переход к следующему направлению в порядке 0 → 1 → 2 → 3 → 0
        direction = (direction + 1) % 4;
    }

    private void GenerateSegment(Vector2Int position)
    {
        int[,] segment = new int[segmentSize, segmentSize];

        // Генерация случайного лабиринта
        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                segment[x, y] = Random.value > 0.7f ? 1 : 0; // 1 - стена, 0 - путь
            }
        }

        // Сохранение сегмента в словарь
        segments[position] = segment;
    }

    private void RenderSegment(Vector2Int position)
    {
        if (!segments.TryGetValue(position, out var segment)) return;

        Vector3Int offset = new Vector3Int(position.x * segmentSize, position.y * segmentSize, 0);

        // Отображение сегмента в Tilemap
        for (int x = 0; x < segmentSize; x++)
        {
            for (int y = 0; y < segmentSize; y++)
            {
                Vector3Int tilePosition = offset + new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, segment[x, y] == 1 ? wallTile : pathTile);
            }
        }
    }

    private Vector2Int GetNextSegmentPosition(Vector2Int current, int dir)
    {
        // Смещения для направлений (0 - вправо, 1 - вниз, 2 - влево, 3 - вверх)
        Vector2Int[] offsets = { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };
        return current + offsets[dir];
    }

    private Vector2Int GetPlayerSegment(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / segmentSize),
            Mathf.FloorToInt(position.y / segmentSize)
        );
    }
}
