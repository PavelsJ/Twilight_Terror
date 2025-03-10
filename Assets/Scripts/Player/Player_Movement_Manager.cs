using System.Collections.Generic;
using FODMapping;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Movement_Manager : MonoBehaviour
{
    public static Player_Movement_Manager Instance { get; private set; }
    
    [Header("Transforms")]
    public Transform player;
    public Transform enemy;
    
    private Vector3 lastMoveDirection = Vector3.zero;
    
    [Header("Stats")] 
    public bool isInvulnerable;
    public bool isStealth;
    
    [Header("Compounds")]
    private Player_Stats stats;
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
        
        stats = GetComponent<Player_Stats>();
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
            stats.UpdateMoveCount();
        
            foreach (var enemy in enemies)
            {
                enemy.OnPlayerMoved();  
            }
        }
    }
    
    public void AddSteps(int amount)
    {
       stats.AddSteps(amount);
    }

    public void ActivateCentipedeChase()
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
    
    public void SetPlayerMoveDirection(Vector3 direction)
    {
        lastMoveDirection = direction;
    }

    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
    
    public void SetInvulnerability(bool state)
    {
        isInvulnerable = state;

        if (isInvulnerable)
        {
            stats.SetMaxSteps();
        }
    }
}
