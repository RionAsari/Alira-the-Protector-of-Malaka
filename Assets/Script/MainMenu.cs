using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Add this for Slider and UI components

public class MainMenuScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public Slider volumeSlider; // Reference to the slider for volume control

    // Set initial volume on start (you can set this from PlayerPrefs if you want to save the volume setting)
    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            AudioListener.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
        else
        {
            AudioListener.volume = 1f; // Default volume
            volumeSlider.value = 1f; // Default slider position
        }

        // Add listener for slider value change
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Function to change volume when slider value is adjusted
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume); // Save volume setting for next time
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TutorialLevel"); // Ganti "TutorialLevel" dengan nama scene yang ingin dibuka
    }

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

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
