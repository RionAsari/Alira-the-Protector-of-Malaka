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

        // Ignore collisions with Arrow and SpecialArrow
        if (other.CompareTag("Arrow") || other.CompareTag("SpecialArrow"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other); // Ignore collision
            return; // Exit the method
        }

        // Ignore collision with MiddleBot
        if (other.CompareTag("MiddleBot"))
        {
            return; // Ignore collision and do nothing
        }

        if (other.CompareTag("Player"))
        {
            // Get the Health and PlayerMovement components of the player
            Health playerHealth = other.GetComponent<Health>();
            PlayerController playerController = other.GetComponent<PlayerController>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Deal damage to the player
            }

            if (playerController != null)
            {
                // Apply knockback
                playerController.KBCounter = playerController.KBTotalTime;
                playerController.KnockFromRight = other.transform.position.x < transform.position.x;
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
            }

            Destroy(gameObject); // Destroy projectile after hitting Ally
        }
        else if (other.CompareTag("Enemy"))
        {
            return; // Simply return and do nothing if it's an enemy
        }
        else
        {
            // Destroy the projectile on hitting any other object (if needed)
            Destroy(gameObject);
        }
    }
}
