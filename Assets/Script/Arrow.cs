using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f; // Time before arrow is destroyed
    public float colliderDelay = 0.2f; // Time before collider is enabled
    private Collider2D arrowCollider; // Arrow's collider
    private float damage; // Damage dealt by the arrow

    private void Start()
    {
        // Get the collider component
        arrowCollider = GetComponent<Collider2D>();

        // Disable the collider initially
        arrowCollider.enabled = false;

        // Enable collider after delay
        StartCoroutine(EnableColliderAfterDelay());

        // Destroy the arrow after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    // Coroutine to enable collider after a short delay
    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderDelay);
        arrowCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the arrow upon hitting an object
        Destroy(gameObject);
    }

    // Function to set the damage of the arrow
    public void SetDamage(float arrowDamage)
    {
        damage = arrowDamage;
    }
}
