using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    AudioManager audioManager;

    private bool canMove = false;
public LayerMask groundLayer;  // Set layer Ground dari inspector
    public AudioClip jumpSound;  // Suara saat melompat
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    private bool isAboveWall;
    private bool isTouchingLeftWall;
    public float ceilingCheckDistance = 0.5f;
    private bool isTouchingRightWall;
    private Vector2 wallCheckDirection;
    private bool isSFXPlaying = false;
    public AudioSource audioSource; // Reference to AudioSource
    public AudioClip dashSound;  // Suara saat melakukan dash

    // Tambahkan variabel untuk suara langkah
    public AudioClip footstepSound;  // Suara langkah kaki
    public AudioClip hackSound;  // Suara saat ngehack musuh

    
    private bool isWalking = false; // Untuk mengecek apakah pemain sedang berjalan

    private bool isMoving = false; // To track if the player is moving
    public float moveSpeed = 8f; 
    public float jumpForce = 8f; 
    public float fallMultiplier = 2.5f;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private Camera mainCamera;
    private bool usingSpecialArrow = false;

    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;

    private int jumpCount = 0;
    public int maxJumpCount = 2;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing = false;
    private bool isInvincible = false;
    private float lastDashTime = -10f;

    private int dashDirection = 0;
    private float doubleTapTime = 0.2f;
    private KeyCode lastKeyPressed;
    private float lastKeyTime;

    private Animator animator;

    private float moveInput;
    private float chargeWeight = 0f;
    private float chargeSpeed = 3f;

    public Health health;  // Tambahkan ini ke deklarasi variabel

    // Variabel untuk afterimage
    public GameObject afterImagePrefab; // Prefab afterimage
    public float afterImageInterval = 0.05f; // Interval antara setiap afterimage

    // Untuk pengelolaan warna sprite
    private SpriteRenderer[] spriteRenderers; // Array untuk menyimpan semua SpriteRenderer
    private Color[] originalColors; // Array untuk menyimpan warna asli

    // Tambahkan status pause
    public bool isPaused = false;

    // Variables for Touching Directions
    public ContactFilter2D castFilter; // Filter kontak untuk physics casts
    public float groundDistance = 0.05f; // Jarak dari collider ke tanah
    public float wallDistance = 0.05f; // Jarak ke dinding

    private CapsuleCollider2D touchingCol; // Collider untuk mendeteksi kontak
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];

    private bool isGrounded; // Apakah menyentuh tanah
    private bool isTouchingWall; // Apakah menyentuh dinding

    private bool IsOnWall; // Declare IsOnWall variable

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        // Mendapatkan semua SpriteRenderer yang ada di dalam karakter
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];

        // Menyimpan warna asli untuk setiap SpriteRenderer
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        touchingCol = GetComponent<CapsuleCollider2D>(); // Assign collider untuk Touching Directions

        // Cek jika audioSource ada, jika tidak maka tambahkan komponen AudioSource baru
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true; // Agar suara langkah bisa di-loop
    }

    private void Start()
    {
    
    }

    private void Update()
    {
        if (health.isDead)  // Check if the player is dead
    {
        StopAllAudio();  // Stop all sounds if the player is dead
        return;  // Prevent further input or actions
    }
        if (Input.GetKeyDown(KeyCode.F))
    {
        // Mainkan suara ngehack
        PlayHackSound();

        // Logika untuk ngehack musuh (misalnya mengubah status musuh menjadi 'Hacked')
        // Tambahkan kode hack musuh di sini
    }
        if (canMove)
        {
            // Allow player movement here
            float move = Input.GetAxis("Horizontal");
            transform.Translate(Vector3.right * move * Time.deltaTime);
        }
        isTouchingLeftWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, LayerMask.GetMask("WallLayer"));
    
        // Deteksi tembok di kanan
        isTouchingRightWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, LayerMask.GetMask("WallLayer"));
    

        // Deteksi tembok di atas pemain (berfungsi untuk memastikan pemain bisa melangkah ke atas tembok)
        isAboveWall = Physics2D.Raycast(transform.position, Vector2.down, wallCheckDistance, LayerMask.GetMask("WallLayer"));

        // Cek pergerakan
        Move();
        wallCheckDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Perform the wall check
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, wallCheckDirection, wallCheckDistance, wallLayer);
        if (wallHit.collider != null)
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }

        if (health.isDead)  // Cek apakah pemain mati
            return;  // Jika mati, hentikan semua input

        if (isPaused)
            return; // Jika game dipause, tidak ada input yang diproses

        // Update Touching Directions
        DetectEnvironment(); // Memanggil fungsi untuk cek lingkungan

        // Cek apakah pemain bergerak dan putar suara langkah
         if (moveInput != 0 && isGrounded)
        {
            if (!isWalking)
            {
                isWalking = true;
                PlayFootstepSound(); // Mulai mainkan suara langkah
            }
        }
        else if (!isGrounded)
        {
            if (isWalking)
            {
                isWalking = false;
                StopFootstepSound(); // Hentikan suara langkah
            }
        }
         else
    {
        if (isWalking)
        {
            isWalking = false;
            StopFootstepSound(); // Hentikan suara langkah jika tidak bergerak
        }
    }

        if (Input.GetKeyDown(KeyCode.X))
        {
            usingSpecialArrow = !usingSpecialArrow;
            Debug.Log("Using Special Arrow: " + usingSpecialArrow);
        }

        if (KBCounter <= 0)
        {
            if (!isDashing)
            {
                Move();
                HandleJump();
                AimAtMouse();
                UpdateAnimation();
                CheckDash();
            }
            else
            {
                rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
            }
        }
        else
        {
            if (KnockFromRight)
            {
                rb.velocity = new Vector2(-KBForce, KBForce);
            }
            else
            {
                rb.velocity = new Vector2(KBForce, KBForce);
            }
            KBCounter -= Time.deltaTime;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

private void PlayFootstepSound()
{
    // Pastikan footstep sound hanya diputar saat isGrounded dan tidak sedang bermain
    if (footstepSound != null && audioSource != null && !audioSource.isPlaying && isGrounded)
    {
        audioSource.clip = footstepSound;
        audioSource.Play();
    }
}

private void StopFootstepSound()
{
    // Hentikan suara langkah jika audioSource sedang bermain
    if (audioSource != null && audioSource.isPlaying)
    {
        audioSource.Stop();
    }
}


private void Move()
{
    if (health.isDead)
        return;

    if (!isDashing)
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Cek jika pemain bergerak ke arah tembok dengan tag "Wall"
        bool isBlockedLeft = IsWallInDirection(Vector2.left) && moveInput < 0; // Gerak ke kiri & ada tembok kiri
        bool isBlockedRight = IsWallInDirection(Vector2.right) && moveInput > 0; // Gerak ke kanan & ada tembok kanan

        // Hentikan pergerakan jika terhalang tembok
        if (isBlockedLeft || isBlockedRight)
        {
            moveInput = 0;
        }

        // Pergerakan horizontal
        Vector3 moveVelocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, 0);
        rb.velocity = moveVelocity;

        // Flip sprite jika arah berubah
        if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight))
        {
            Flip();
        }

        // Menampilkan bow saat bergerak
    }
}

