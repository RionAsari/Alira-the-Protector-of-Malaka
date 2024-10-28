using System.Collections;
using UnityEngine;

public class LightGrunt : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isHacked = false;
    public HealthbarBehaviour healthbar;
    public GameObject hackedLightGruntPrefab;

    private Animator animator;
    private bool isAttacking = false;

    public float attackRange = 1.5f;
    public float followSpeed = 3f;
    private Transform currentTarget;

    public float detectionRange = 5f;

    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    public string allyTag = "Ally";
    public string playerTag = "Player"; // Tag player untuk deteksi

    private Vector3 originalScale; // Simpan skala asli

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        // Simpan skala asli sprite
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
                SwitchToHackedLightGrunt();
            }
            return;
        }

        // Pastikan tidak mengejar target saat menyerang
        if (!isAttacking)
        {
            HandleTargeting();
        }
    }

    private void HandleTargeting()
    {
        if (isDisabled) return;

        // Cari semua ally dan player
        GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
        GameObject player = GameObject.FindGameObjectWithTag(playerTag); // Temukan player berdasarkan tag

        // Temukan target terdekat antara ally dan player
        GameObject nearestTarget = GetNearestTarget(allies, player);

        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) < detectionRange)
        {
            currentTarget = nearestTarget.transform;
        }
        else
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            // Jika dalam jarak serang, serang target
            if (distanceToTarget <= attackRange)
            {
                AttackTarget();
            }
            else
            {
                // Kejar target jika tidak dalam jarak serang
                ChaseTarget();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private GameObject GetNearestTarget(GameObject[] allies, GameObject player)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        // Cari ally terdekat
        foreach (GameObject ally in allies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, ally.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = ally;
            }
        }

        // Periksa apakah player lebih dekat daripada ally
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
        if (currentTarget == null || isDisabled || isAttacking) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget > attackRange)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;

            // Flip sprite berdasarkan arah gerakan
            FlipSprite(direction);

            transform.position += direction * followSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void AttackTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
            animator.SetTrigger("isAttacking");
            lastAttackTime = Time.time;

            FlipSprite(currentTarget.position - transform.position);

            if (currentTarget != null && currentTarget.CompareTag("Ally"))
            {
                HackedLightGrunt hackedLightGrunt = currentTarget.GetComponent<HackedLightGrunt>();
                if (hackedLightGrunt != null)
                {
                    hackedLightGrunt.TakeDamage(10);
                }
                else
                {
                    HackedMiddleBot hackedMiddleBot = currentTarget.GetComponent<HackedMiddleBot>();
                    if (hackedMiddleBot != null)
                    {
                        hackedMiddleBot.TakeDamage(10);
                    }
                }
            }

            if (currentTarget != null && currentTarget.CompareTag("Player"))
            {
                var playerHealth = currentTarget.GetComponent<Health>();
                var playerController = currentTarget.GetComponent<PlayerController>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10);
                }

                if (playerController != null)
                {
                    // Apply knockback to the player
                    playerController.KBCounter = playerController.KBTotalTime;
                    playerController.KnockFromRight = currentTarget.position.x < transform.position.x;
                }
            }

            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x < 0)
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

    private void SwitchToHackedLightGrunt()
    {
        GameObject hackedGrunt = Instantiate(hackedLightGruntPrefab, transform.position, transform.rotation);
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
                Debug.Log($"LightGrunt hit by HackedVolley. Damage taken: {hackedVolley.damage}");
            }

            Destroy(other.gameObject);
        }
    }
}