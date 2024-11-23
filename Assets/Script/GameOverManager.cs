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

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true); // Show the GameOver menu
            // Set other texts and buttons to be invisible initially
            SetTextsAndButtonsInvisible();
            StartCoroutine(FadeInGameOverElements()); // Fade in both Game Over text and button together
        }
    }

    private void SetTextsAndButtonsInvisible()
    {
        // Set all other texts to be fully transparent at the beginning
        foreach (TMP_Text text in otherTexts)
        {
            Color textColor = text.color;
            textColor.a = 0f;
            text.color = textColor;
        }

        // Set the Game Over button to be fully transparent at the beginning
        Color buttonColor = gameOverButton.image.color;
        buttonColor.a = 0f;
        gameOverButton.image.color = buttonColor;

        // If the Game Over button has text, set it to be transparent as well
        if (gameOverButton.GetComponentInChildren<TMP_Text>())
        {
            TMP_Text buttonText = gameOverButton.GetComponentInChildren<TMP_Text>();
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0f);
        }

        // Set all other buttons to be fully transparent at the beginning
        foreach (Button button in otherButtons)
        {
            buttonColor = button.image.color;
            buttonColor.a = 0f;
            button.image.color = buttonColor;

            // If buttons have text, set their alpha to 0 as well
            if (button.GetComponentInChildren<TMP_Text>())
            {
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0f);
            }
        }
    }

    private IEnumerator FadeInGameOverElements()
    {
        float elapsedTime = 0f;

        // Fade in both the Game Over text and button together
        while (elapsedTime < fadeDuration)
        {
            float alphaValue = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            // Fade in Game Over text
            Color textColor = gameOverText.color;
            textColor.a = alphaValue;
            gameOverText.color = textColor;

            // Fade in Game Over button
            Color buttonColor = gameOverButton.image.color;
            buttonColor.a = alphaValue;
            gameOverButton.image.color = buttonColor;

            // If the Game Over button has text, fade it in as well
            if (gameOverButton.GetComponentInChildren<TMP_Text>())
            {
                TMP_Text buttonText = gameOverButton.GetComponentInChildren<TMP_Text>();
                buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, alphaValue);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure both elements are fully visible after fading
        Color finalTextColor = gameOverText.color;
        finalTextColor.a = 1f;
        gameOverText.color = finalTextColor;

        Color finalButtonColor = gameOverButton.image.color;
        finalButtonColor.a = 1f;
        gameOverButton.image.color = finalButtonColor;

        // Ensure button text is fully visible
        if (gameOverButton.GetComponentInChildren<TMP_Text>())
        {
            TMP_Text buttonText = gameOverButton.GetComponentInChildren<TMP_Text>();
            buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f);
        }

        // After Game Over text and button fade in, fade in the other buttons
        StartCoroutine(FadeInOtherButtons());
    }

    private IEnumerator FadeInOtherButtons()
    {
        // Fade in all other buttons (Retry, Quit, Back to Main Menu)
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Fade other buttons and their text
            foreach (Button button in otherButtons)
            {
                Color buttonColor = button.image.color;
                buttonColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                button.image.color = buttonColor;

                // Fade button text
                if (button.GetComponentInChildren<TMP_Text>())
                {
                    TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                    buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration));
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is 1 (fully visible) for all buttons
        foreach (Button button in otherButtons)
        {
            Color buttonColor = button.image.color;
            buttonColor.a = 1f;
            button.image.color = buttonColor;

            // Ensure button text is fully visible
            if (button.GetComponentInChildren<TMP_Text>())
            {
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f);
            }
        }
    }

    public void Retry()
    {
        // Logic to restart the game
        Debug.Log("Game Restarted");
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        SaveCurrentScene(); // Save the last scene before quitting
        Application.Quit(); // Close the application
    }

    public void GoToMainMenu()
    {
        SaveCurrentScene(); // Save the last scene before returning to the main menu
        SceneManager.LoadScene("Main Menu"); // Go to the Main Menu
    }

    // Function to save the current scene name
    void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name; // Get the active scene name
        PlayerPrefs.SetString("SavedScene", currentScene); // Save the scene name
        PlayerPrefs.Save(); // Ensure the data is saved
    }
}
