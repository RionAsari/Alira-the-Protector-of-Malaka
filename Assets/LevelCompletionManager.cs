using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    public string nextSceneName = "MainMenu"; // Nama scene berikutnya (MainMenu)
    private LightGrunt[] lightGrunts;
    private MiddleBot[] middleBots;

    void Start()
    {
        // Mendapatkan semua LightGrunt dan MiddleBot di scene
        lightGrunts = FindObjectsOfType<LightGrunt>();
        middleBots = FindObjectsOfType<MiddleBot>();
    }

    void Update()
    {
        // Cek apakah semua musuh telah dikalahkan
        if (AreAllEnemiesDefeated())
        {
            // Perbarui PlayerPrefs untuk menandai level selesai
            MarkLevelAsCompleted();

            // Pindah ke MainMenu
            LoadNextScene();
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        // Periksa semua LightGrunt
        foreach (var grunt in lightGrunts)
        {
            if (grunt != null && grunt.gameObject.activeInHierarchy)
                return false;
        }

        // Periksa semua MiddleBot
        foreach (var bot in middleBots)
        {
            if (bot != null && bot.gameObject.activeInHierarchy)
                return false;
        }

        return true; // Semua musuh telah dikalahkan
    }

    private void MarkLevelAsCompleted()
    {
        // Ambil jumlah level yang telah diselesaikan
        int levelsCompleted = PlayerPrefs.GetInt("LevelCompleted", 0);

        // Jika Level4 selesai, berarti semua level telah selesai
        if (SceneManager.GetActiveScene().name == "Level4" && levelsCompleted < 4)
        {
            PlayerPrefs.SetInt("LevelCompleted", 4); // Tandai semua level selesai
            PlayerPrefs.Save(); // Simpan perubahan ke disk
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
