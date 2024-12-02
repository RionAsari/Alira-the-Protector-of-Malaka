using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackedLightGrunt : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar;
    public float followDistance = 3f;
    public float followSpeed = 3f;
    public float attackRange = 1.5f;
    public float enemyDetectRange = 5f;
    public float attackCooldown = 2f;
    public float damage = 10f;

    public AudioClip walkSound;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;
    public AudioClip attackSound; // Suara serangan


    private Animator animator;
    private Transform player;
    private Transform currentTarget;
    private float lastAttackTime;
    private Vector3 originalScale;

    private Collider2D playerCollider;
    private Collider2D enemyCollider;

    private bool isPlayingWalkSound = false;
    private bool hasPlayedDeathSound = false; // Flag to prevent multiple death sounds
    
    private void Awake()
{
    animator = GetComponent<Animator>();
}
    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        playerCollider = player.GetComponent<Collider2D>();
        enemyCollider = GetComponent<Collider2D>();

        if (playerCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, enemyCollider);
        }

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        lastAttackTime = Time.time;

        originalScale = transform.localScale;
    }

    private void Update()
    {
        UpdateAnimation();
        DetectEnemyOrFollowPlayer();
        UpdateHealthBar();
        AdjustAudioRelativeToPlayer();
        if (Time.timeScale == 0) // If the game is paused
    {
        if (isPlayingWalkSound && audioSource != null)
        {
            audioSource.Pause();  // Pause the audio
            isPlayingWalkSound = false;
        }
    }
    else // When game is unpaused
    {
        if (!isPlayingWalkSound && audioSource != null && walkSound != null)
        {
            audioSource.Play();  // Resume the audio
            isPlayingWalkSound = true;
        }
    }
    }
    

    private void UpdateAnimation()
    {
        animator.SetBool("isWalking", false);
    }

    private void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.UpdatePosition(transform.position);
        }
    }
    private void OnEnable()
{
    if (animator != null)
    {
        animator.SetTrigger("WakeUp");
    }
}

    private void DetectEnemyOrFollowPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] middleBots = GameObject.FindGameObjectsWithTag("MiddleBot");

        float nearestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < nearestDistance && distanceToTarget <= enemyDetectRange)
            {
                nearestDistance = distanceToTarget;
                currentTarget = target.transform;
            }
        }

        foreach (GameObject middleBot in middleBots)
        {
            float distanceToMiddleBot = Vector3.Distance(transform.position, middleBot.transform.position);
            if (distanceToMiddleBot < nearestDistance && distanceToMiddleBot <= enemyDetectRange)
            {
                nearestDistance = distanceToMiddleBot;
                currentTarget = middleBot.transform;
            }
        }

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                AttackTarget();
            }
            else
            {
                ChaseTarget();
            }
        }
        else
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > followDistance)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
                else if (direction.x < 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }

                transform.position += direction * followSpeed * Time.deltaTime;

                animator.SetBool("isWalking", true);

                if (!isPlayingWalkSound && audioSource != null && walkSound != null)
                {
                    audioSource.clip = walkSound;
                    audioSource.loop = true;
                    audioSource.Play();
                    isPlayingWalkSound = true;
                }
            }
            else
            {
                animator.SetBool("isWalking", false);

                if (isPlayingWalkSound && audioSource != null)
                {
                    audioSource.Stop();
                    isPlayingWalkSound = false;
                }
            }
        }
    }

    private void ChaseTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }

            transform.position += direction * followSpeed * Time.deltaTime;

            animator.SetBool("isWalking", true);
        }
    }

    private void AttackTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("isAttacking");

            if (currentTarget != null)
            {
                if (currentTarget.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
                else if (currentTarget.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
                if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
            }

            if (currentTarget != null)
            {
                if (currentTarget.CompareTag("Enemy"))
                {
                    LightGrunt enemyScript = currentTarget.GetComponent<LightGrunt>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(damage);
                    }
                }
                else if (currentTarget.CompareTag("MiddleBot"))
                {
                    MiddleBot middleBotScript = currentTarget.GetComponent<MiddleBot>();
                    if (middleBotScript != null)
                    {
                        middleBotScript.TakeDamage(damage);
                    }
                }
            }

            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (audioSource != null && takeDamageSound != null)
        {
            audioSource.PlayOneShot(takeDamageSound);
        }

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        if (health <= 0 && !hasPlayedDeathSound)  // Ensure the sound only plays once
        {
            Die();
        }
    }

    private void Die()
    {
        if (audioSource != null && deathSound != null && !hasPlayedDeathSound)
        {
            GameObject soundObject = new GameObject("DeathSound");
            AudioSource soundAudioSource = soundObject.AddComponent<AudioSource>();
            soundAudioSource.clip = deathSound;
            soundAudioSource.Play();

            DontDestroyOnLoad(soundObject);
            Destroy(soundObject, deathSound.length);

            hasPlayedDeathSound = true; // Set flag to true after sound plays
        }

        animator.SetTrigger("isDead");

        if (isPlayingWalkSound && audioSource != null)
        {
            audioSource.Stop();
            isPlayingWalkSound = false;
        }

        Destroy(gameObject, 0.2f);
    }
    private void AdjustAudioRelativeToPlayer()
{
    if (player == null || audioSource == null) return;

    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // Sesuaikan volume berdasarkan jarak
    float maxDistance = 20f; // Jarak maksimum suara terdengar
    float volume = Mathf.Clamp01(1 - (distanceToPlayer / maxDistance)); // Volume berkurang sesuai jarak
    audioSource.volume = volume;

    // Sesuaikan arah suara (kiri/kanan)
    float pan = Mathf.Clamp((transform.position.x - player.position.x) / maxDistance, -1f, 1f);
    audioSource.panStereo = pan; // -1 = kiri, 0 = tengah, 1 = kanan
}

}
