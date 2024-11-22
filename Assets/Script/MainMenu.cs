using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Pastikan kita menggunakan namespace TextMeshPro
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject loadMenuPanel; // Panel untuk memilih level
    public Button loadButton; // Button load game
    public Transform levelListParent; // Parent object untuk menampilkan tombol level
    public Button levelButtonPrefab; // Prefab tombol level yang akan diinstansiasi
    public TMP_Text warningText; // Teks peringatan untuk Load Game
    public TMP_Text warningText2; // Teks peringatan untuk "Load Game is available only after completing all levels"

    void Start()
    {
        CheckLevelCompletion();
        DisplaySavedGames();
    }

    // Fungsi untuk memulai game baru
    public void StartGame()
    {
        SceneManager.LoadScene("Intro"); // Ganti "Intro" dengan nama scene yang ingin dibuka
    }

    // Fungsi untuk melanjutkan permainan
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string savedScene = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.Log("No saved game found.");
        }
    }

    // Fungsi untuk membuka pengaturan
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    // Fungsi untuk menutup pengaturan
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    // Fungsi untuk memeriksa apakah level sudah selesai atau belum
    void CheckLevelCompletion()
    {
        if (PlayerPrefs.GetInt("LevelCompleted", 0) > 0)
        {
            warningText.text = ""; // Menyembunyikan peringatan jika level sudah selesai
            warningText2.text = "Load Game is available"; // Memberitahu jika Load Game sudah tersedia
        }
        else
        {
            warningText.text = "You must complete all levels first."; // Menampilkan peringatan untuk menyelesaikan semua level
            warningText2.text = ""; // Menyembunyikan peringatan load game jika belum selesai
        }
    }

    // Fungsi untuk menampilkan panel load game
    public void ShowLoadMenu()
    {
        mainMenuPanel.SetActive(false); // Menyembunyikan main menu
        loadMenuPanel.SetActive(true); // Menampilkan panel load game
    }

    // Fungsi untuk menampilkan daftar level yang bisa dimuat
    void DisplaySavedGames()
    {
        // Contoh level yang disimpan
        string[] savedScenes = { "Level1", "Level2", "Level3", "Level4" }; // Ganti dengan nama level yang ada di game

        foreach (var sceneName in savedScenes)
        {
            Button newButton = Instantiate(levelButtonPrefab, levelListParent); // Membuat tombol baru untuk setiap scene
            newButton.GetComponentInChildren<TMP_Text>().text = sceneName; // Menampilkan nama scene pada tombol
            newButton.onClick.AddListener(() => LoadLevel(sceneName)); // Menambahkan listener untuk memuat scene ketika tombol diklik
        }
    }

    // Fungsi untuk memuat level yang dipilih
    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Memuat scene sesuai yang dipilih
    }

    // Fungsi untuk kembali ke menu utama setelah memilih level
    public void BackToMainMenu()
    {
        loadMenuPanel.SetActive(false); // Menyembunyikan load menu
        mainMenuPanel.SetActive(true); // Menampilkan main menu
    }
}
