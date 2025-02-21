using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities_Spawn : MonoBehaviour
{
    [Header("Player_Entities")] 
    public Transform playerTransform;
    
    [Header("Light_Entities")]
    public GameObject lightPrefab;

    [Header("Enemy_Entities")] 
    public Transform enemyCentipede;
    public GameObject enemyDungEater;

    [Header("Trap_Entities")] 
    public GameObject bearTrap;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
