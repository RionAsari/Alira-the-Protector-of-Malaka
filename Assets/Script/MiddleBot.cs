using System.Collections;
using UnityEngine;

public class MiddleBot : MonoBehaviour
{
    public float health = 500f;
    public float maxHealth = 500f;

    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isHacked = false;
    public HealthbarBehaviour healthbar;
    public GameObject hackedMiddleBotPrefab;

    private Animator animator;
    private BulletTransform bulletTransform; // Referensi ke BulletTransform

    public float followSpeed = 3f;
    private Transform currentTarget;

    public float detectionRange = 15f;  // Jarak deteksi target
    public float attackRange = 10f;     // Jarak serangan MiddleBot
    public float attackCooldown = 1f;   // Waktu jeda antar serangan
    private float lastAttackTime = 0f;

    public string allyTag = "Ally";
    public string playerTag = "Player";

    private Vector3 originalScale;
    private int hitCount = 0; // Penghitung hit dari SpecialArrow

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        // Mendapatkan komponen BulletTransform dari objek anak
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

        GameObject nearestTarget = GetNearestTargetWithTags(allyTag, playerTag);
        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) < detectionRange)
        {
            currentTarget = nearestTarget.transform;
            bulletTransform.SetTarget(currentTarget);  // Menetapkan target untuk BulletTransform
        }
        else
        {
            currentTarget = null; // Tidak ada target ditemukan
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange)
            {
                Attack(); // Serang jika berada dalam jarak
            }
            else
            {
                ChaseTarget(); // Mengejar jika di luar jarak serangan
            }
        }
        else
        {
            animator.SetBool("isWalking", false); // Berhenti berjalan jika tidak ada target
        }
    }

    // Menemukan target terdekat dari MiddleBot berdasarkan tag yang diberikan
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
        animator.SetBool("isWalking", true); // Menampilkan animasi berjalan hanya saat bergerak
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (bulletTransform != null)
            {
                bulletTransform.ShootAtTarget();
            }

            lastAttackTime = Time.time; // Memperbarui waktu serangan terakhir
        }

        if (currentTarget != null)
        {
            // Tambahkan ini untuk memastikan sprite menghadap target saat menyerang
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }

        animator.SetBool("isWalking", false); // Menghentikan animasi berjalan
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x < 0) // Jika target berada di kiri
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x > 0) // Jika target berada di kanan
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
            hitCount = 0; // Reset hit count
            return true; // Menandakan bahwa MiddleBot dapat dinonaktifkan
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
                Destroy(other.gameObject); // Destroy the projectile after dealing damage
            }
        }
    }
}
