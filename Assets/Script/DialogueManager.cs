using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // TextMeshProUGUI component for the dialogue
    public Image background;  // Image UI for background (Black for unconscious state)
    public float textSpeed = 0.05f; // Speed of text reveal
    public GameObject player;  // Reference to the player (to control movement)
    public Animator wakingUpAnimator; // Reference to the Animator for Alira's waking-up animation

    private bool isDialogueActive = false;
    private bool canSkip = false;

    void Start()
    {
        // Start the opening sequence immediately
        StartCoroutine(PlayOpeningSequence());
    }

    void Update()
    {
        // Allow the player to skip text when dialogue is active and they press a button
        if (isDialogueActive && canSkip && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            StopAllCoroutines(); // Stop the current text display
            dialogueText.text = dialogueText.text; // Finish displaying the text
            canSkip = false; // Disable skipping once the text is fully displayed
        }
    }

    public IEnumerator PlayOpeningSequence()
    {
        // Start with the black background for the intro
        background.color = Color.black;
        yield return StartCoroutine(DisplayText("In the dawn of the 21st century after the Great War, there is a girl who is living with her family peacefully deep in the forest living a secluded life."));

        // Transition to the next background (from black to the first scene)
        yield return StartCoroutine(DisplayText("But all of it shattered when an army of iron beasts invaded her home and destroyed everything in their path"));
        
        // More dialogue
        yield return StartCoroutine(DisplayText("The girl is now unconscious after being shot by one of the iron beasts."));
        
        // Transition to black screen (unconscious state)
        yield return StartCoroutine(TransitionToBlackScreen());

        // After a few moments, "3 hours later..."
        yield return StartCoroutine(DisplayText("3 hours later…"));

        // Trigger waking-up animation (no background change here, just animation)
        yield return StartCoroutine(PlayWakingUpAnimation());

        // Allow player movement now
        player.GetComponent<PlayerController>().EnableMovement(true);

        // Start tutorial sequence with text bubble
        yield return StartCoroutine(DisplayTextBubble("Alira:", "Slowly waking up…"));
        yield return StartCoroutine(DisplayTextBubble("Alira:", "What happ- agh.. It hurts.. I need to find something to close the wound"));

        // Player can now move and interact with medkit
        yield return new WaitForSeconds(2f); // Wait for a moment
        dialogueText.text = ""; // Clear text before player interacts
        canSkip = false; // Disable skipping while player interacts

        // When the player picks up the Medkit
        yield return StartCoroutine(DisplayTextBubble("Alira:", "That’s better, I need to find others.. But these iron beasts are everywhere, I need to find myself a weapon."));
        
        // When the player picks up the Bow
        yield return StartCoroutine(DisplayTextBubble("Alira:", "A bow? I don’t know if these can damage them… Wait but the arrows… looks like my people have always prepared for this. I can use this."));
    }

    public IEnumerator DisplayText(string text)
    {
        dialogueText.text = "";  // Clear previous text
        canSkip = true; // Allow skipping the text
        isDialogueActive = true; // Set dialogue as active

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed); // Wait for a bit for each letter
        }

        // After text finishes, disable skipping
        canSkip = false;
    }

    public IEnumerator DisplayTextBubble(string speaker, string text)
    {
        dialogueText.text = $"{speaker} {text}";
        yield return new WaitForSeconds(3f); // Wait for a few seconds to show the bubble
    }

    public IEnumerator TransitionToBlackScreen()
    {
        background.color = Color.black; // Keep the background black for the unconscious state
        yield return new WaitForSeconds(1f); // Wait for a moment
    }

    // Trigger the waking-up animation (Alira waking up)
    public IEnumerator PlayWakingUpAnimation()
    {
        wakingUpAnimator.SetTrigger("WakeUp"); // Trigger the animation (ensure you have the "WakeUp" trigger in your Animator)
        yield return new WaitForSeconds(3f); // Wait for the animation to finish
    }
}
