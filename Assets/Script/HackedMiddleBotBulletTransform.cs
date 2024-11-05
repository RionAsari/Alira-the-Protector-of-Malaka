using UnityEngine;
using System.Collections;

public class HackedMiddlebotBulletTransform : MonoBehaviour
{
    public Transform target;
    public GameObject volleyPrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 3f; // Cooldown setelah semua proyektil ditembakkan
    public float volleyInterval = 0.2f; // Interval antar proyektil
    public int volleyCount = 4; // Jumlah proyektil yang akan ditembakkan

    private float lastAttackTime = 0f;
    private Animator animator;
    private bool isShooting = false;

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

    public void ShootAtTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !isShooting)
        {
            StartCoroutine(ShootVolley());
        }
    }

private IEnumerator ShootVolley()
{
    isShooting = true; // Mencegah serangan berulang selama coroutine berjalan

    if (animator != null)
    {
        animator.SetTrigger("Attack");
    }

    // Simpan posisi target terakhir jika target hancur di tengah serangan
    Vector3 lastKnownTargetPosition = target != null ? target.position : transform.position;

    // Loop untuk menembakkan beberapa proyektil dalam satu volley
    for (int i = 0; i < volleyCount; i++)
    {
        // Cek apakah target masih ada, jika tidak, gunakan posisi terakhir yang diketahui
        Vector3 shootTargetPosition = target != null ? target.position : lastKnownTargetPosition;

        GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 direction = (shootTargetPosition - transform.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }

        yield return new WaitForSeconds(volleyInterval); // Jeda antara setiap tembakan
    }

    lastAttackTime = Time.time; // Menyimpan waktu serangan terakhir
    isShooting = false; // Mengizinkan serangan setelah cooldown selesai
}

}
