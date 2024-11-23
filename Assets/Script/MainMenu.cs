using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject loadMenuPanel;
    public Button loadButton;
    public Transform levelListParent;
    public Button levelButtonPrefab;
    public TMP_Text warningText;
    public TMP_Text warningText2;
    public AudioClip buttonClickSound; // Tambahkan referensi audio
    private AudioSource audioSource; // Untuk memutar suara

    void Start()
    {
        // Inisialisasi AudioSource
        audioSource = GetComponent<AudioSource>();
        CheckLevelCompletion();
        DisplaySavedGames();

        // Tambahkan suara ke semua tombol
        AssignButtonSounds();
    }

    // Fungsi untuk memutar suara klik
    void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Fungsi untuk menambahkan suara klik ke semua tombol
    void AssignButtonSounds()
    {
        foreach (Button button in FindObjectsOfType<Button>())
        {
            button.onClick.AddListener(PlayButtonClickSound);
        }
    }

    public void StartGame()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("Intro");
    }

    public void ContinueGame()
    {
        PlayButtonClickSound();
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

    public void OpenSettings()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        PlayButtonClickSound();
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        PlayButtonClickSound();
        Debug.Log("Game Quit");
        Application.Quit();
    }

    void CheckLevelCompletion()
    {
        if (PlayerPrefs.GetInt("LevelCompleted", 0) > 0)
        {
            warningText.text = "";
            warningText2.text = "Load Game is available";
        }
        else
        {
            warningText.text = "You must complete all levels first.";
            warningText2.text = "";
        }
    }

    public void ShowLoadMenu()
    {
        PlayButtonClickSound();
        mainMenuPanel.SetActive(false);
        loadMenuPanel.SetActive(true);
    }

    void DisplaySavedGames()
    {
        string[] savedScenes = { "Level1", "Level2", "Level3", "Level4" };

        foreach (var sceneName in savedScenes)
        {
            Button newButton = Instantiate(levelButtonPrefab, levelListParent);
            newButton.GetComponentInChildren<TMP_Text>().text = sceneName;
            newButton.onClick.AddListener(() => LoadLevel(sceneName));
        }
    }

    void LoadLevel(string sceneName)
    {
        PlayButtonClickSound();
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMainMenu()
    {
        PlayButtonClickSound();
        loadMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
