using System.Collections;
using UnityEngine;

public class HackedMiddleBot : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar;

    public float followSpeed = 3f;
    public float detectionRange = 15f;
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private Transform currentTarget;
    private Animator animator;
    private HackedMiddlebotBulletTransform bulletTransform; // Reference to HackedMiddlebotBulletTransform
    private Vector3 originalScale;
    private Transform playerTransform; // Reference to the player

    // New variable for stopping distance
    public float stopDistance = 2f;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        bulletTransform = GetComponentInChildren<HackedMiddlebotBulletTransform>(); // Obtain HackedMiddlebotBulletTransform component

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        originalScale = transform.localScale;

        // Find the player transform (assumes the player has a tag "Player")
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (health <= 0) return;

        UpdateHealthBar();
        HandleTargeting(); // Detect and handle target behavior
    }

    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }
    }

    private void HandleTargeting()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] middleBots = GameObject.FindGameObjectsWithTag("MiddleBot");

        // Check for enemies or MiddleBots within attack range
        GameObject nearestTarget = GetNearestTarget(enemies, middleBots);

        // If there's a target within attack range, attack it
        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) <= attackRange)
        {
            currentTarget = nearestTarget.transform;
            Attack();
        }
        else
        {
            // If no target, follow the player
            currentTarget = playerTransform; // Set current target to player
            ChaseTarget();
        }
    }

    private GameObject GetNearestTarget(GameObject[] enemies, GameObject[] middleBots)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        // Check all enemies
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestTarget = enemy;
            }
        }

        // Check all MiddleBots
        foreach (GameObject middleBot in middleBots)
        {
            float distanceToMiddleBot = Vector3.Distance(transform.position, middleBot.transform.position);
            if (distanceToMiddleBot < shortestDistance)
            {
                shortestDistance = distanceToMiddleBot;
                nearestTarget = middleBot;
            }
        }

        return nearestTarget;
    }

    private void ChaseTarget()
    {
        if (currentTarget == null) return;

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.position);

        // Check if within stop distance
        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);

            transform.position += direction * followSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true); // Set walking animation when moving
        }
        else
        {
            animator.SetBool("isWalking", false); // Stop walking animation if close to the player
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Call the shooting method from HackedMiddlebotBulletTransform
            if (bulletTransform != null)
            {
                bulletTransform.ShootAtTarget(currentTarget.position); // Shoot at target position
            }

            lastAttackTime = Time.time; // Update last attack time
        }

        animator.SetBool("isWalking", false); // Stop walking animation during attack
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
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

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        Destroy(gameObject, 0.2f);
    }

    // Optional: Ignore collision with player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
