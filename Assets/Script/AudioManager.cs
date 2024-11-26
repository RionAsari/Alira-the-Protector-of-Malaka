using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource SFXSource;
    [SerializeField] private AudioSource chargeSFXSource;

    [Header("Audio Clips")]
    public AudioClip moving;
    public AudioClip healSound;
    public AudioClip bowChargeSound;
    public AudioClip bowReleaseSound;
    public AudioClip dashSound;  // Added for dash sound

    public static AudioManager instance;

    private void Awake()
    {
        // Ensure there is only one instance of AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (instance != this)
        {
            // Destroy duplicates
            
        }

        // Add scene change listener
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy the AudioManager if we're not in the gameplay scenes
        if (scene.name == "TutorialLevel" || 
            scene.name == "LevelOne" || 
            scene.name == "LevelTwo" || 
            scene.name == "Level3" || 
            scene.name == "Level4")
        {
            DontDestroyOnLoad(gameObject);  // Keep in the gameplay scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy in non-interactive scenes
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayHealSound()
    {
        if (SFXSource != null && healSound != null)
        {
            SFXSource.PlayOneShot(healSound);
        }
    }

    public void PlayBowChargeSound()
    {
        if (chargeSFXSource != null && bowChargeSound != null)
        {
            chargeSFXSource.PlayOneShot(bowChargeSound);
        }
    }

    public void PlayBowReleaseSound()
    {
        if (SFXSource != null && bowReleaseSound != null)
        {
            SFXSource.PlayOneShot(bowReleaseSound);
        }
    }

    public void PlayDashSound()  // Added method to play dash sound
    {
        if (SFXSource != null && dashSound != null)
        {
            SFXSource.PlayOneShot(dashSound);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (SFXSource != null) SFXSource.volume = volume;
        if (chargeSFXSource != null) chargeSFXSource.volume = volume;
    }
}
