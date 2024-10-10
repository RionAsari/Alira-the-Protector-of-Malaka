using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGrunt : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 3f;
    public float detectionRange = 5f;
    public int damage = 10;
    public float attackCooldown = 2f;

    private bool movingRight = true;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private Transform player;
    private Rigidbody2D rb;

    public LayerMask playerLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null)
        {
            return; // If player not found, do nothing
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            Debug.Log("Player detected within detection range!");

            // If within attack range, stop and attack
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
            else
            {
                // If not in attack range, move towards player
                MoveTowardsPlayer();
            }
        }
        else
        {
            // Move randomly or idle if the player is not within detection range
            Patrol();
        }

        FlipBasedOnMovement();
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking)
        {
            return; // Don't move while attacking
        }

        // Move towards player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop moving when attacking

        // Check direction to flip before attacking
        if (player.position.x < transform.position.x && movingRight)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && !movingRight)
        {
            Flip();
        }

        lastAttackTime = Time.time;

        // Call damage function on the player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damage);
            Debug.Log("Player took damage!");
        }

        // Optionally, you can trigger an attack animation here

        // Reset attacking state after a short delay
        Invoke("ResetAttack", 0.5f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void Patrol()
    {
        // Simple patrolling, moving left and right
        if (movingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }

    private void FlipBasedOnMovement()
    {
        if (rb.velocity.x > 0 && !movingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && movingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // Flip the enemy's direction by inverting the X scale
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Change direction when hitting a wall or an edge
        if (collision.CompareTag("Edge") || collision.CompareTag("Obstacle"))
        {
            movingRight = !movingRight;
            Flip();
        }
    }
}
