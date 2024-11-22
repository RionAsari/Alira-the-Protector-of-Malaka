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
        SaveCurrentScene(); // Simpan scene terakhir sebelum keluar game
        Application.Quit(); // Menutup aplikasi
    }

    public void GoToMainMenu()
    {
        SaveCurrentScene(); // Simpan scene terakhir sebelum kembali ke menu utama
        SceneManager.LoadScene("Main Menu"); // Pindah ke scene Main Menu
    }

    // Function to save the current scene name
    void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name; // Mendapatkan nama scene yang sedang aktif
        PlayerPrefs.SetString("SavedScene", currentScene);         // Menyimpan nama scene
        PlayerPrefs.Save();                                        // Pastikan data disimpan
    }
}
