using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Steps : MonoBehaviour
{
    public static Player_Steps Instance { get; private set; }
    
    [Header("Transforms")]
    public Transform player;
    public Transform enemy;
    
    [Header("Stats")] 
    public bool invincible;
    public bool stealth;
    
    [Header("UI")] 
    public int stepCount = 20;
    public int playerLives = 2;
    
    private int maxStepCount;
    public TextMeshProUGUI stepCountText;
    
    private List<IEnemy> enemies = new List<IEnemy>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = Player_Movement.Instance.transform;
        }
        
        maxStepCount = stepCount;
        UpdateStepCountText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NotifyEnemiesOfPlayerMove();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ActivateFog();
        }
    }

    public void RegisterEnemy(IEnemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }
    
    public void DeregisterEnemy(IEnemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    
    public void NotifyEnemiesOfPlayerMove()
    {
        if (!stealth)
        {
            IncrementMoveCount();
        
            foreach (var enemy in enemies)
            {
                enemy.OnPlayerMoved();  
            }
        }
    }
    
    private void IncrementMoveCount()
    {
        if (invincible || playerLives == 0) return;
        stepCount--;  
        
        if (stepCount <= 1)
        {
            if (!UI_Inventory.Instance.IsInventoryEmpty())
            {
                UI_Inventory.Instance.RemoveItem();
            }
        }
        
        if (stepCount <= 0 )
        {
            playerLives--;
            
            FOD_Agent agent = player.GetComponent<FOD_Agent>();
            float radius = agent.sightRange;
            agent.ChangeRadiusValue(radius - 8);
            
            if (playerLives <= 0)
            {
                ActivateCentipedeChase();
                
                stepCountText.text = "Light - 00, (00)";  
                return;
            }
            
            stepCount += maxStepCount / 2;
            maxStepCount = stepCount;
        }
        
        UpdateStepCountText();
    }

    public void AddSteps(int steps)
    {
        stepCount += steps;
        UpdateStepCountText();
    }
    
    private void UpdateStepCountText()
    {
        if (stepCountText != null)
        {
            stepCountText.text = $"Light - {stepCount:00}, ({maxStepCount:00})";  
        }
    }
    
    private void ActivateCentipedeChase()
    {
        if(enemy != null) enemy.gameObject.SetActive(true);
    }

    public void ActivateSpiderChase()
    {
        if (enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy is Enemy_Spider_Movement spiderMovement)
                {
                    spiderMovement.isChasingPlayer = true;
                }
            }
        }
    }

    private void ActivateFog()
    {
        FOD_Manager manager = FindObjectOfType<FOD_Manager>(true);
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            manager.StartCoroutine(manager.EnableWithDelay(0.8f));
        }
    }
}
