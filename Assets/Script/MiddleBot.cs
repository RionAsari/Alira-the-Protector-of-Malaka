using System.Collections;
using UnityEngine;

public class MiddleBot : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isHacked = false;
    public HealthbarBehaviour healthbar;
    public GameObject hackedMiddleBotPrefab;

    private Animator animator;
    private BulletTransform bulletTransform; // Reference to BulletTransform

    public float followSpeed = 3f;
    private Transform currentTarget;

    public float detectionRange = 15f;  // Detection range for finding targets
    public float attackRange = 10f;     // Attack range for the MiddleBot
    public float attackCooldown = 1f;   // Time between attacks
    private float lastAttackTime = 0f;  // Tracks the last attack time

    public string allyTag = "Ally";
    public string playerTag = "Player";

    private Vector3 originalScale;
    private int hitCount = 0; // Counter for hits from SpecialArrow

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        // Get the BulletTransform component from the child object
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
    }

    private void HandleTargeting()
    {
        if (isDisabled) return;

        GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        GameObject nearestTarget = GetNearestTarget(allies, player);

        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) < detectionRange)
        {
            currentTarget = nearestTarget.transform;
        }
        else
        {
            currentTarget = null; // No target found
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange)
            {
                Attack(); // Attack if within range
            }
            else
            {
                ChaseTarget(); // Chase if outside of attack range
            }
        }
        else
        {
            // No target, stop walking
            animator.SetBool("isWalking", false);
        }
    }

    private GameObject GetNearestTarget(GameObject[] allies, GameObject player)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject ally in allies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, ally.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = ally;
            }
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestTarget = player;
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
        animator.SetBool("isWalking", true); // Set walking animation only when moving
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Call the ShootAtPlayer method from BulletTransform
            if (bulletTransform != null)
            {
                bulletTransform.ShootAtPlayer();
            }

            lastAttackTime = Time.time; // Update the last attack time
        }

        animator.SetBool("isWalking", false); // Stop walking animation
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x < 0) // If the target is to the left
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x > 0) // If the target is to the right
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

    // Increment hit count from SpecialArrow
    public bool IncrementHitCount()
    {
        hitCount++;
        if (hitCount >= 2)
        {
            hitCount = 0; // Reset hit count
            return true; // Indicate that MiddleBot can be disabled
        }
        return false; // Not yet disabled
    }

    private void SwitchToHackedMiddleBot()
    {
        // Instantiate the hacked MiddleBot prefab at the current position
        if (hackedMiddleBotPrefab != null)
        {
            Instantiate(hackedMiddleBotPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // Destroy this MiddleBot
    }
}
