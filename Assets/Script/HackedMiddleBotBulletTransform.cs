using UnityEngine;

public class HackedMiddlebotBulletTransform : MonoBehaviour
{
    public Transform target; // Target for hacked MiddleBot (Enemy or MiddleBot)
    public GameObject volleyPrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 2f;

    private float lastAttackTime = 0f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        FindTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
        }
        else
        {
            AimAtTarget();
        }
    }

    private void FindTarget()
    {
        // Find the nearest target with tag "Enemy" or "MiddleBot"
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (GameObject potentialTarget in potentialTargets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = potentialTarget.transform;
            }
        }

        // If no "Enemy" is found, look for "MiddleBot" targets
        if (nearestTarget == null)
        {
            GameObject[] middleBots = GameObject.FindGameObjectsWithTag("MiddleBot");
            foreach (GameObject bot in middleBots)
            {
                float distanceToBot = Vector3.Distance(transform.position, bot.transform.position);
                if (distanceToBot < shortestDistance)
                {
                    shortestDistance = distanceToBot;
                    nearestTarget = bot.transform;
                }
            }
        }

        target = nearestTarget;
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

    // Change this method to take a Vector3 position parameter
    public void ShootAtTarget(Vector3 targetPosition)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (targetPosition - transform.position).normalized; // Use targetPosition instead of target.position
                rb.velocity = direction * projectileSpeed;
            }

            lastAttackTime = Time.time;
        }
    }
}
