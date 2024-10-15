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
    public Transform playerTransform;
    public LayerMask enemyLayer; // For detecting other enemies
    public float allyAttackRange = 7f; // Range to detect other enemies

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isAlly = false;

    private float initialHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private bool alreadyAttacked = false;
    private Coroutine disableCoroutine;

    private void Start()
    {
        initialHealth = health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(IdleMovement());
    }

    private void Update()
    {
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
        }
        else if (distanceToPlayer <= attackRange && !alreadyAttacked)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        alreadyAttacked = true;
        animator.SetTrigger("Attack");
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
        Destroy(gameObject);
    }

    public IEnumerator DisableEnemy()
    {
        isDisabled = true;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetTrigger("Disabled");

        // Start the countdown to destroy the enemy if not hacked within the duration
        yield return new WaitForSeconds(hackableDuration);

        // Only destroy if not hacked
        if (!isHackable)
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

        if (distanceToPlayer > 2f)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop moving when close to the player
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
        animator.SetTrigger("Attack");

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && !enemyScript.isAlly) // Only attack enemies that are not allies
        {
            enemyScript.TakeDamage(damageAmount);
        }

        yield return new WaitForSeconds(1f);
        alreadyAttacked = false;
    }

    // To move past the player and not be blocked by them
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
