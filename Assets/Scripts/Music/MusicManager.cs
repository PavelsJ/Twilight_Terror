using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    public AudioClip mainThemeMusic;
    public AudioClip ambientMusic;
    public AudioClip chaseMusic;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneChange; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateMusic(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        UpdateMusic(scene, mode);
    }
    
    private void UpdateMusic(Scene scene, LoadSceneMode mode)
    {
        AudioClip newMusic = scene.buildIndex == 0 ? mainThemeMusic : ambientMusic;
        
        if (musicSource.clip != newMusic)
        {
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if(clip == musicSource.clip) return;
        
        musicSource.clip = clip;
        musicSource.Play();
    }
}
