using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum SoundType
{
    Footsteps,
    Gritting,
    Twilight,
    Shake,
    Warning,
    Hurt
}

[ExecuteInEditMode]
public class Audio_Manager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    
    [Header("SFX")]
    [SerializeField] private SoundData[] sounds;
    private static Audio_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.sounds[(int)sound].Sounds;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        instance.sfxSource.PlayOneShot(randomClip, volume);
    }
    
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref sounds, names.Length);

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].name = names[i];
        }
    }
#endif
    
}

[Serializable]
public struct SoundData
{
    public AudioClip[] Sounds {get => sounds;}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
