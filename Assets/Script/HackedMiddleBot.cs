using System.Collections;
using UnityEngine;

public class HackedMiddleBot : MonoBehaviour
{
    public float health = 500f;
    public float maxHealth = 500f;
    public HealthBarMiddleBot healthbar;

    public float followSpeed = 3f;
    public float detectionRange = 15f;
    public float attackRange = 10f;
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

        // Set the health bar's initial value
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

        UpdateHealthBar();  // Update health bar position and value
        HandleTargeting();

        // Ensure sprite faces target every frame if there's a target
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);  // Update health bar position to follow the bot
            healthbar.SetHealth(health, maxHealth);  // Update health bar's displayed health
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
        if (bulletTransform != null)
        {
            bulletTransform.ShootAtTarget(); // Mengubah untuk memanggil ShootAtTarget tanpa parameter
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

        UpdateHealthBar();  // Call to update health bar after taking damage

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

            Destroy(other.gameObject);
        }
    }
}
