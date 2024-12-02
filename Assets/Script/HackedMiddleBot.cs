using System.Collections;
using UnityEngine;

public class HackedMiddleBot : MonoBehaviour
{
    // AudioSource dan sound clips
    private AudioSource audioSource;
    public AudioClip walkingSound;
    private bool isWalkingSoundPlaying = false;
    public AudioClip dieSound; // Tidak ada disableSound untuk HackedMiddleBot
    public AudioClip damageSound;
    

    public float health = 500f;
    public float maxHealth = 500f;
    public HealthBarMiddleBot healthbar;

    public float followSpeed = 3f;
    public float detectionRange = 15f;
    public float attackRange = 10f;
    private Transform currentTarget;
    private Animator animator;
    private HackedMiddlebotBulletTransform bulletTransform;
    private Vector3 originalScale;
    private Transform playerTransform;

    public float stopDistance = 2f;
private void Awake()
{
    animator = GetComponent<Animator>();
}
    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        bulletTransform = GetComponentInChildren<HackedMiddlebotBulletTransform>();
        audioSource = GetComponent<AudioSource>();

        // Set health bar initial value
        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        originalScale = transform.localScale;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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

        UpdateHealthBar();
        HandleTargeting();

        // Play walking sound
        if (animator.GetBool("isWalking") && !isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = true;
            audioSource.clip = walkingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (!animator.GetBool("isWalking") && isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = false;
            audioSource.Stop();
        }

        // Ensure sprite faces target every frame if there's a target
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
            healthbar.SetHealth(health, maxHealth);
        }
    }

    private void HandleTargeting()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] middleBots = GameObject.FindGameObjectsWithTag("MiddleBot");

        GameObject nearestTarget = GetNearestTarget(enemies, middleBots);

        if (nearestTarget != null && Vector3.Distance(transform.position, nearestTarget.transform.position) <= attackRange)
        {
            currentTarget = nearestTarget.transform;
            Attack();
        }
        else
        {
            currentTarget = playerTransform;
            ChaseTarget();
        }
    }

    private GameObject GetNearestTarget(GameObject[] enemies, GameObject[] middleBots)
    {
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestTarget = enemy;
            }
        }

        foreach (GameObject bot in middleBots)
        {
            float distanceToBot = Vector3.Distance(transform.position, bot.transform.position);
            if (distanceToBot < shortestDistance)
            {
                shortestDistance = distanceToBot;
                nearestTarget = bot;
            }
        }

        return nearestTarget;
    }

    private void ChaseTarget()
    {
        if (currentTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            FlipSprite(direction);

            transform.position += direction * followSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    private void OnEnable()
{
    if (animator != null)
    {
        animator.SetTrigger("WakeUp");
    }
}

    private void Attack()
    {
        if (bulletTransform != null)
        {
            bulletTransform.ShootAtTarget();
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

        // Play damage sound
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play die sound
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        animator.SetTrigger("isDead");
        Destroy(gameObject, 0.2f);
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
