using System.Collections;
using UnityEngine;

public class LightGrunt : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public LayerMask enemyLayer;

    public bool isDisabled = false;
    public bool isHackable = false; // This is set to false until disabled
    public bool isHacked = false; // To check if it's hacked or not

    public HealthbarBehaviour healthbar;
    public GameObject hackedLightGruntPrefab;
    private Rigidbody2D rb;
    private Animator animator;
    private Coroutine patrolCoroutine;
    private bool canAttack = true;

    public virtual void Start()
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

    public virtual void Update()
    {
        if (health <= 0) return;

        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }

        if (isHacked)
        {
            // If hacked, detect enemies
            DetectAndAttackEnemies();
        }
        else if (!isDisabled)
        {
            // If not hacked, detect the player
            DetectAndAttackPlayer();
        }

        if (Input.GetKeyDown(KeyCode.F) && isDisabled && isHackable)
        {
            SwitchToHackedLightGrunt();
        }

        // Update walking animations
        animator.SetBool("isWalking", rb.velocity.magnitude > 0);

        // Flip character
        if (rb.velocity.x != 0)
        {
            UpdateFacingDirection(rb.velocity.x);
        }
    }

    private void UpdateFacingDirection(float moveDirectionX)
    {
        if (moveDirectionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirectionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator Patrol()
    {
        while (!isDisabled && !isHacked)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f);

            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            yield return new WaitForSeconds(2f);
        }
    }

    private void DetectAndAttackEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
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
        }
    }

    private void DetectAndAttackPlayer()
    {
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange)
        {
            if (distanceToPlayer < attackRange)
            {
                if (canAttack)
                {
                    Attack(playerTransform.gameObject);
                }
            }
            else if (distanceToPlayer < attackRange + 1f)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
                animator.SetBool("isAttacking", false);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Attack(GameObject target)
    {
        animator.SetTrigger("Attack");
        animator.SetBool("isAttacking", true);

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)attackDamage);
        }

        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        animator.SetBool("isAttacking", false);
    }

    public virtual void TakeDamage(float damage)
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

    protected void Die()
    {
        animator.SetTrigger("isDead");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    public IEnumerator DisableEnemy(float duration)
    {
        isDisabled = true;
        isHackable = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        animator.SetTrigger("isDisabled");
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        isDisabled = false;
        rb.isKinematic = false;
        GetComponent<Collider2D>().enabled = true;
    }

// In LightGrunt.cs when transforming to HackedLightGrunt
private void SwitchToHackedLightGrunt()
{
    if (hackedLightGruntPrefab != null)
    {
        // Instantiate HackedLightGrunt at the same position
        GameObject hackedGrunt = Instantiate(hackedLightGruntPrefab, transform.position, transform.rotation);
        hackedGrunt.tag = "Ally"; // Change the tag to 'Ally' after hacking

        // Ensure it's no longer hackable and any other setup for HackedLightGrunt
        Destroy(gameObject); // Destroy the current LightGrunt
    }
}

}
