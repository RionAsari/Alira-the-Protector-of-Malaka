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
    private HackedMiddlebotBulletTransform bulletTransform;
    private Vector3 originalScale;
    private Transform playerTransform;

    public float stopDistance = 2f;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        bulletTransform = GetComponentInChildren<HackedMiddlebotBulletTransform>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        originalScale = transform.localScale;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (health <= 0) return;

        UpdateHealthBar();
        HandleTargeting();
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

        GameObject nearestTarget = GetNearestTarget(enemies, middleBots);

        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) <= attackRange)
        {
            currentTarget = nearestTarget.transform;
            Attack();
        }
        else
        {
            currentTarget = playerTransform;
            ChaseTarget();
        }
    }

    private GameObject GetNearestTarget(GameObject[] enemies, GameObject[] middleBots)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestTarget = enemy;
            }
        }

        foreach (GameObject bot in middleBots)
        {
            float distanceToBot = Vector3.Distance(transform.position, bot.transform.position);
            if (distanceToBot < shortestDistance)
            {
                shortestDistance = distanceToBot;
                nearestTarget = bot;
            }
        }

        return nearestTarget;
    }

    private void ChaseTarget()
    {
        if (currentTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);

            transform.position += direction * followSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (bulletTransform != null)
            {
                bulletTransform.ShootAtTarget(currentTarget.position);
            }

            lastAttackTime = Time.time;
        }

        animator.SetBool("isWalking", false);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack") || other.CompareTag("Volley"))
        {
            float damage = 10f;

            TakeDamage(damage);
            Debug.Log($"HackedMiddleBot took {damage} damage from {other.gameObject.tag}");

            Destroy(other.gameObject);
        }
    }
}
