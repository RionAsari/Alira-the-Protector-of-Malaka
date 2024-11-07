using UnityEngine;
using UnityEngine.SceneManagement; // Untuk Scene Management

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenu; // GameOver UI

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true); // Menampilkan menu Game Over
        }
    }

    public void Retry()
    {
        // Logika untuk memulai ulang permainan
        Debug.Log("Game Restarted");
        // Misalnya, me-reload scene saat ini
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene saat ini
    }

    public void QuitGame()
    {
        Application.Quit(); // Menutup aplikasi
    }

    public void GoToMainMenu()
    {
        // Ganti "MainMenu" dengan nama scene Main Menu Anda
        SceneManager.LoadScene("Main Menu"); // Pindah ke scene Main Menu
    }
}
