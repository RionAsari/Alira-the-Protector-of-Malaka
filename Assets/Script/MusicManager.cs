using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;  // Musik untuk main menu
    public AudioClip gameMusic;      // Musik untuk game

    public static MusicManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Hancurkan instance duplikat
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Musik tetap ada saat scene berganti
        }
    }

    private void Start()
    {
        SetBackgroundMusic(mainMenuMusic);  // Set default music saat aplikasi dimulai
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ganti musik saat scene berganti
        switch (scene.name)
        {
            case "MainMenu":
                SetBackgroundMusic(mainMenuMusic);
                break;

            case "GameScene":
                SetBackgroundMusic(gameMusic);
                break;

            default:
                SetBackgroundMusic(mainMenuMusic);  // Musik default
                break;
        }
    }

    private void SetBackgroundMusic(AudioClip clip)
    {
        if (musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
}
