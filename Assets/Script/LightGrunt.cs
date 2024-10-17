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
    public LayerMask enemyLayer; 
    public float allyAttackRange = 7f; 
    public float hackDistance = 2f; 

    public bool isDisabled = false; 
    public bool isHackable = true; 
    public bool isAlly = false; 
    public bool isHacked = false; 

    private float initialHealth; 
    private Rigidbody2D rb; 
    private Animator animator; 
    private bool alreadyAttacked = false; 
    private SpriteRenderer spriteRenderer; 
    private bool isChasingPlayer = false; 
    private Coroutine patrolCoroutine; 

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Pemain tidak ditemukan. Pastikan pemain memiliki tag 'Player'.");
        }

        initialHealth = health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if required components are found
        if (rb == null) Debug.LogError("Rigidbody2D is missing!", this);
        if (animator == null) Debug.LogError("Animator is missing!", this);
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer is missing!", this);

        patrolCoroutine = StartCoroutine(Patrol()); 
    }

    private void Update()
    {
        if (health <= 0)
        {
            return; 
        }

        if (isHacked)
        {
            FollowPlayer();
            DetectAndAttackEnemies();
        }
        else if (isAlly)
        {
            DetectAndAttackEnemies();
        }
        else if (!isDisabled)
        {
            DetectAndAttackPlayer();
        }

        // Hack the enemy when "F" is pressed, the enemy is hackable, and within hack distance
        if (Input.GetKeyDown(KeyCode.F) && isHackable && playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= hackDistance)
        {
            HackEnemy();
        }

        // Handle Idle and Walking animation
        animator.SetBool("isWalking", rb.velocity.magnitude > 0);
    }

    private void DetectAndAttackPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            isChasingPlayer = true;
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            spriteRenderer.flipX = direction.x > 0; // Flipping logic reversed
        }
        else if (distanceToPlayer <= attackRange && !alreadyAttacked)
        {
            StartCoroutine(AttackPlayer());
        }
        else
        {
            rb.velocity = Vector2.zero; 
            isChasingPlayer = false; 
        }
    }

    private IEnumerator Patrol()
    {
        while (!isAlly && !isDisabled && !isHacked)
        {
            // Move to the right
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            spriteRenderer.flipX = true; // Flipping logic reversed

            yield return new WaitForSeconds(2f);

            // Move to the left
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            spriteRenderer.flipX = false; // Flipping logic reversed

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator AttackPlayer()
    {
        alreadyAttacked = true;
        animator.SetTrigger("isAttacking"); 
        if (playerTransform != null)
        {
            PlayerController playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage((int)damageAmount);
            }
            else
            {
                Debug.LogError("PlayerController is missing on the player!", this);
            }
        }

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
        else
        {
            rb.velocity = Vector2.zero; 
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead"); 
        rb.velocity = Vector2.zero; 
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f); // Replace with your animation duration
        Destroy(gameObject);
    }

    public IEnumerator DisableEnemy()
    {
        isDisabled = true;
        rb.velocity = Vector2.zero; 
        rb.isKinematic = true; 
        animator.SetTrigger("isDisabled"); 

        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null; 
        }

        GetComponent<Collider2D>().enabled = false; 

        yield return new WaitForSeconds(5f);

        if (!isHacked && !isAlly)
        {
            Die();
        }
        else
        {
            isDisabled = false; 
            rb.isKinematic = false; 
            GetComponent<Collider2D>().enabled = true; 
            patrolCoroutine = StartCoroutine(Patrol()); 
        }
    }

    public void HandleHacking(float chargeLevel)
    {
        if (isHackable) 
        {
            HackEnemy(); 
        }
    }

    private void HackEnemy()
    {
        isHackable = false; 
        isAlly = true; 
        isHacked = true; 
        health = initialHealth; 
        rb.velocity = Vector2.zero; 
        gameObject.tag = "Ally"; 

        animator.SetTrigger("isHacked");

        StartCoroutine(DestroyHackedEnemy());
    }

    private IEnumerator DestroyHackedEnemy()
    {
        yield return new WaitForSeconds(allyDuration);
        Die(); 
    }

    private void FollowPlayer()
    {
        if (playerTransform == null) return; 

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized; 

        if (distanceToPlayer > 1f)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            animator.SetBool("isWalking", true); 
        }
        else
        {
            rb.velocity = Vector2.zero; 
            animator.SetBool("isWalking", false); 
        }

        spriteRenderer.flipX = direction.x > 0; // Flipping logic reversed
    }

    private void DetectAndAttackEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, allyAttackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

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

            if (closestEnemy != null)
            {
                Vector2 directionToEnemy = (closestEnemy.position - transform.position).normalized;
                rb.velocity = new Vector2(directionToEnemy.x * moveSpeed, rb.velocity.y);

                if (closestDistance <= attackRange && !alreadyAttacked)
                {
                    StartCoroutine(AttackEnemy(closestEnemy));
                }

                spriteRenderer.flipX = directionToEnemy.x > 0; // Flipping logic reversed
            }
        }
    }

    private IEnumerator AttackEnemy(Transform enemy)
    {
        alreadyAttacked = true;
        animator.SetTrigger("isAttacking"); 

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null && !enemyScript.isAlly) 
        {
            enemyScript.TakeDamage(damageAmount);
        }

        yield return new WaitForSeconds(1f);
        alreadyAttacked = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisabled && collision.collider.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, allyAttackRange);
    }
}
