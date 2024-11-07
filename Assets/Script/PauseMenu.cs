using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;       // Reference to the panel or pause menu UI
    private bool isPaused = false;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>(); // Mendapatkan referensi ke PlayerController
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
        pauseMenuUI.SetActive(true);         // Menampilkan menu pause
        playerController.TogglePause();     // Menonaktifkan interaksi pada PlayerController
        Time.timeScale = 0f;                // Menjeda permainan
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);        // Menyembunyikan menu pause
        playerController.TogglePause();     // Melanjutkan interaksi pada PlayerController
        Time.timeScale = 1f;                // Melanjutkan permainan
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
        SceneManager.LoadScene("Main Menu"); // Ganti dengan nama scene yang sesuai
    }
}
