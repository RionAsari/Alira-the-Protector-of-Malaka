using System.Collections;
using UnityEngine;

public class SpecialArrow : MonoBehaviour
{
    public float disableDuration = 5f; // Duration the enemy is disabled
    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component

        // Make this arrow's collider a trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true; // Set collider to be a trigger
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If collides with LightGrunt
        if (other.CompareTag("LightGrunt"))
        {
            LightGrunt enemy = other.GetComponent<LightGrunt>();

            if (enemy != null)
            {
                Debug.Log("Hit LightGrunt: " + enemy.name); // Debug log
                // Disable the enemy immediately
                StartCoroutine(DisableEnemy(enemy)); // Disable the enemy
            }
            else
            {
                Debug.Log("Enemy component is null.");
            }
        }
        else if (other.CompareTag("Ally"))
        {
            // Do nothing if it hits HackedLightGrunt
            Debug.Log("SpecialArrow hit an ally: " + other.gameObject.name);
            return; // Prevent further processing for HackedLightGrunt
        }

        // Trigger hit animation and handle destruction
        TriggerHitAnimation();
    }

    // Coroutine to disable the enemy
    private IEnumerator DisableEnemy(LightGrunt enemy)
    {
        enemy.isHackable = true; // Set the enemy to be hackable

        // Call DisableEnemy with the specified duration
        yield return StartCoroutine(enemy.DisableEnemy(disableDuration)); // Wait for the disable duration

        // After the disable duration, reset the hackable state
        enemy.isHackable = false; // Set to not hackable
    }

    // Trigger hit animation and prepare for destruction
    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Trigger hit animation
        }

        // Remove collider and rigidbody immediately to avoid further physics interactions
        Destroy(GetComponent<Collider2D>()); // Remove collider
        Destroy(GetComponent<Rigidbody2D>()); // Remove rigidbody

        // Call DestroyArrow after a delay that matches your hit animation length
        Invoke("DestroyArrow", 0.2f); // Example delay, adjust as needed
    }

    // Call this function at the end of the Hit animation to destroy the arrow
    public void DestroyArrow()
    {
        Destroy(gameObject); // Destroy the arrow object
    }
}
