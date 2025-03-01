using System.Collections.Generic;
using FODMapping;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Steps : MonoBehaviour
{
    public static Player_Steps Instance { get; private set; }
    
    [Header("Transforms")]
    public Transform player;
    public Transform enemy;
    
    [Header("Stats")] 
    public bool isInvulnerable;
    public bool isStealth;
    
    [Header("UI")] 
    public int stepCount = 20;
    public int playerLives = 2;
    
    public Image healthSlot;
    public Sprite damagedHealthSlot;
    
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
        if (!isStealth)
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
        if (isInvulnerable || playerLives == 0) return;
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
            
            healthSlot.sprite = damagedHealthSlot;
            
            FOD_Agent agent = player.GetComponent<FOD_Agent>();
            float radius = agent.sightRange;
            agent.ChangeRadiusValue(radius - 8);
            
            if (playerLives <= 0)
            {
                ActivateCentipedeChase();
                healthSlot.enabled = false;
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
        MusicManager.instance.PlayMusic(MusicManager.instance.chaseMusic);
        Audio_Manager.PlaySound(SoundType.Warning);
        
        if(enemy != null) enemy.gameObject.SetActive(true);
    }

    public void ActivateSpiderChase()
    {
        if (enemies.Count > 0)
        {
            MusicManager.instance.PlayMusic(MusicManager.instance.chaseMusic);
            Audio_Manager.PlaySound(SoundType.Warning);
            
            foreach (var enemy in enemies)
            {
                if (enemy is Enemy_Spider_Movement spiderMovement)
                {
                    spiderMovement.isChasingPlayer = true;
                    spiderMovement.spriteMask.enabled = true;
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
    
    public void SetInvulnerability(bool state)
    {
        isInvulnerable = state;

        if (isInvulnerable)
        {
            stepCount = maxStepCount;
            UpdateStepCountText();
        }
    }
}
