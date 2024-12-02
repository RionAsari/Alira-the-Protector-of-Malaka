using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;          // Musik untuk main menu
    public AudioClip gameMusic;              // Musik untuk game
    public AudioClip tutorialLevelMusic;    // Musik untuk tutorial level
    public AudioClip introMusic;            // Musik untuk intro
    public AudioClip cutSceneMusic;         // Musik untuk cutscene
    public AudioClip levelOneMusic;         // Musik untuk level one
    public AudioClip levelTwoMusic;         // Musik untuk level two
    public AudioClip level3Music;           // Musik untuk level three
    public AudioClip level4Music;           // Musik untuk level four
    public AudioClip endingCutsceneMusic;   // Musik untuk ending cutscene
    public AudioClip creditMusic;           // Musik untuk credit

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

        // Register to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Debugging: Check the current scene name at the start
        Debug.Log("Starting in scene: " + SceneManager.GetActiveScene().name);
        
        SetBackgroundMusicBasedOnScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debugging: Output the loaded scene name
        Debug.Log("Scene Loaded: " + scene.name);

        // Change the background music when a new scene is loaded
        SetBackgroundMusicBasedOnScene(scene.name);
    }

    private void SetBackgroundMusicBasedOnScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        // Debugging: Show the exact scene name received
        Debug.Log("Received Scene Name: " + sceneName);

        // Tentukan musik yang sesuai berdasarkan nama scene
        switch (sceneName)
        {
            case "Main Menu":
                clipToPlay = mainMenuMusic;
                break;

            case "Intro":
                clipToPlay = introMusic;
                break;

            case "CutScene":
                clipToPlay = cutSceneMusic;
                break;

            case "TutorialLevel":
                clipToPlay = tutorialLevelMusic;
                break;

            case "LevelOne":
                clipToPlay = levelOneMusic;
                break;

            case "LevelTwo":
                clipToPlay = levelTwoMusic;
                break;

            case "Level3":
                clipToPlay = level3Music;
                break;

            case "Level4":
                clipToPlay = level4Music;
                break;

            case "Ending Cutscene":
                clipToPlay = endingCutsceneMusic;
                break;

            case "Credit":
                clipToPlay = creditMusic;
                break;

            case "GameScene":
                clipToPlay = gameMusic;
                break;

            default:
                clipToPlay = mainMenuMusic;  // Default ke main menu music
                break;
        }

        // Debugging: Check which music clip has been selected
        Debug.Log("Selected Music Clip: " + (clipToPlay != null ? clipToPlay.name : "None"));

        // Jika musik yang dipilih berbeda dengan yang sedang diputar, ubah musik
        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.Stop();  // Hentikan musik yang sedang diputar
            musicSource.clip = clipToPlay;  // Ganti musik
            musicSource.Play();  // Putar musik yang baru
            Debug.Log("Now Playing: " + clipToPlay.name);  // Debugging: Konfirmasi musik yang baru diputar
        }
    }

    private void OnDestroy()
    {
        // Unregister dari event scene loaded untuk mencegah memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
