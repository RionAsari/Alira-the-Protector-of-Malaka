using System.Collections;
using UnityEngine;

public class MiddleBot : MonoBehaviour
{
    public float health = 500f;
    public float maxHealth = 500f;

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isHacked = false;
    public HealthBarMiddleBot healthbar;
    public GameObject hackedMiddleBotPrefab;

    private Animator animator;
    private BulletTransform bulletTransform;

    public float followSpeed = 3f;
    private Transform currentTarget;

    public float detectionRange = 15f;
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    public string allyTag = "Ally";
    public string playerTag = "Player";

    private Vector3 originalScale;
    private int hitCount = 0;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        bulletTransform = GetComponentInChildren<BulletTransform>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (health <= 0) return;

        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }

        if (isDisabled && isHackable)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SwitchToHackedMiddleBot();
            }
            return;
        }

        HandleTargeting();

        // Ensure sprite faces target every frame if there's a target
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }
    }

    private void HandleTargeting()
    {
        if (isDisabled) return;

        GameObject nearestTarget = GetNearestTargetWithTags(allyTag, playerTag);
        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) < detectionRange)
        {
            currentTarget = nearestTarget.transform;
            bulletTransform.SetTarget(currentTarget);
        }
        else
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange)
            {
                Attack();
            }
            else
            {
                ChaseTarget();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private GameObject GetNearestTargetWithTags(params string[] tags)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in tags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = target;
                }
            }
        }
        return nearestTarget;
    }

    private void ChaseTarget()
    {
        if (currentTarget == null || isDisabled) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        Vector3 direction = (currentTarget.position - transform.position).normalized;

        FlipSprite(direction);

        transform.position += direction * followSpeed * Time.deltaTime;
        animator.SetBool("isWalking", true);
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (bulletTransform != null)
            {
                bulletTransform.ShootAtTarget();
            }

            lastAttackTime = Time.time;
        }

        if (currentTarget != null)
        {
            // Ensure sprite faces target while attacking
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
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
        isDisabled = true;
        Destroy(gameObject, 0.2f);
    }

    public IEnumerator DisableEnemy(float duration)
    {
        isDisabled = true;
        isHackable = true;
        animator.SetTrigger("isDisabled");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(duration);

        rb.isKinematic = false;
        isDisabled = false;
        animator.SetTrigger("isReactivated");
    }

    public bool IncrementHitCount()
    {
        hitCount++;
        if (hitCount >= 2)
        {
            hitCount = 0;
            return true;
        }
        return false;
    }

    private void SwitchToHackedMiddleBot()
    {
        if (hackedMiddleBotPrefab != null)
        {
            Instantiate(hackedMiddleBotPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HackedVolley"))
        {
            HackedVolley hackedVolley = other.GetComponent<HackedVolley>();
            if (hackedVolley != null)
            {
                TakeDamage(hackedVolley.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
