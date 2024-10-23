using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar; // Reference to HealthbarBehaviour

    private void Start()
    {
        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth); // Initialize health bar
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth); // Update health bar
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle death (e.g., play animation, destroy object, etc.)
        Debug.Log("Player has died");
        // Add additional death logic here
    }
}
