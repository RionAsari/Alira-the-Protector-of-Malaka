using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;          // Reference to the pause menu UI
    public GameObject optionsMenuUI;        // Reference to the options menu UI
    private bool isPaused = false;
    private PlayerController playerController;

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
        playerController.TogglePause();      // Disable player interactions
        Time.timeScale = 0f;                 // Pause the game
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);        // Hide pause menu
        optionsMenuUI.SetActive(false);      // Hide options menu if open
        playerController.TogglePause();      // Enable player interactions
        Time.timeScale = 1f;                 // Resume the game
    }

    public void Options()
    {
        optionsMenuUI.SetActive(true);       // Show options menu
        pauseMenuUI.SetActive(false);        // Hide pause menu
    }

    public void Restart()
    {
        Time.timeScale = 1f;                 // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;                 // Reset time scale
        Application.Quit();                  // Quit application (won't work in editor)
    }

    public void GoToStartMenu()
    {
        Time.timeScale = 1f;                 // Reset time scale
        SceneManager.LoadScene("Main Menu"); // Replace with the appropriate scene name
    }
}
