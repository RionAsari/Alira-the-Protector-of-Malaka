using UnityEngine;

public class BulletTransform : MonoBehaviour
{
    public Transform player;
    public GameObject volleyPrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 2f;

    private float lastAttackTime = 0f;
    private Animator animator; // Reference to Animator

    private void Start()
    {
        // Get the Animator component
        animator = GetComponentInParent<Animator>(); // Assuming BulletTransform is a child of MiddleBot
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }
        else
        {
            AimAtPlayer();
            // No need to call ShootAtPlayer here. It will be called from the MiddleBot's Attack method.
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void AimAtPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void ShootAtPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Set the attack trigger
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }

            lastAttackTime = Time.time;
        }
    }
}
