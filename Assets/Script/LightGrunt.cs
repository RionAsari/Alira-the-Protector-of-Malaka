using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f; // Enemy health
    public float moveSpeed = 2f; // Enemy movement speed
    public float attackRange = 5f; // Distance to attack player or enemies
    public float damageAmount = 10f; // Damage dealt to player/enemies
    public float detectionRange = 10f; // Range to detect player
    public float hackableDuration = 5f; // Time before the enemy can be hacked again
    public float allyDuration = 10f; // Time before the enemy is destroyed after becoming an ally
    public Transform playerTransform; // Transform for the player
    public LayerMask enemyLayer; // Layer for detecting other enemies
    public float allyAttackRange = 7f; // Range to detect other enemies when hacked

    public bool isDisabled = false; // Is the enemy disabled?
    public bool isHackable = true; // Can the enemy be hacked? Set to true by default
    public bool isAlly = false; // Is the enemy an ally?

    private float initialHealth; // Store initial health
    private Rigidbody2D rb; // Rigidbody for movement
    private Animator animator; // Animator for animations
    private bool alreadyAttacked = false; // Flag to prevent double attacks
    private SpriteRenderer spriteRenderer; // For flipping the sprite
    private bool isChasingPlayer = false; // Track if the enemy is chasing the player

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure the player has the 'Player' tag.");
        }

        initialHealth = health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        StartCoroutine(IdleMovement()); // Start idle movement coroutine
    }

    private void Update()
    {
        if (health <= 0)
        {
            return; // Stop updating if enemy is dead
        }

        if (isAlly)
        {
            FollowPlayer();
            DetectAndAttackEnemies(); // Detect and attack other enemies
        }
        else if (!isDisabled)
        {
            DetectAndAttackPlayer();
        }

        // Hack the enemy when "F" is pressed and the enemy is hackable
        if (Input.GetKeyDown(KeyCode.F) && isHackable)
        {
            HackEnemy();
        }

        // Handle Idle and Walking animation
        if (rb.velocity.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
    }

    private IEnumerator IdleMovement()
    {
        while (!isAlly && !isDisabled && !isChasingPlayer)
        {
            // Gerakan acak ke kanan dan kiri
            float randomDirection = Random.Range(-1f, 1f);
            rb.velocity = new Vector2(randomDirection * moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f); // Gerak selama 2 detik
            rb.velocity = Vector2.zero; // Henti sejenak
            yield return new WaitForSeconds(1f); // Henti selama 1 detik
        }
    }

    private void DetectAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            isChasingPlayer = true; // Start chasing the player
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            animator.SetBool("isWalking", true); // Activate walk animation
        }
        else if (distanceToPlayer <= attackRange && !alreadyAttacked)
        {
            StartCoroutine(AttackPlayer());
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false); // Return to idle if not moving
            isChasingPlayer = false; // Stop chasing the player if out of range
            if (!isChasingPlayer)
            {
                StartCoroutine(IdleMovement()); // Return to idle movement if not chasing
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        alreadyAttacked = true;
        animator.SetTrigger("isAttacking"); // Call attack animation
        playerTransform.GetComponent<PlayerController>().TakeDamage((int)damageAmount);

        yield return new WaitForSeconds(1f);
        alreadyAttacked = false;
    }

    public void TakeDamage(float arrowDamage)
    {
        health -= arrowDamage;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead"); // Trigger death animation
        rb.velocity = Vector2.zero; // Stop movement
        Invoke("DestroyEnemy", 2f); // Wait for 2 seconds after death animation
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public IEnumerator DisableEnemy()
    {
        isDisabled = true;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetTrigger("isDisabled"); // Trigger disable animation

        // Start the countdown to destroy the enemy if not hacked within the duration
        yield return new WaitForSeconds(hackableDuration);

        if (!isHackable && !isAlly)
        {
            Die();
        }
        else
        {
            isDisabled = false; // Re-enable if hacked
        }
    }

    public void HandleHacking(float chargeLevel)
    {
        if (isHackable) // Only proceed if the enemy is hackable
        {
            HackEnemy(); // Call the hacking method directly
        }
    }

    private void HackEnemy()
    {
        isHackable = false; // After being hacked, the enemy cannot be hacked again
        isAlly = true; // The enemy becomes an ally
        health = initialHealth; // Restore health
        isDisabled = false;
        gameObject.tag = "Ally"; // Change tag to Ally

        // Start the countdown for destroying the hacked enemy after ally duration
        StartCoroutine(DestroyHackedEnemy());
    }

    private IEnumerator DestroyHackedEnemy()
    {
        yield return new WaitForSeconds(allyDuration);
        Die(); // Destroy the hacked enemy after ally duration
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized; // Calculate direction to player

        if (distanceToPlayer > 1f)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            animator.SetBool("isWalking", true); // Activate walk animation when approaching player
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop moving when close to the player
            animator.SetBool("isWalking", false); // Switch to idle animation when close
        }

        // Flip the sprite to face the player
        spriteRenderer.flipX = direction.x < 0; // Flip sprite based on direction to player
    }

    private void DetectAndAttackEnemies()
    {
        // Use the existing enemy layer mask to find enemies within range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, allyAttackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            // Find the closest enemy tagged "Enemy"
            foreach (var enemyCollider in enemiesInRange)
            {
                if (enemyCollider.CompareTag("Enemy"))
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemyCollider.transform.position);
                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        closestEnemy = enemyCollider.transform;
                    }
                }
            }

            // If a closest enemy is found, move towards and attack it
            if (closestEnemy != null)
            {
                Vector2 directionToEnemy = (closestEnemy.position - transform.position).normalized;
                rb.velocity = new Vector2(directionToEnemy.x * moveSpeed, rb.velocity.y);

                // Attack if within attack range
                if (Vector2.Distance(transform.position, closestEnemy.position) <= attackRange && !alreadyAttacked)
                {
                    StartCoroutine(AttackEnemy(closestEnemy));
                }
            }
        }
    }

    private IEnumerator AttackEnemy(Transform enemy)
    {
        alreadyAttacked = true;
        animator.SetTrigger("isAttacking"); // Trigger attack animation

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && !enemyScript.isAlly) // Only attack enemies that are not allies
        {
            enemyScript.TakeDamage(damageAmount);
        }

        yield return new WaitForSeconds(1f);
        alreadyAttacked = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAlly)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, allyAttackRange); // Draw detection range
    }
}
