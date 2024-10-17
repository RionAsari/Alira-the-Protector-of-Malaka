using System.Collections;
using UnityEngine;

public class SpecialArrow : MonoBehaviour
{
    public float disableDuration = 5f; // Duration the enemy is disabled
    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If it collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null && !enemy.isAlly) // Check if enemy is not an ally
            {
                enemy.HandleHacking(0.5f); // Pass a generic charge level
                StartCoroutine(DisableEnemy(enemy)); // Disable the enemy
            }
        }

        // Trigger the hit animation and handle destruction
        TriggerHitAnimation();
    }

    // Coroutine to disable the enemy
    private IEnumerator DisableEnemy(Enemy enemy)
    {
        enemy.isHackable = true; // Set enemy to hackable
        yield return enemy.DisableEnemy(); // Call DisableEnemy in the Enemy script

        yield return new WaitForSeconds(disableDuration); // Wait for disable duration

        if (!enemy.isAlly) // If the enemy is not hacked yet
        {
            enemy.isHackable = false; // Set to not hackable
            Destroy(enemy.gameObject); // Destroy the enemy
        }
    }

    // Trigger hit animation and prepare for destruction
    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Trigger the hit animation
        }

        // Remove the collider and rigidbody immediately to avoid further physics interaction
        Destroy(GetComponent<Collider2D>()); // Remove the collider
        Destroy(GetComponent<Rigidbody2D>()); // Remove the rigidbody

        // Call DestroyArrow after a delay that matches your hit animation length
        // Adjust the delay as necessary based on your animation timing
        Invoke("DestroyArrow", 0.2f); // Example delay
    }

    // Call this function at the end of the Hit animation to destroy the arrow
    public void DestroyArrow()
    {
        Destroy(gameObject); // Destroy the arrow object
    }
}
