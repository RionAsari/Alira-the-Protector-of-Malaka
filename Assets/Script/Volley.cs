using UnityEngine;

public class Volley : MonoBehaviour
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
        Debug.Log("Collided with: " + other.gameObject.name); // Log the collided object name

        // Ignore collisions with Arrow and SpecialArrow
        if (other.CompareTag("Arrow") || other.CompareTag("SpecialArrow"))
        {
            Debug.Log("Ignored collision with: " + other.gameObject.name);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other); // Ignore collision
            return; // Exit the method
        }

        // Ignore collision with MiddleBot
        if (other.CompareTag("MiddleBot"))
        {
            Debug.Log("Ignored collision with MiddleBot: " + other.gameObject.name);
            return; // Ignore collision and do nothing
        }

        if (other.CompareTag("Player"))
        {
            // Get the Health component of the player
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Deal damage to the player
                Debug.Log("Damage dealt to player: " + damage); // Log damage dealt
            }

            // Destroy the projectile upon collision with the player
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ally"))
        {
            // Handle collision with an Ally (such as HackedMiddleBot or HackedLightGrunt)
            var allyHealth = other.GetComponent<HackedMiddleBot>() ?? (MonoBehaviour)other.GetComponent<HackedLightGrunt>();
            if (allyHealth != null)
            {
                if (allyHealth is HackedMiddleBot)
                    ((HackedMiddleBot)allyHealth).TakeDamage(damage); // Deal damage to HackedMiddleBot
                else if (allyHealth is HackedLightGrunt)
                    ((HackedLightGrunt)allyHealth).TakeDamage(damage); // Deal damage to HackedLightGrunt

                Debug.Log($"Damage dealt to ally: {damage}"); // Log damage dealt
            }

            Destroy(gameObject); // Destroy projectile after hitting Ally
        }
        else if (other.CompareTag("Enemy"))
        {
            // Ignore collisions with enemies
            Debug.Log("Ignored collision with enemy: " + other.gameObject.name);
            return; // Simply return and do nothing if it's an enemy
        }
        else
        {
            // Destroy the projectile on hitting any other object (if needed)
            Destroy(gameObject);
        }
    }
}
