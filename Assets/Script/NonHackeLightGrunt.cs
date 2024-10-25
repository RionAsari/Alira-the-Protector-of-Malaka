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

        HandleTargeting();
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
        if (currentTarget == null || isDisabled) return;

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
            // Set walking ke false sebelum menyerang
            animator.SetBool("isWalking", false);
            animator.SetTrigger("isAttacking");
            lastAttackTime = Time.time;

            // Flip sprite berdasarkan posisi target
            FlipSprite(currentTarget.position - transform.position);

            // Jika target bertag "Ally", serang ally
            if (currentTarget != null && currentTarget.CompareTag("Ally"))
            {
                HackedLightGrunt allyScript = currentTarget.GetComponent<HackedLightGrunt>();
                if (allyScript != null)
                {
                    allyScript.TakeDamage(10);  // Berikan damage pada HackedLightGrunt
                }
            }

            // Jika target bertag "Player", serang player
            if (currentTarget != null && currentTarget.CompareTag("Player"))
            {
                // Coba dapatkan komponen Health jika ada
                var playerHealth = currentTarget.GetComponent<Health>();  // Asumsi player memiliki komponen Health
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10);  // Berikan damage pada player
                }
            }

            // Kembali ke idle setelah menyerang
            animator.SetBool("isWalking", false);
        }
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x > 0) // Jika target ada di kanan
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);  // Balik ke kanan
        }
        else if (direction.x < 0) // Jika target ada di kiri
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);  // Tetap menghadap kiri
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

    // Method to handle damage from HackedVolley
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with a HackedVolley projectile
        if (other.CompareTag("HackedVolley"))
        {
            HackedVolley hackedVolley = other.GetComponent<HackedVolley>();
            if (hackedVolley != null)
            {
                TakeDamage(hackedVolley.damage); // Take damage from the HackedVolley
                Debug.Log($"LightGrunt hit by HackedVolley. Damage taken: {hackedVolley.damage}");
            }

            Destroy(other.gameObject); // Destroy the HackedVolley projectile after hitting
        }
    }
}
