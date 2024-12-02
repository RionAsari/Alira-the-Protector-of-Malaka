using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For PointerEventData and EventSystem
using System.Collections.Generic; // For List<>

public class MainMenuScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject loadMenuPanel;
    public Button loadButton;
    public Button levelButton1; // Tombol 1 untuk "TutorialLevel"
    public Button levelButton2; // Tombol 2 untuk "LevelOne"
    public Button levelButton3; // Tombol 3 untuk "LevelTwo"
    public Button levelButton4; // Tombol 4 untuk "Level3"
    public Button levelButton5; // Tombol 5 untuk "Level4"
    public Button backButton;  // Button in the load menu to go back
    public TMP_Text warningText;
    public TMP_Text warningText2;
    public AudioClip buttonClickSound;
    private AudioSource audioSource;
    private bool showWarning = false; // Menandai apakah peringatan sedang ditampilkan

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CheckLevelCompletion();
        AssignButtonListeners(); // Assign listener untuk setiap tombol
        AssignButtonSounds();

        // Assign listener for the back button to return to the main menu
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
    }

    void Update()
    {
        if (showWarning)
        {
            if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                Debug.Log("Input detected, hiding warning.");
                HideWarning();
            }
        }
    }

    bool IsPointerOverUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0; // If there are any UI elements under the raycast, return true
    }

    void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    void AssignButtonListeners()
    {
        // Menambahkan listener ke masing-masing tombol untuk memuat scene
        levelButton1.onClick.AddListener(() => LoadLevel("TutorialLevel"));
        levelButton2.onClick.AddListener(() => LoadLevel("LevelOne"));
        levelButton3.onClick.AddListener(() => LoadLevel("LevelTwo"));
        levelButton4.onClick.AddListener(() => LoadLevel("Level3"));
        levelButton5.onClick.AddListener(() => LoadLevel("Level4"));
    }

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
        Debug.Log("CheckLevelCompletion called");
        int levelCompleted = PlayerPrefs.GetInt("LevelCompleted", 0);
        Debug.Log("Level completed: " + levelCompleted);

        if (levelCompleted < 4)
        {
            warningText.text = "Level Selection is not yet unlocked!";
            warningText2.text = "";
            showWarning = true;

            warningText.gameObject.SetActive(true);
            warningText2.gameObject.SetActive(false);

            Debug.Log("showWarning set to true, first warning displayed.");
        }
        else
        {
            warningText.text = "";
            warningText2.text = "Load Game is available";
            showWarning = true;

            warningText.gameObject.SetActive(false);
            warningText2.gameObject.SetActive(true);

            Debug.Log("showWarning set to true, second warning displayed.");
        }
    }

    void HideWarning()
    {
        Debug.Log("Hiding warning text");
        // Hide warning text and reset flag
        warningText.text = "";
        warningText2.text = "";
        showWarning = false;

        // Set both warning texts to inactive
        warningText.gameObject.SetActive(false);
        warningText2.gameObject.SetActive(false);
    }

    public void ShowLoadMenu()
    {
        PlayButtonClickSound();

        if (PlayerPrefs.GetInt("LevelCompleted", 0) < 4) // Jika level belum selesai
        {
            warningText.text = "Level Selection is not yet unlocked!";
            showWarning = true; // Tampilkan peringatan
        }
        else
        {
            mainMenuPanel.SetActive(false);
            loadMenuPanel.SetActive(true);
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
