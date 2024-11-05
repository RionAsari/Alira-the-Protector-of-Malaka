using UnityEngine;

public class HackedVolley : MonoBehaviour
{
    public float damage = 25f; // Damage dealt by the projectile
    public float lifetime = 5f; // Lifetime of the projectile before auto-destroying

    private void Start()
    {
        // Destroy the projectile after a certain lifetime if it doesn't hit anything
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Ignore collisions with Arrow, SpecialArrow, and Ally
        if (other.CompareTag("Arrow") || other.CompareTag("SpecialArrow") || other.CompareTag("Ally"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other); // Ignore collision
            return; // Exit the method
        }

        // Ignore collision with Player
        if (other.CompareTag("Player"))
        {
            return; // Ignore collision and do nothing
        }

        // Deal damage to enemies and MiddleBots
        if (other.CompareTag("Enemy") || other.CompareTag("MiddleBot"))
        {
            // Get the Health component of the enemy or MiddleBot
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Deal damage to the enemy or MiddleBot
            }

            // Destroy the projectile upon collision with an enemy or MiddleBot
            Destroy(gameObject);
        }
        else
        {
            // Destroy the projectile on hitting any other object (if needed)
            Destroy(gameObject);
        }
    }
}
