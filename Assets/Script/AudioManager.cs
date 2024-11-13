using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;  // Music for the main menu
    public AudioClip gameMusic;      // Music for the game
    public AudioClip moving;         // Sound effects

    private static AudioManager instance;

    // Make sure there's only one instance of AudioManager across scenes
    private void Awake()
    {
        // Ensure this AudioManager persists between scenes
        if (instance != null)
        {
  
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Prevent destruction on scene load
        }

        // Add listener for scene loading to change music
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        musicSource.Play();  // Start music from the beginning
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Change background music based on the scene name
        switch (scene.name)
        {
            case "MainMenu":  // Replace with your actual main menu scene name
                SetBackgroundMusic(mainMenuMusic);
                break;

            case "TutorialLevel":  // Replace with your actual game scene name
                SetBackgroundMusic(gameMusic);
                break;

            // Add more cases for different scenes if needed
            default:
                SetBackgroundMusic(mainMenuMusic);  // Default to main menu music
                break;
        }
    }

    // Set the background music based on the selected clip
    private void SetBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}