using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar;

    private void Start()
    {
        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(0, health); // Pastikan health tidak di bawah 0

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        health = Mathf.Min(health, maxHealth); // Pastikan health tidak melebihi maxHealth

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }
        
        Debug.Log($"Player healed by {healAmount} points. Current health: {health}");
    }

    private void Die()
    {
        Debug.Log("Player has died");
        // Tambahkan logika kematian di sini
    }
}
