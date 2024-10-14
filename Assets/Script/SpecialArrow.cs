using System.Collections;
using UnityEngine;

public class SpecialArrow : Arrow // Inherits from Arrow
{
    public float disableDuration = 5f; // Duration enemies are disabled

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the arrow collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Disable the enemy for a duration
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(DisableEnemy(enemy));
            }
        }
        
        // Destroy the arrow upon hitting an object
        Destroy(gameObject);
    }

    private IEnumerator DisableEnemy(Enemy enemy)
    {
        enemy.Disable(); // Assuming you have a method to disable enemy behavior
        yield return new WaitForSeconds(disableDuration);
        enemy.Enable(); // Re-enable enemy behavior after the duration
    }
}
