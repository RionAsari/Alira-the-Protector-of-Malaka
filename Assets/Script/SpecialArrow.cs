using System.Collections;
using UnityEngine;

public class SpecialArrow : MonoBehaviour
{
    public float disableDuration = 5f; // Duration the enemy is disabled

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If it collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null && !enemy.isAlly) // Check if enemy is not an ally
            {
                enemy.HandleHacking(0.5f); // Pass a generic charge level, can be adjusted if needed
                StartCoroutine(DisableEnemy(enemy)); // Disable the enemy
            }
        }

        // Destroy the arrow after hitting an object
        Destroy(gameObject);
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
}
