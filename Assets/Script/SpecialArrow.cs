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
        // Ignore collision with Volley projectiles
        if (other.CompareTag("Volley"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other); // Ignore collision
            return; // Exit the method
        }

        // If collides with anything tagged as "Enemy"
        if (other.CompareTag("Enemy"))
        {
            LightGrunt enemy = other.GetComponent<LightGrunt>();
            if (enemy != null)
            {
                StartCoroutine(DisableEnemy(enemy)); // Disable the enemy
            }
        }
        else if (other.CompareTag("MiddleBot"))
        {
            MiddleBot middleBot = other.GetComponent<MiddleBot>();
            if (middleBot != null && middleBot.IncrementHitCount())
            {
                StartCoroutine(DisableEnemy(middleBot)); // Disable the MiddleBot
            }
        }
        else if (other.CompareTag("Ally"))
        {
            // Do nothing if it hits HackedLightGrunt
            return; // Prevent further processing for HackedLightGrunt
        }

        // Trigger hit animation and handle destruction
        TriggerHitAnimation();
    }

    private IEnumerator DisableEnemy(LightGrunt enemy)
    {
        enemy.isHackable = true; // Set the enemy to be hackable
        yield return StartCoroutine(enemy.DisableEnemy(disableDuration)); // Wait for the disable duration
        enemy.isHackable = false; // Set to not hackable
    }

    private IEnumerator DisableEnemy(MiddleBot middleBot)
    {
        yield return StartCoroutine(middleBot.DisableEnemy(disableDuration)); // Wait for the disable duration
    }

    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Trigger hit animation
        }

        // Remove collider and rigidbody immediately to avoid further physics interactions
        Destroy(GetComponent<Collider2D>()); // Remove collider
        Destroy(GetComponent<Rigidbody2D>()); // Remove rigidbody

        Invoke("DestroyArrow", 0.2f); // Example delay, adjust as needed
    }

    public void DestroyArrow()
    {
        Destroy(gameObject); // Destroy the arrow object
    }
}
