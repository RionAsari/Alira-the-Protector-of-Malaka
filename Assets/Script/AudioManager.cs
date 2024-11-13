using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource chargeSFXSource; // Separate source for charging sound

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;  // Music for the main menu
    public AudioClip gameMusic;      // Music for the game
    public AudioClip moving;         // Sound effects
    public AudioClip healSound;      // Healing sound effect
    public AudioClip bowChargeSound; // Bow charge sound effect

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {

        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        musicSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                SetBackgroundMusic(mainMenuMusic);
                break;

            case "TutorialLevel":
                SetBackgroundMusic(gameMusic);
                break;

            default:
                SetBackgroundMusic(mainMenuMusic);
                break;
        }
    }

    private void SetBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayHealSound()
    {
        if (healSound != null)
        {
            SFXSource.PlayOneShot(healSound);
        }
    }

    public void PlayBowChargeSound()
    {
        if (bowChargeSound != null)
        {
            chargeSFXSource.PlayOneShot(bowChargeSound); // Play on dedicated charge source
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
        chargeSFXSource.volume = volume; // Set volume for both SFX and charge sources
    }
}
