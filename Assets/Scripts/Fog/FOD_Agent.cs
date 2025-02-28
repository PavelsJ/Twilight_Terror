using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class FOD_Agent : MonoBehaviour
{
    [Header("Agent State")]
    public bool isActive = false; 
    public bool deactivateOnEnd = false;
    
    [Header("Agent Customization")]
    [Range(0.0f, 480.0f)] public float sightRange = 50.0f;
    [Range(0.0f, 1.0f)] public float sightTransparency = 0.5f;

    [Header("Light_Flickering")]
    public bool flickering = true;
    public float flickeringSpeed = 6;
    
    private float baseRadius = 0;
    private float targetRadius;
    private bool increasing = false;
    
    private FOD_Manager manager;
    private Coroutine updateRoutine;
    private Action fogInitCallback;

    private void Awake()
    {
        manager = FindObjectOfType<FOD_Manager>(true);
        
        baseRadius = sightRange;
        targetRadius = baseRadius - 2.0f;
    }

    private void OnEnable()
    {
        if (manager == null) return;
        
        isActive = true;
        manager.AddAgent(this);
        
        if (manager.IsFogInitialized)
        {
            StartAgent();
        }
        else
        {
            fogInitCallback = () => StartAgent();
            manager.OnFogInitialized += fogInitCallback;
        }
    }
    
    private void OnDisable()
    {
        if (manager != null)
        {
            if (fogInitCallback != null)
            {
                manager.OnFogInitialized -= fogInitCallback;
            }

            if (!isActive)
            {
                manager.RemoveAgent(this);
            }
        }
    }
    
    private void OnBecameVisible()
    {
        if (!isActive && !gameObject.CompareTag("Player"))
        {
            ActivateAgent();
        }
    }

    private void OnBecameInvisible()
    {
        if (isActive && !gameObject.CompareTag("Player"))
        {
            DeactivateAgent();
        }
    }
    
    private void ActivateAgent()
    {
        if (isActive) return;

        isActive = true;
        manager.AddAgent(this);
        
        StartAgent();
    }
    
    private void DeactivateAgent()
    {
        if (!isActive) return;
        
        EndAgent();
    }
    
    private void StartAgent(float delay = 1f)
    {
        if (updateRoutine != null) StopCoroutine(updateRoutine);
       
        StartCoroutine(FadeIn(delay));
    }

    public void EndAgent(float delay = 0.7f)
    {
        if (updateRoutine != null)
        {
            StopCoroutine(updateRoutine);
            updateRoutine = null;
        }
        
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOut(delay));
        }
    }
    
    private IEnumerator FadeIn(float time)
    {
        float duration = time;
        float elapsedTime = 0.0f;
        float startRadius = 0;

        while (elapsedTime < duration)
        {
            sightRange = Mathf.Lerp(startRadius, baseRadius, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        sightRange = baseRadius;
        updateRoutine = StartCoroutine(UpdateAgent());
    }
    
    private IEnumerator FadeOut(float time)
    {
        float duration = time;
        float elapsedTime = 0.0f;
        float startRadius = sightRange;
        
        while (elapsedTime < duration)
        {
            sightRange = Mathf.Lerp(startRadius, 0, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        sightRange = 0;

        if (!gameObject.CompareTag("Player"))
        {
            isActive = false;
            manager.RemoveAgent(this);
            
            if (deactivateOnEnd)
            {
                yield return new WaitForSeconds(0.2f); 
                gameObject.SetActive(false);
            }
        }
    }

    public void ChangeRadiusValue(float newRadius)
    {
        baseRadius = Mathf.Clamp(newRadius, 0.0f, 480.0f);
        targetRadius = baseRadius - 2.0f;
        flickeringSpeed += 2;
    }

    private IEnumerator UpdateAgent()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            UpdateRadiusValues();
        }
    }

    private void UpdateRadiusValues()
    {
        sightRange = Mathf.MoveTowards(sightRange, targetRadius, flickeringSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(sightRange - targetRadius) < 0.1f)
        {
            increasing = !increasing;
            targetRadius = increasing ? baseRadius : baseRadius - 2.0f;
        }
    }
}
