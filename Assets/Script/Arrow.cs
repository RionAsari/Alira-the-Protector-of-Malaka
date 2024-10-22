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
        // Ignore player and HackedLightGrunt, handle LightGrunt
        if (other.CompareTag("Player"))
        {
            // Do nothing if it hits the player
            Debug.Log("Arrow hit the player: " + other.gameObject.name);
            return;
        }
        else if (other.CompareTag("Ally")) // Check for HackedLightGrunt
        {
            // Do nothing if it hits HackedLightGrunt
            Debug.Log("Arrow hit an ally: " + other.gameObject.name);
            return; // Prevent further processing for HackedLightGrunt
        }
        else if (other.CompareTag("LightGrunt"))
        {
            LightGrunt enemy = other.GetComponent<LightGrunt>();

            if (enemy != null)
            {
                HandleHit(enemy); // Handle the hit logic for LightGrunt
            }
        }

        // Destroy the arrow upon hitting an object
        Destroy(gameObject);
    }

    // Method to handle the hit logic for LightGrunt
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

    // Calculate damage based on charge level
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

    // Function to set the damage of the arrow
    public void SetDamage(float arrowDamage)
    {
        damage = arrowDamage; // Set damage value
    }

    // Function to get the damage of the arrow
    public float GetDamage()
    {
        return damage; // Return the damage value
    }

    // Function to set the charge level of the arrow
    public void SetChargeLevel(float level)
    {
        chargeLevel = level; // Set charge level
    }
}
