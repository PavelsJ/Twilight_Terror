using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource.clip = ambientMusic;
        musicSource.Play();
    }
}
