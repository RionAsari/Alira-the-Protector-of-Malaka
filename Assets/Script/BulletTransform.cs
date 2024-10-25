using UnityEngine;

public class BulletTransform : MonoBehaviour
{
    public Transform target;  // target untuk Player dan Ally
    public GameObject volleyPrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 2f;

    private float lastAttackTime = 0f;
    private Animator animator;

    private void Start()
    {
        // Mendapatkan komponen Animator
        animator = GetComponentInParent<Animator>(); // Mengasumsikan BulletTransform adalah anak dari MiddleBot
    }

    // Metode untuk menetapkan target dari MiddleBot
    public void SetTarget(Transform newTarget) 
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target != null)
        {
            AimAtTarget();
        }
    }

    private void AimAtTarget()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void ShootAtTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown && target != null)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }

            lastAttackTime = Time.time;
        }
    }
}
