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
    private Vector2 firstPos;
    
    private float transitionDuration;
    
    private bool isActive = false;
    private bool isStart = true;
    
    private CinemachineVirtualCamera cinemachine;

    private void Awake()
    {
        cinemachine = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }
    
    void Start()
    {
        FOD_Manager manager = FindObjectOfType<FOD_Manager>(true);
        
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            manager.StartCoroutine(manager.EnableInstantly());
        }
        
        sectorPos = sectorPosParent.GetComponentsInChildren<Transform>()
            .Where(t => t != sectorPosParent)
            .ToList();
        
        foreach (var sector in midSectors)
        {
            sector.gameObject.SetActive(false);
        }

        firstPos = new Vector2(0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && isStart)
        {
            Player_Movement.Instance.MovePlayerTo(new Vector2(-4.5f, 1.5f));
            isStart = false;
        }
    }

    public void OnStart(int index, float time)
    {
        if (!isActive)
        {
            StartCoroutine(MoveSectorsSimultaneously(index, time));
            isActive = true;
        }
    }

    public void OnActive(int sectorPosIndex, float time)
    {
        StartCoroutine(MoveSectorsSimultaneously(sectorPosIndex, time));
    }
    
    private IEnumerator MoveSectorsSimultaneously(int index, float time)
    {
        StartCoroutine(MoveSector(lastSector, sectorPos[index + 1].position, time));
        yield return new WaitForSeconds(0.4f);
        midSectors[index].gameObject.SetActive(true);
    }

    private IEnumerator MoveSector(Transform sector, Vector2 targetPos, float moveDuration)
    {
        float moveSpeed = 5;
        float elapsedTime = 0f;
        Vector2 direction = (targetPos - (Vector2)sector.position).normalized; 
        float maxShakeIntensity = 0.5f;

        while (elapsedTime < moveDuration)
        {
            float shakeStrength = Mathf.SmoothStep(0f, maxShakeIntensity, elapsedTime / moveDuration);

            sector.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
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

    public void ChangeGridState()
    {
        GameObject player = Player_Movement.Instance.gameObject;
        Player_Steps.Instance.enemy.gameObject.SetActive(false);
        
        if (player != null)
        {
            Player_Movement.Instance.movePoint.position = playerTargetPos.position;
            player.transform.position = playerTargetPos.position;
            
            player.SetActive((true));
        }
        
        lastSector.position = firstPos;

        foreach (var sector in midSectors)
        {
            sector.gameObject.SetActive(false);
        }
    }
}
