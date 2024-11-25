using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;          // Reference to the pause menu UI
    public GameObject optionsMenuUI;        // Reference to the options menu UI
    private bool isPaused = false;
    private PlayerController playerController;

    public AudioSource audioSource;         // Reference to AudioSource component
    public AudioClip pauseSound;            // The sound to play when pausing
    public AudioClip buttonClickSound;      // Sound to play when UI button is clicked

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>(); // Get reference to PlayerController
    }

    void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);         // Show pause menu
        optionsMenuUI.SetActive(false);      // Ensure options menu is hidden
        playerController.TogglePause();      // Disable player interactions
        Time.timeScale = 0f;                 // Pause the game

        // Play the pause sound when the pause menu is shown
        if (audioSource != null && pauseSound != null)
        {
            audioSource.PlayOneShot(pauseSound);  // Play the pause sound effect
        }
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);        // Hide pause menu
        optionsMenuUI.SetActive(false);      // Hide options menu if open
        playerController.TogglePause();      // Enable player interactions
        Time.timeScale = 1f;                 // Resume the game

        // Play button click sound
        PlayButtonClickSound();
    }

    public void Options()
    {
        optionsMenuUI.SetActive(true);       // Show options menu
        pauseMenuUI.SetActive(false);        // Hide pause menu

        // Play button click sound
        PlayButtonClickSound();
    }

    public void BackToPauseMenu()
    {
        optionsMenuUI.SetActive(false);      // Hide options menu
        pauseMenuUI.SetActive(true);         // Show pause menu

        // Play button click sound
        PlayButtonClickSound();
    }

    public void Restart()
    {
        Time.timeScale = 1f;                 // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene

        // Play button click sound
        PlayButtonClickSound();
    }

    public void QuitGame()
    {
        SaveCurrentScene();  // Save the current scene before quitting the game
        Time.timeScale = 1f; // Reset time scale
        Application.Quit();  // Quit application (won't work in editor)

        // Play button click sound
        PlayButtonClickSound();
    }

    public void GoToStartMenu()
    {
        SaveCurrentScene();  // Save the current scene before transitioning to Main Menu
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene("Main Menu"); // Load the main menu scene

        // Play button click sound
        PlayButtonClickSound();
    }

    // Function to save the current scene name
    void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name; // Get the name of the active scene
        PlayerPrefs.SetString("SavedScene", currentScene);         // Save the scene name
        PlayerPrefs.Save();                                        // Ensure the data is saved
    }

    // Play the button click sound effect
    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);  // Play button click sound
        }
    }
}
