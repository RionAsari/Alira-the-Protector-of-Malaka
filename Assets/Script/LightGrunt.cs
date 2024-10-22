using System.Collections;
using UnityEngine;

public class LightGrunt : MonoBehaviour
{
    // Variables
    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float attackRange = 5f;
    public float damageAmount = 10f;
    public float detectionRange = 10f;
    public float allyDuration = 10f;
    public LayerMask enemyLayer;
    public float allyAttackRange = 7f;
    public float hackDistance = 2f;
    public float attackCooldown = 1f;  
    public bool isDisabled = false;
    public bool isHackable = true;
    public bool isAlly = false;
    public bool isHacked = false;

    public HealthbarBehaviour healthbar;

    private Rigidbody2D rb;
    private Animator animator;
    private Coroutine patrolCoroutine;
    private float lastAttackTime = 0f;

    private void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        patrolCoroutine = StartCoroutine(Patrol());
    }

    private void Update()
    {
        if (health <= 0) return;

        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }

        if (isHacked)
        {
            // If hacked, check for un-hacked enemies
            DetectAndAttackEnemies();
        }
        else if (!isDisabled)
        {
            // First check if there are hacked enemies before chasing the player
            if (!DetectAndAttackHackedEnemies())
            {
                DetectAndAttackPlayer();
            }
        }

        // Hacking logic
        if (Input.GetKeyDown(KeyCode.F) && isHackable && Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) <= hackDistance)
        {
            HackEnemy();
        }

        animator.SetBool("isWalking", rb.velocity.magnitude > 0);
        
        // Update character's facing direction
        if (rb.velocity.x != 0)
        {
            UpdateFacingDirection(rb.velocity.x);
        }
    }

    private void UpdateFacingDirection(float moveDirectionX)
    {
        if (moveDirectionX > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirectionX < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Patrol Logic
    private IEnumerator Patrol()
    {
        while (!isAlly && !isDisabled && !isHacked)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f);

            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f);
        }
    }

    // Detect and attack hacked enemies
    private bool DetectAndAttackHackedEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        Transform closestHackedEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemyCollider in enemiesInRange)
        {
            LightGrunt enemyScript = enemyCollider.GetComponent<LightGrunt>();
            if (enemyScript != null && enemyScript.isHacked && enemyScript.health > 0)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestHackedEnemy = enemyCollider.transform;
                }
            }
        }

        if (closestHackedEnemy != null)
        {
            Vector2 directionToTarget = (closestHackedEnemy.position - transform.position).normalized;
            rb.velocity = new Vector2(directionToTarget.x * moveSpeed, rb.velocity.y);

            if (closestDistance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackEnemy(closestHackedEnemy));
            }

            return true; 
        }

        return false; 
    }

    // Player Detection and Attack
    private void DetectAndAttackPlayer()
    {
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        else if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackPlayer());
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Detect and attack other enemies when hacked
    private void DetectAndAttackEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, allyAttackRange, enemyLayer);
        Transform target = null;
        float closestDistance = float.MaxValue;

        foreach (var enemyCollider in enemiesInRange)
        {
            LightGrunt enemyScript = enemyCollider.GetComponent<LightGrunt>();
            if (enemyScript != null && enemyScript.health > 0 && !enemyScript.isHacked)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    target = enemyCollider.transform;
                }
            }
        }

        if (target != null)
        {
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(directionToTarget.x * moveSpeed, rb.velocity.y);

            if (closestDistance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackEnemy(target));
            }
        }
        else
        {
            FollowPlayer(); // Optional: keep following the player if no enemy found
        }
    }

    // Attack Player
    private IEnumerator AttackPlayer()
    {
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;

        animator.SetTrigger("isAttacking");

        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            PlayerController playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage((int)damageAmount);
            }
        }

        yield return new WaitForSeconds(1f);
        ResumeMovement();
    }

    // Attack Enemy when hacked
    private IEnumerator AttackEnemy(Transform enemy)
    {
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("isAttacking");

        LightGrunt enemyScript = enemy.GetComponent<LightGrunt>();
        if (enemyScript != null && !enemyScript.isAlly)
        {
            enemyScript.TakeDamage(damageAmount);
        }

        yield return new WaitForSeconds(1f);
        ResumeMovement();
    }

    private void ResumeMovement()
    {
        rb.velocity = Vector2.zero;
    }

    // Follow Player
    private void FollowPlayer()
    {
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > 1f)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        if (health > 0)
        {
            Vector2 knockbackDirection = (transform.position - GameObject.FindWithTag("Player").transform.position).normalized;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    public void HackEnemy()
    {
        isHackable = false;
        isAlly = true;
        isHacked = true;
        health = maxHealth;
        rb.velocity = Vector2.zero;

        // Ubah layer menjadi "Ally"
        gameObject.layer = LayerMask.NameToLayer("Ally"); // Ganti "Ally" dengan nama layer yang sesuai
        gameObject.tag = "Ally";
        animator.SetTrigger("isHacked");
        StartCoroutine(DestroyHackedEnemy());
    }

    private IEnumerator DestroyHackedEnemy()
    {
        yield return new WaitForSeconds(allyDuration);
        Die();
    }

    public IEnumerator DisableEnemy(float duration)
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

        yield return new WaitForSeconds(duration);

        isHacked = false;
        HackEnemy();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah objek yang bertabrakan adalah Ally (dengan tag "Ally")
        if (collision.gameObject.CompareTag("Ally"))
        {
            // Tambahkan logika yang ingin dijalankan ketika collisi terjadi
            Debug.Log("Bertabrakan dengan Ally: " + collision.gameObject.name);
            // Misalnya, Anda bisa melakukan sesuatu, seperti mengubah kecepatan, kesehatan, dll.
        }
    }
}
