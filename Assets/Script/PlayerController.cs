using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f; 
    public float jumpForce = 8f; 
    public float fallMultiplier = 2.5f;
    private Rigidbody2D rb;
    private bool isGrounded = true;
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
    public GameObject bowTransform;

    private float moveInput;
    private float chargeWeight = 0f;
    private float chargeSpeed = 3f;

    private bool canMoveLeft = true;
    private bool canMoveRight = true;
    public Health health;  // Tambahkan ini ke deklarasi variabel


    // Variabel untuk afterimage
    public GameObject afterImagePrefab; // Prefab afterimage
    public float afterImageInterval = 0.05f; // Interval antara setiap afterimage

    // Untuk pengelolaan warna sprite
    private SpriteRenderer[] spriteRenderers; // Array untuk menyimpan semua SpriteRenderer
    private Color[] originalColors; // Array untuk menyimpan warna asli

    // Tambahkan status pause
    public bool isPaused = false;

    private void Awake()
    {
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
    }

    private void Start()
    {
        ShowBow(true);
    }

    private void Update()
    {
            if (health.isDead)  // Cek apakah pemain mati
        return;  // Jika mati, hentikan semua input

        if (isPaused)
            return; // Jika game dipause, tidak ada input yang diproses

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

    private void Move()
    {
            if (health.isDead)  // Cek apakah pemain mati
        return;  // Jika mati, hentikan pergerakan

        if (!isDashing)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if ((moveInput < 0 && !canMoveLeft) || (moveInput > 0 && !canMoveRight))
            {
                moveInput = 0;
            }

            Vector3 moveVelocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, 0);
            rb.velocity = moveVelocity;

            if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight))
            {
                Flip();
            }

            ShowBow(true);
        }
    }

    public void ShowBow(bool show)
    {
        bowTransform.SetActive(show);
    }

    public bool IsIdle()
    {
        return moveInput == 0 && isGrounded;
    }

    private void UpdateAnimation()
    {
        bool isCharging = Input.GetMouseButton(0);
        chargeWeight = Mathf.MoveTowards(chargeWeight, isCharging ? 1f : 0f, chargeSpeed * Time.deltaTime);

        animator.SetLayerWeight(animator.GetLayerIndex("ChargeLayer"), chargeWeight);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isMoving", isGrounded && moveInput != 0);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);
        animator.SetBool("isDashing", isDashing);
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                jumpCount = 1;
                animator.SetBool("isJumping", true);
            }
            else if (jumpCount < maxJumpCount)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                animator.SetBool("isJumping", true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            animator.SetBool("isGrounded", true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 playerPosition = transform.position;

            if (contactPoint.x < playerPosition.x)
            {
                canMoveLeft = false;
            }
            else if (contactPoint.x > playerPosition.x)
            {
                canMoveRight = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            canMoveLeft = true;
            canMoveRight = true;
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

        // Ubah warna semua SpriteRenderer saat dash
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 100f / 255f); // Merah dengan alpha 100
        }

        StartCoroutine(DashCoroutine());
        StartCoroutine(CreateAfterImage()); // Mulai coroutine untuk membuat afterimages
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

        // Kembalikan warna semua SpriteRenderer setelah dash
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i]; // Kembalikan ke warna asli
        }
    }

    private IEnumerator CreateAfterImage()
    {
        while (isDashing)
        {
            GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            afterImage.transform.localScale = transform.localScale;

            yield return new WaitForSeconds(afterImageInterval); // Interval antara afterimages

            // Mencari dan menghancurkan afterimage yang sudah dibuat
            GameObject[] afterImages = GameObject.FindGameObjectsWithTag("AfterImage");
            foreach (GameObject img in afterImages)
            {
                Destroy(img); // Menghancurkan afterimage yang ditemukan
            }
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            KBCounter = KBTotalTime;
            KnockFromRight = other.transform.position.x >= transform.position.x;
        }
    }

    // Pause / Unpause Game
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; // Stop or resume the game
    }
    // Add these methods in the PlayerController script

public bool GetIsGrounded()
{
    return isGrounded;
}

public float GetMoveInput()
{
    return moveInput;
}

}
