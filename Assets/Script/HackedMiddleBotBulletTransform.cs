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
    public float initialAttackDelay = 1f; // Delay 1 detik sebelum bisa menyerang pertama kali

    public AudioClip shootSound; // Klip audio untuk suara tembakan
    private AudioSource audioSource; // Komponen AudioSource

    private float lastAttackTime = 0f;
    private Animator animator;
    private bool isShooting = false;
    private bool initialDelayDone = false; // Flag untuk delay awal

    private void Start()
    {
        // Mendapatkan komponen Animator dan AudioSource
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Komponen AudioSource pada GameObject yang sama
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
            AdjustAudioRelativeToPlayer(); // Adjust audio position each frame
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
        if (!initialDelayDone)
        {
            StartCoroutine(InitialDelay()); // Mulai delay 1 detik pertama kali
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown && !isShooting)
        {
            StartCoroutine(ShootVolley());
        }
    }

    private IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(initialAttackDelay);
        initialDelayDone = true; // Delay 1 detik selesai, sekarang bisa menyerang
    }

    private IEnumerator ShootVolley()
    {
        isShooting = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Vector3 lastKnownTargetPosition = target != null ? target.position : transform.position;

        for (int i = 0; i < volleyCount; i++)
        {
            Vector3 shootTargetPosition = target != null ? target.position : lastKnownTargetPosition;

            // Mainkan suara tembakan
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (shootTargetPosition - transform.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }

            yield return new WaitForSeconds(volleyInterval);
        }

        lastAttackTime = Time.time;
        isShooting = false;
    }

    // Adjust audio based on player position
    private void AdjustAudioRelativeToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || audioSource == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Adjust volume based on distance
        float maxDistance = 20f; // Maximum distance at which the sound is still heard clearly
        float volume = Mathf.Clamp01(1 - (distanceToPlayer / maxDistance)); // Volume decreases with distance
        audioSource.volume = volume;

        // Adjust the sound direction (left/right) based on the position of the player
        float pan = Mathf.Clamp((transform.position.x - player.transform.position.x) / maxDistance, -1f, 1f);
        audioSource.panStereo = pan; // -1 = left, 0 = center, 1 = right
    }
}
