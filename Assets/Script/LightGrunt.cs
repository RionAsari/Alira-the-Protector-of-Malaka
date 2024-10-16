using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float moveSpeed = 2f;
    public float attackRange = 5f;
    public float damageAmount = 10f;
    public float detectionRange = 10f;
    public float hackableDuration = 5f;
    public float allyDuration = 10f;
    public Transform playerTransform; // Transform player
    public LayerMask enemyLayer; // For detecting other enemies
    public float allyAttackRange = 7f; // Range to detect other enemies

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isAlly = false;

    private float initialHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private bool alreadyAttacked = false;
    private SpriteRenderer spriteRenderer; // Untuk flip sprite

    private void Start()
    {
        // Pastikan untuk mendapatkan transform player
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
        spriteRenderer = GetComponent<SpriteRenderer>(); // Ambil komponen SpriteRenderer

        StartCoroutine(IdleMovement());
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

        // Flip musuh agar menghadap ke arah pemain
        FacePlayer();

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
        while (!isAlly && !isDisabled)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f);
            moveSpeed = -moveSpeed;
        }
    }

    private void DetectAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
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
        // Optional: add delay before destroying the object
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

        // Only destroy if not hacked
        if (!isHackable && !isAlly)
        {
            Die();
        }
        else
        {
            isDisabled = false; // Re-enable if hacked
        }
    }

    private void HackEnemy()
    {
        isHackable = false; // After being hacked, the enemy cannot be hacked again
        isAlly = true; // The enemy becomes an ally
        health = initialHealth; // Restore health
        isDisabled = false;
        animator.SetTrigger("Hacked");

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

        if (distanceToPlayer > 1f)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            animator.SetBool("isWalking", true); // Activate walk animation when approaching player
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop moving when close to the player
            animator.SetBool("isWalking", false); // Switch to idle animation when close
        }
    }

    private void DetectAndAttackEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, allyAttackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Transform closestEnemy = enemiesInRange[0].transform;

            for (int i = 1; i < enemiesInRange.Length; i++)
            {
                if (Vector2.Distance(transform.position, enemiesInRange[i].transform.position) <
                    Vector2.Distance(transform.position, closestEnemy.position))
                {
                    closestEnemy = enemiesInRange[i].transform;
                }
            }

            // Move towards and attack the closest enemy
            Vector2 directionToEnemy = (closestEnemy.position - transform.position).normalized;
            rb.velocity = new Vector2(directionToEnemy.x * moveSpeed, rb.velocity.y);

            if (Vector2.Distance(transform.position, closestEnemy.position) <= attackRange && !alreadyAttacked)
            {
                StartCoroutine(AttackEnemy(closestEnemy));
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

    // Membalikkan sprite musuh agar melihat ke arah player
    private void FacePlayer()
    {
        if (playerTransform == null)
        {
            return; // Jika playerTransform tidak ada, abaikan.
        }

        // Jika posisi pemain di kanan musuh, sprite harus tidak ter-flip (menghadap kanan)
        if (playerTransform.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        // Jika posisi pemain di kiri musuh, sprite harus flip (menghadap kiri)
        else if (playerTransform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
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
        Gizmos.DrawWireSphere(transform.position, allyAttackRange);
    }
}
