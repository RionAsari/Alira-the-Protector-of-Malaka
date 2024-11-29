using UnityEngine;
using UnityEngine.SceneManagement; // For Scene Management
using TMPro; // For TMP_Text component (TextMesh Pro)
using UnityEngine.UI; // For Button component
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenu; // GameOver UI
    public TMP_Text gameOverText; // GameOver text element
    public TMP_Text[] otherTexts; // Array to reference other texts like Retry, Quit, Back to Main Menu
    public Button gameOverButton; // Game Over button (separate from other buttons)
    public Button[] otherButtons; // Array to reference other buttons (Retry, Quit, Back to Main Menu buttons)
    public float fadeDuration = 1f; // Duration of the fade-in effect (1 second)

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip buttonSound; // Audio clip for button click

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true); // Show the GameOver menu
            SetTextsAndButtonsInvisible();
            StartCoroutine(FadeInGameOverElements()); // Fade in both Game Over text and button together
        }
    }

    private void SetTextsAndButtonsInvisible()
    {
        // Set Game Over button to be fully transparent at the beginning
        Color buttonColor = gameOverButton.image.color;
        buttonColor.a = 0f;
        gameOverButton.image.color = buttonColor;

        // Set all other buttons to be fully transparent at the beginning
        foreach (Button button in otherButtons)
        {
            buttonColor = button.image.color;
            buttonColor.a = 0f;
            button.image.color = buttonColor;
        }
    }

    private IEnumerator FadeInGameOverElements()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alphaValue = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            // Fade in Game Over button's image
            Color buttonColor = gameOverButton.image.color;
            buttonColor.a = alphaValue;
            gameOverButton.image.color = buttonColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure button is fully visible after fading
        Color finalButtonColor = gameOverButton.image.color;
        finalButtonColor.a = 1f;
        gameOverButton.image.color = finalButtonColor;

        // Fade in other buttons
        StartCoroutine(FadeInOtherButtons());
    }

    private IEnumerator FadeInOtherButtons()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            foreach (Button button in otherButtons)
            {
                Color buttonColor = button.image.color;
                buttonColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                button.image.color = buttonColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all buttons are fully visible
        foreach (Button button in otherButtons)
        {
            Color buttonColor = button.image.color;
            buttonColor.a = 1f;
            button.image.color = buttonColor;
        }
    }

    public void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound); // Play the button sound
        }
    }

    public void Retry()
    {
        PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        SaveCurrentScene();
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        PlayButtonSound();
        SaveCurrentScene();
        SceneManager.LoadScene("Main Menu");
    }

    // Function to save the current scene name
    void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("SavedScene", currentScene);
        PlayerPrefs.Save();
    }
}