// Fungsi untuk memeriksa apakah ada dinding di arah tertentu berdasarkan tag
private bool IsWallInDirection(Vector2 direction)
{
    RaycastHit2D[] hits = new RaycastHit2D[5];
    int hitCount = touchingCol.Cast(direction, castFilter, hits, wallCheckDistance);

    for (int i = 0; i < hitCount; i++)
    {
        if (hits[i].collider != null && hits[i].collider.CompareTag("Wall"))
        {
            return true;
        }
    }
    return false;
}


private void DetectEnvironment()
{
    // Ensure we are only checking the main player collider for grounding.
    Collider2D playerCollider = GetComponent<Collider2D>();
    int groundHitsCount = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance);

    for (int i = 0; i < groundHitsCount; i++)
    {
        // Exclude bow and only consider the player's main collider for grounding
        if (groundHits[i].collider != playerCollider && groundHits[i].collider.CompareTag("Ground"))
        {
            isGrounded = true;
            break;
        }
    }

    if (groundHitsCount == 0)
    {
        isGrounded = false;
    }

    animator.SetBool("isGrounded", isGrounded);
}


    private void UpdateAnimation()
{
    bool isCharging = Input.GetMouseButton(0);
    chargeWeight = Mathf.MoveTowards(chargeWeight, isCharging ? 1f : 0f, chargeSpeed * Time.deltaTime);

    // Mengatur nilai layer "ChargeLayer" di animator
    animator.SetLayerWeight(animator.GetLayerIndex("ChargeLayer"), chargeWeight);

    // Memastikan animasi bergerak saat ada input
    animator.SetBool("isMoving", moveInput != 0);

    // Memastikan animasi dash aktif
    animator.SetBool("isDashing", isDashing);

    // Menambahkan pengaturan YVelocity untuk mengetahui apakah pemain sedang Falling atau Rising
    animator.SetFloat("YVelocity", rb.velocity.y);
}


    private void AimAtMouse()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        if (mousePosition.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

private void HandleJump()
{
    // Reset jumpCount hanya saat pemain menyentuh tanah
    if (isGrounded)
    {
        // Pastikan pemain hanya reset jumpCount saat mendarat
        if (jumpCount > 0)
        {
            jumpCount = 0;
            animator.ResetTrigger("isJumping");  // Reset trigger saat berada di tanah
        }
    }

    // Memeriksa input lompat
    if (Input.GetKeyDown(KeyCode.W))
    {
        // Pastikan pemain bisa melompat hingga dua kali, meskipun tidak di tanah
        if (jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            animator.SetTrigger("isJumping");  // Menggunakan trigger untuk lompat

            // Play the jump sound
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);  // Mainkan suara lompat
            }
        }
    }
}


    private void CheckDash()
    {
        if (Time.time - lastDashTime > dashCooldown)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (lastKeyPressed == KeyCode.D && Time.time - lastKeyTime < doubleTapTime)
                {
                    StartDash(1);
                }
                else
                {
                    lastKeyPressed = KeyCode.D;
                    lastKeyTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (lastKeyPressed == KeyCode.A && Time.time - lastKeyTime < doubleTapTime)
                {
                    StartDash(-1);
                }
                else
                {
                    lastKeyPressed = KeyCode.A;
                    lastKeyTime = Time.time;
                }
            }
        }
    }

    private void StartDash(int direction)
    {
        dashDirection = direction;
        isDashing = true;
        isInvincible = true;
        gameObject.layer = LayerMask.NameToLayer("PlayerDash");
        animator.SetTrigger("StartDash");

        if (dashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 100f / 255f);
        }

        StartCoroutine(DashCoroutine());
        StartCoroutine(CreateAfterImage());
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            yield return null;
        }

        isDashing = false;
        isInvincible = false;
        lastDashTime = Time.time;
        gameObject.layer = LayerMask.NameToLayer("Player");

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }

    private IEnumerator CreateAfterImage()
    {
        while (isDashing)
        {
            GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            afterImage.transform.localScale = transform.localScale;

            yield return new WaitForSeconds(afterImageInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            KBCounter = KBTotalTime;
            KnockFromRight = other.transform.position.x >= transform.position.x;
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
    

public void TogglePause()
{
    isPaused = !isPaused;
    Time.timeScale = isPaused ? 0f : 1f;

    // Hentikan semua suara saat game dipause
    if (isPaused)
    {
        StopAllAudio();
    }
}

private void StopAllAudio()
{
    // Hentikan suara langkah
    if (audioSource.isPlaying)
    {
        audioSource.Stop();
    }
}

    public bool IsIdle()
{
    return moveInput == 0 && rb.velocity.y == 0 && !isDashing; // Idle jika tidak ada input gerakan, tidak sedang melompat/dash
}
public void EnableMovement(bool enable)
    {
        canMove = enable;
    }
    private void PlayHackSound()
{
    if (hackSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(hackSound);  // Mainkan suara hack
    }
}


}