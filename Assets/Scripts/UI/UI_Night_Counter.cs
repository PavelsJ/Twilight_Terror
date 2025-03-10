using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_Night_Counter : MonoBehaviour
{
    public TMP_Text oldTextTens, newTextTens; // Десятки
    public TMP_Text oldTextOnes, newTextOnes; // Единицы
    
    public float animationTime = 0.3f;
    public float randomizationTime = 0.6f;

    private int currentScore = 0;

    private void OnEnable()
    {
        if(PlayerPrefs.HasKey("NightCount"))
        {
            currentScore = PlayerPrefs.GetInt("NightCount");
        }
        
        UpdateScore(currentScore);
        Debug.Log(currentScore);
    }
    
     private void UpdateScore(int newScore)
    {
        int newTens = newScore / 10;
        int newOnes = newScore % 10;

        StartCoroutine(FlipNumber(oldTextTens, newTextTens, newTens));
        StartCoroutine(FlipNumber(oldTextOnes, newTextOnes, newOnes));
    }

    private IEnumerator FlipNumber(TMP_Text oldText, TMP_Text newText, int newValue)
    {
        float elapsedTime = 0f;
        Vector2 oldStartPos = oldText.rectTransform.anchoredPosition;
        Vector2 newStartPos = new Vector2(0, -50);

        Color oldColor = oldText.color;
        Color newColor = newText.color;
        newColor.a = 0f; // Начинаем с прозрачного текста
        newText.color = newColor;

        newText.rectTransform.anchoredPosition = newStartPos;

        // Запускаем случайные изменения текста в обоих полях
        Coroutine randomCoroutineOld = StartCoroutine(RandomizeText(oldText, oldText.text));
        Coroutine randomCoroutineNew = StartCoroutine(RandomizeText(newText, newValue.ToString()));

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationTime;

            oldText.rectTransform.anchoredPosition = Vector2.Lerp(oldStartPos, new Vector2(0, 50), t);
            newText.rectTransform.anchoredPosition = Vector2.Lerp(newStartPos, Vector2.zero, t);

            oldColor.a = Mathf.Lerp(1f, 0f, t);
            newColor.a = Mathf.Lerp(0f, 1f, t);
            oldText.color = oldColor;
            newText.color = newColor;

            yield return null;
        }

        // Останавливаем случайное изменение и устанавливаем финальное значение
        StopCoroutine(randomCoroutineOld);
        StopCoroutine(randomCoroutineNew);
        newText.text = newValue.ToString();

        // Финальная фиксация
        oldText.text = newText.text;
        oldText.rectTransform.anchoredPosition = Vector2.zero;
        oldText.color = Color.white;
        newText.color = Color.white;
    }

    private IEnumerator RandomizeText(TMP_Text text, string finalValue)
    {
        float time = 0f;
        float delay = 0.05f; // Начальная скорость смены цифр

        while (time < randomizationTime)
        {
            text.text = Random.Range(0, 10).ToString();
            yield return new WaitForSeconds(delay);

            time += delay;
            delay *= 1.2f; // Постепенное замедление смены цифр
        }

        text.text = finalValue; // Фиксируем настоящее число
    }
}
