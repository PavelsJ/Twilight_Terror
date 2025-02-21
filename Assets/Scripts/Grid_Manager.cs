using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FODMapping;
using UnityEngine;

public class Grid_Manager : MonoBehaviour
{
    public Transform firstSector;
    public Transform[] midSectors;
    public Transform lastSector;

    public Transform sectorPosParent;
    public Transform playerTargetPos;
    
    private List<Transform> sectorPos;
    
    private float transitionDuration = 1.5f;
    private bool isActive = false;
    private CinemachineVirtualCamera cinemachine;

    private void Awake()
    {
        cinemachine = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }
    
    void Start()
    {
        sectorPos = sectorPosParent.GetComponentsInChildren<Transform>()
            .Where(t => t != sectorPosParent)
            .ToList();
        
        foreach (var sector in midSectors)
        {
            sector.gameObject.SetActive(false);
        }
    }

    public void OnStart(int index)
    {
        if (!isActive)
        {
            FOD_Manager manager = FindObjectOfType<FOD_Manager>(true);
            if (manager != null)
            {
                manager.gameObject.SetActive(true);
                manager.StartCoroutine(manager.EnableWithDelay(0.8f));
            }

            StartCoroutine(MoveSectorsSimultaneously(index));
            isActive = true;
        }
    }

    public void OnActive(int sectorPosIndex)
    {
        StartCoroutine(MoveSectorsSimultaneously(sectorPosIndex));
    }
    
    private IEnumerator MoveSectorsSimultaneously(int index)
    {
        yield return StartCoroutine(MoveSector(lastSector, sectorPos[index + 1].position));
        midSectors[index].gameObject.SetActive(true);
        // yield return StartCoroutine(MoveSector(firstSector, sectorPos[index].position));
        
        // for (int i = 0; i < midSectors.Length; i++)
        // {
        //     midSectors[i].gameObject.SetActive(i == index);
        // }
    }

    private IEnumerator MoveSector(Transform sector, Vector2 targetPos)
    {
        Vector2 startPos = sector.position;
        float elapsedTime = 0f;
        float maxShakeIntensity = 0.5f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float shakeStrength = Mathf.SmoothStep(0f, maxShakeIntensity, t < 0.5f ? t * 2 : (1 - t) * 2);

            sector.position = Vector2.Lerp(startPos, targetPos, t);
            ShakeCamera(shakeStrength);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sector.position = targetPos;
        ShakeCamera(0);
    }

    private void ShakeCamera(float intensity)
    {
        CinemachineBasicMultiChannelPerlin amplitude =
            cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        amplitude.m_AmplitudeGain = intensity;
    }

    public void ChangePlayerPos()
    {
        GameObject player = Player_Movement.Instance.gameObject;
        if (player != null)
        {
            Player_Movement.Instance.movePoint.position = playerTargetPos.position;
            player.transform.position = playerTargetPos.position;
            
            player.SetActive((true));
        }
    }
}
