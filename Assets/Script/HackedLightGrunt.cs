using System.Collections;
using UnityEngine;

public class HackedLightGrunt : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool canAttack = true;

    private void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectAndAttackEnemies();

        animator.SetBool("isWalking", rb.velocity.magnitude > 0);

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
            if (Vector2.Distance(transform.position, target.position) <= attackRange && canAttack)
            {
                Attack(target.gameObject);
            }
        }
    }

    private void Attack(GameObject enemy)
    {
        animator.SetTrigger("Attack");

        LightGrunt lightGrunt = enemy.GetComponent<LightGrunt>();
        if (lightGrunt != null)
                {
            lightGrunt.TakeDamage(attackDamage); // Inflict damage to the LightGrunt
        }

        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Avoid collision with player
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
