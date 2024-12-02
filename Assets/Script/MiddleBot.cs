using System.Collections;
using UnityEngine;

public class MiddleBot : MonoBehaviour
{
    // AudioSource and sound clips
    private AudioSource audioSource;
    public AudioClip walkingSound; // Drag and drop the walking sound here in the inspector
    private bool isWalkingSoundPlaying = false;
    public AudioClip disableSound; // Drag and drop the disable sound here in the inspector
    private bool isDisableSoundPlaying = false;
    private bool hasPlayedDisableSound = false;
    public AudioClip dieSound; // Drag and drop the death sound here in the inspector
    public AudioClip damageSound;
    

    public float health = 500f;
    public static int activeEnemies = 0; // Variabel statis untuk menghitung musuh aktif
    public float maxHealth = 500f;
    public float hackRange = 2f;
    public bool isDisabled = false;
    public bool isHackable = false;
    public bool isHacked = false;
    public HealthBarMiddleBot healthbar;
    public GameObject hackedMiddleBotPrefab;

    private Animator animator;
    private BulletTransform bulletTransform;

    public float followSpeed = 3f;
    private Transform currentTarget;

    public float detectionRange = 15f;
    public float attackRange = 10f;

    public string allyTag = "Ally";
    public string playerTag = "Player";

    private Vector3 originalScale;
    private int hitCount = 0;

    // Prefab untuk healing items
    public GameObject healingItem20Prefab;
    public GameObject healingItem50Prefab;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        bulletTransform = GetComponentInChildren<BulletTransform>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
    {
        // Stop any playing sound when paused
        if (audioSource.isPlaying)
        {
            audioSource.Pause(); // Pause the sound if it's playing
        }
        return; // Exit early if the game is paused
    }
    else
    {
        // Resume sound if not paused
        if (!audioSource.isPlaying && isWalkingSoundPlaying)
        {
            audioSource.Play(); // Play sound if it's not playing and it should be
        }
    }
        AdjustAudioRelativeToPlayer();
        
        if (health <= 0) return;

        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }

        if (isDisabled && isHackable)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (Input.GetKeyDown(KeyCode.F) && distanceToPlayer <= hackRange)
                {
                    SwitchToHackedMiddleBot();
                }
            }
            return;
        }

        HandleTargeting();

        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }

        // Play walking sound when moving
        if (animator.GetBool("isWalking") && !isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = true;
            audioSource.clip = walkingSound;
            audioSource.loop = true; // Loop the walking sound while moving
            audioSource.Play();
        }
        else if (!animator.GetBool("isWalking") && isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = false;
            audioSource.Stop();
        }
    }

    private void HandleTargeting()
    {
        if (isDisabled) return;

        GameObject nearestTarget = GetNearestTargetWithTags(allyTag, playerTag);
        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) < detectionRange)
        {
            currentTarget = nearestTarget.transform;
            bulletTransform.SetTarget(currentTarget);
        }
        else
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange)
            {
                Attack();
            }
            else
            {
                ChaseTarget();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

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
        animator.SetBool("isWalking", true);
    }

    private void Attack()
    {
        if (bulletTransform != null)
        {
            bulletTransform.ShootAtTarget();
        }

        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }

        animator.SetBool("isWalking", false);
    }

    private void FlipSprite(Vector3 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);  
        }

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
    // Play death sound if available
    if (audioSource != null && dieSound != null)
    {
        // Create a temporary GameObject to hold the audio source for the death sound
        GameObject soundObject = new GameObject("DeathSound");
        AudioSource soundAudioSource = soundObject.AddComponent<AudioSource>();
        soundAudioSource.clip = dieSound;
        soundAudioSource.Play();

        // Make sure the sound persists after this object is destroyed
        DontDestroyOnLoad(soundObject);

        // Destroy the temporary sound object after the sound has finished playing
        Destroy(soundObject, dieSound.length);
    }

    animator.SetTrigger("isDead");
    isDisabled = true;

    if (isWalkingSoundPlaying && audioSource != null)
    {
        audioSource.Stop();
        isWalkingSoundPlaying = false;
    }

    DropHealingItem();

    // Destroy the MiddleBot prefab after a short delay
    Destroy(gameObject, 0.6f);
}


    private void DropHealingItem()
    {
        float dropChance = Random.Range(0f, 1f);

        if (dropChance <= 0.5f)
        {
            // 50% chance to drop 20 healing points item
            Instantiate(healingItem20Prefab, transform.position, Quaternion.identity);
        }
        else if (dropChance <= 0.7f)
        {
            // 20% chance to drop 50 healing points item
            Instantiate(healingItem50Prefab, transform.position, Quaternion.identity);
        }
        // 30% chance to drop nothing
    }

    public IEnumerator DisableEnemy(float duration)
    {
        isDisabled = true;
        isHackable = true;
        animator.SetTrigger("isDisabled");

        // Pastikan audioSource dan disableSound tidak null, kemudian mainkan suara hanya sekali
        if (audioSource != null && disableSound != null && !isDisableSoundPlaying)
        {
            audioSource.PlayOneShot(disableSound);
            isDisableSoundPlaying = true;  // Tandai bahwa suara telah dimainkan
        }

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

        // Reset flag suara setelah durasi selesai, agar bisa dimainkan lagi jika diperlukan
        isDisableSoundPlaying = false;
    }

    public bool IncrementHitCount()
    {
        hitCount++;
        if (hitCount >= 2)
        {
            hitCount = 0;
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        activeEnemies++; // Tambahkan ketika MiddleBot diaktifkan
    }

    private void OnDisable()
    {
        activeEnemies--; // Kurangi ketika MiddleBot dinonaktifkan
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
                Destroy(other.gameObject);
            }
        }
    }
    private void AdjustAudioRelativeToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && audioSource != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Adjust volume and pan based on the distance to the player
            audioSource.volume = Mathf.Clamp01(1f - (distanceToPlayer / 20f)); // Volume decreases with distance
            audioSource.panStereo = Mathf.Clamp((transform.position.x - player.transform.position.x) / 10f, -1f, 1f); // Stereo pan based on position
        }
    }
}
