using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f; // Time before the arrow is destroyed
    public float colliderDelay = 0.2f; // Time before the collider is enabled
    private Collider2D arrowCollider; // Arrow's collider
    private float damage; // Damage dealt by the arrow
    public float chargeLevel; // For managing charge levels
    public bool isSpecialArrow = false; // Flag to identify if the arrow is special

    private void Start()
    {
        // Get the collider component
        arrowCollider = GetComponent<Collider2D>();

        // Make the arrow's collider a trigger
        if (arrowCollider != null)
        {
            arrowCollider.isTrigger = true; // Set collider to be a trigger
        }

        // Destroy the arrow after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collision with Volley projectiles
        if (other.CompareTag("Volley"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other); // Ignore collision
            return; // Exit the method
        }

        // Ignore player and allies, handle enemies
        if (other.CompareTag("Player"))
        {
            return; // Do nothing if it hits the player
        }
        else if (other.CompareTag("Ally")) 
        {
            return; // Do nothing if it hits an ally
        }
        else if (other.CompareTag("Enemy")) // Check for any enemy tagged as "Enemy"
        {
            LightGrunt enemy = other.GetComponent<LightGrunt>(); // Still using LightGrunt component
            if (enemy != null)
            {
                HandleHit(enemy); // Handle the hit logic for any "Enemy" tag
            }
        }
        else if (other.CompareTag("MiddleBot")) // Check for MiddleBot tag
        {
            MiddleBot middleBot = other.GetComponent<MiddleBot>(); // Get MiddleBot component
            if (middleBot != null)
            {
                HandleHitMiddleBot(middleBot); // Handle hit logic for MiddleBot
            }
        }

        // Destroy the arrow upon hitting an object
        Destroy(gameObject);
    }

    private void HandleHit(LightGrunt enemy)
    {
        if (isSpecialArrow)
        {
            // Disable the enemy if it's a special arrow
            StartCoroutine(enemy.DisableEnemy(5f)); // Disable for 5 seconds
        }
        else
        {
            // Calculate damage based on charge level for regular arrows
            int damageToDeal = CalculateDamage();
            damageToDeal = Mathf.Min(damageToDeal, Mathf.FloorToInt(enemy.maxHealth));
            enemy.TakeDamage(damageToDeal); // Call method to reduce health
        }
    }

    private void HandleHitMiddleBot(MiddleBot middleBot)
    {
        if (isSpecialArrow)
        {
            // Disable the MiddleBot if it's a special arrow
            StartCoroutine(middleBot.DisableEnemy(5f)); // Adjust method as needed
        }
        else
        {
            // Calculate damage based on charge level for regular arrows
            int damageToDeal = CalculateDamage();
            middleBot.TakeDamage(damageToDeal); // Call method to reduce health
        }
    }

    private int CalculateDamage()
    {
        if (chargeLevel >= 0.01f && chargeLevel < 0.5f) // 1%-49% charge
        {
            return 25;
        }
        else if (chargeLevel >= 0.5f && chargeLevel < 1f) // 50%-99% charge
        {
            return 50;
        }
        else if (chargeLevel >= 1f) // 100% charge
        {
            return 100;
        }
        return 0; // No damage if charge level is 0
    }

    public void SetDamage(float arrowDamage)
    {
        damage = arrowDamage; // Set damage value
    }

    public float GetDamage()
    {
        return damage; // Return the damage value
    }

    public void SetChargeLevel(float level)
    {
        chargeLevel = level; // Set charge level
    }
}
