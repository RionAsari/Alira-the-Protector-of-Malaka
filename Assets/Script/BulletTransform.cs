using UnityEngine;
using System.Collections;

public class BulletTransform : MonoBehaviour
{
    public Transform target;  // Target untuk Player dan Ally
    public GameObject volleyPrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 3f; // Cooldown setelah semua proyektil ditembakkan
    public float volleyInterval = 0.2f; // Interval antar proyektil
    public int volleyCount = 4; // Jumlah proyektil yang akan ditembakkan

    public AudioClip shootSound; // Tambahkan audio clip untuk suara tembakan
    private AudioSource audioSource;
    public string playerTag = "Player"; // Tag untuk mencari objek player

    private float lastAttackTime = 0f;
    private Animator animator;
    private bool isShooting = false;

    private void Start()
    {
        // Mendapatkan komponen Animator dan AudioSource
        animator = GetComponentInParent<Animator>(); // Mengasumsikan BulletTransform adalah anak dari MiddleBot
        audioSource = GetComponent<AudioSource>(); // Mendapatkan AudioSource
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
        if (Time.time >= lastAttackTime + attackCooldown && target != null && !isShooting)
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

        // Loop untuk menembakkan beberapa proyektil dalam satu volley
        for (int i = 0; i < volleyCount; i++)
        {
            // Mainkan suara tembakan setiap kali proyektil ditembakkan
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
                AdjustAudioRelativeToPlayer();  // Sesuaikan suara berdasarkan posisi pemain
            }

            GameObject projectile = Instantiate(volleyPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }

            yield return new WaitForSeconds(volleyInterval); // Jeda antara setiap tembakan
        }

        lastAttackTime = Time.time; // Menyimpan waktu serangan terakhir
        isShooting = false; // Mengizinkan serangan setelah cooldown selesai
    }

    private void AdjustAudioRelativeToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag); // Find the player object
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
