using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("=== AUDIO SOURCES ===")]
    public AudioSource musicSource;
    public AudioSource vfxSource;
    
    [Header("=== BACKGROUND MUSIC ===")]
    public AudioClip bgMusic;

    [Header("=== SOUND EFFECTS (VFX) ===")]
    public AudioClip slash1Sound;
    public AudioClip slash2Sound;
    public AudioClip healSound;
    public AudioClip bossMadSound;
    public AudioClip hurtSound;
    public AudioClip sheepHitSound;
    public AudioClip enemyHitSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        PlayMusic(bgMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if(clip != null &&  musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayVFX(AudioClip clip)
    {
        if(clip != null &&vfxSource != null)
        {
            vfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlayVFXWithDuration(AudioClip clip, float duration)
    {
        if (clip != null && vfxSource != null)
        {
            vfxSource.clip = clip;
            vfxSource.Play(); 
        
            // Gọi Coroutine
            StartCoroutine(StopAudioAfterTime(vfxSource, duration));
        }
    }

    private IEnumerator StopAudioAfterTime(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (source != null)
        {
            source.Stop(); 
        }
    }
}
