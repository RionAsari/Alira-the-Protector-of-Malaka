using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    public string nextSceneName = "EndingCutscene"; // Name of the next scene (EndingCutscene)
    private LightGrunt[] lightGrunts;
    private MiddleBot[] middleBots;
    private EndingSceneTransitionManager transitionManager;

    void Start()
    {
        // Get all instances of LightGrunt and MiddleBot in the scene
        lightGrunts = FindObjectsOfType<LightGrunt>();
        middleBots = FindObjectsOfType<MiddleBot>();
        
        // Get the EndingSceneTransitionManager instance
        transitionManager = FindObjectOfType<EndingSceneTransitionManager>();
    }

    void Update()
    {
        // Check if all enemies are defeated
        if (AreAllEnemiesDefeated())
        {
            // Update PlayerPrefs to mark the level as completed
            MarkLevelAsCompleted();

            // Trigger the fade out and scene change
            if (transitionManager != null)
            {
                transitionManager.LoadSceneWithFade(nextSceneName);
            }
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        // Check if all LightGrunt enemies are defeated
        foreach (var grunt in lightGrunts)
        {
            if (grunt != null && grunt.gameObject.activeInHierarchy)
                return false; // A grunt is still active
        }

        // Check if all MiddleBot enemies are defeated
        foreach (var bot in middleBots)
        {
            if (bot != null && bot.gameObject.activeInHierarchy)
                return false; // A bot is still active
        }

        return true; // All enemies are defeated
    }

    private void MarkLevelAsCompleted()
    {
        // Get the current level from the scene name
        string currentLevel = SceneManager.GetActiveScene().name;
        
        // Mark level completion based on the current scene name
        if (currentLevel == "Level4")
        {
            int levelsCompleted = PlayerPrefs.GetInt("LevelCompleted", 0);
            if (levelsCompleted < 4)
            {
                PlayerPrefs.SetInt("LevelCompleted", 4); // Mark all levels as completed
                PlayerPrefs.Save(); // Save the progress
            }
        }
    }
}
