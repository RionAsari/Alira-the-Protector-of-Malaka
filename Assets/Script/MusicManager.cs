using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;  // Musik untuk main menu
    public AudioClip gameMusic;      // Musik untuk game
    public AudioClip tutorialLevelMusic;  // Musik untuk tutorial level

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

        // Determine which music clip to play based on the scene name
        switch (sceneName)
        {
            case "Main Menu":  // Ensure that "Main Menu" with space is used here
                clipToPlay = mainMenuMusic;
                break;

            case "GameScene":
                clipToPlay = gameMusic;
                break;

            case "TutorialLevel":  // Added condition for the Tutorial Level scene
                clipToPlay = tutorialLevelMusic;
                break;

            default:
                clipToPlay = mainMenuMusic;  // Default to main menu music
                break;
        }

        // Debugging: Check which music clip has been selected
        Debug.Log("Selected Music Clip: " + (clipToPlay != null ? clipToPlay.name : "None"));

        // If the selected music is not the same as the current one, change it
        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.Stop();  // Stop any currently playing music
            musicSource.clip = clipToPlay;  // Assign the new music
            musicSource.Play();  // Play the new music
            Debug.Log("Now Playing: " + clipToPlay.name);  // Debugging: Confirm the new music is playing
        }
    }

    private void OnDestroy()
    {
        // Unregister from the scene loaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
