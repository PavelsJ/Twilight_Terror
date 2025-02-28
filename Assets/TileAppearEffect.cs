using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAppearEffect : MonoBehaviour
{
    public float moveDuration = 1f;
    public float startOffset = -1f;

    private Tilemap tilemap;
    private Vector3 originalPosition;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        StartCoroutine(AppearEffect());
    }

    private IEnumerator AppearEffect()
    {
        transform.position = originalPosition + Vector3.down * startOffset;
        tilemap.color = new Color(1, 1, 1, 0); 

        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(originalPosition + Vector3.down * startOffset, originalPosition, t);
            tilemap.color = new Color(1, 1, 1, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        tilemap.color = new Color(1, 1, 1, 1);
    }
}