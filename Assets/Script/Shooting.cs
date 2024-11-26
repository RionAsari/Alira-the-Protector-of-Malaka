using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Slider cooldownSlider;
        public Sprite normalArrowSprite; // Sprite untuk panah normal
    public Sprite specialArrowSprite; // Sprite untuk panah spesial
    public Image arrowTypeUI; // UI Image untuk menampilkan jenis panah
        private bool isSpecialArrowOnCooldown = false;
    private float specialArrowCooldownTimer = 0f;
    public float specialArrowCooldown = 10f; // Waktu cooldown panah spesial
    
    private Animator bowAnimator; // Reference to the bow's Animator
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform playerTransform;

    public GameObject arrowPrefab; // Prefab for the normal arrow
    public GameObject specialArrowPrefab; // Prefab for the special arrow
    public Transform bowTransform; // Position where arrows are spawned
    public Transform rotatePoint; // GameObject that serves as rotation point
    public float rotationDistance = 1.5f; // Offset distance from rotatePoint
    public float maxArrowSpeed = 15f; // Maximum arrow speed (constant)
    public float chargeSpeed = 5f; // Speed of charging (how fast charge builds)
    
    private float currentCharge = 0f; // Amount of charge
    private bool isCharging = false; // Is the player currently charging?
    private bool canFire = true;
    private float timer;
    public float timeBetweenFiring = 0.5f; // Cooldown time between firing
    private PlayerController playerController;
    private Health playerHealth; // Reference to the Health script

    // UI Slider for charge bar
    public Slider chargeSlider; // Reference to the slider
    private const float maxChargeTime = 2.5f; // Change time needed for full charge to 2.5 seconds

    // Damage values
    public float baseDamage = 25f; // Damage for 1%-49%
    public float midDamage = 50f; // Damage for 50%-99%
    public float maxDamage = 100f; // Damage for 100%

    private bool usingSpecialArrow = false; // To check if the player is using special arrows

    // Offset for slider position
    public Vector3 sliderOffset = new Vector3(0, 2, 0); // Offset to place slider above player

    // Add a reference to the AudioManager and Sound Effects
    public AudioClip bowChargeSound;  // Suara saat mengisi daya bow
    public AudioClip bowReleaseSound; // Suara saat melepas anak panah

    private AudioSource audioSource; // Sumber suara untuk memutar suara

    private void Start()
    {
        if (arrowTypeUI != null)
        {
            arrowTypeUI.sprite = normalArrowSprite;
        }
        mainCam = Camera.main; // Get the main camera reference
        playerTransform = transform.parent; // Get the reference of the player (parent)
        playerController = playerTransform.GetComponent<PlayerController>(); // Get PlayerController
        playerHealth = playerTransform.GetComponent<Health>(); // Get Health script

        // Reset slider at the start
        if (chargeSlider != null)
        {
            chargeSlider.value = 0f; // Set slider to 0 initially
            chargeSlider.maxValue = 1f; // Slider range between 0 and 1
            chargeSlider.gameObject.SetActive(false); // Hide slider at the start
        }

        // Get reference to the bow's Animator
        bowAnimator = bowTransform.GetComponent<Animator>();

        // Get the AudioSource component for playing sounds
        audioSource = GetComponent<AudioSource>(); // Pastikan komponen AudioSource ada pada objek yang sama
    }

    private void Update()
    {
        
        // Check if player is dead
        if (playerHealth.isDead || playerController.isPaused) return; // If dead or game is paused, stop Update

        // Get mouse position in world coordinates
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Set z to 0 since we're in 2D

        Vector3 direction = mousePos - rotatePoint.position; // Calculate direction from rotatePoint
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate rotation

        // Flip rotation if player is facing left
        if (playerTransform.localScale.x < 0)
        {
            rotationZ += 180;
        }

        // Calculate new position for bow based on rotation and offset distance
        Vector3 bowPosition = rotatePoint.position + direction.normalized * rotationDistance;

        // Set bow position and rotation
        bowTransform.position = bowPosition; // Set bow position
        bowTransform.rotation = Quaternion.Euler(0, 0, rotationZ); // Set bow rotation

        // Update the slider position to follow the player
        if (chargeSlider != null)
        {
            chargeSlider.transform.position = Camera.main.WorldToScreenPoint(playerTransform.position + sliderOffset);
        }

        // Handle charging
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            isCharging = true;
            currentCharge = 0f;

            // Show slider
            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(true);
            }

            // Start charging animation
            bowAnimator.SetBool("isCharging", true);

            // Play bow charging sound
            PlayBowChargeSound();  // Play the charge sound
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            // Increase charge based on time
            currentCharge += (chargeSpeed / maxChargeTime) * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, 1f); // Keep charge value between 0 and 1

            // Update slider UI
            if (chargeSlider != null)
            {
                chargeSlider.value = currentCharge;
                UpdateSliderColor(currentCharge); // Update the color of the slider
            }
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            FireArrow(currentCharge); // Shoot arrow with accumulated charge
            canFire = false; // Set cooldown
            currentCharge = 0f;

            // Reset bow animation
            bowAnimator.SetBool("isCharging", false);

            // Hide slider after shooting
            if (chargeSlider != null)
            {
                chargeSlider.value = 0f; // Reset slider
                chargeSlider.gameObject.SetActive(false); // Hide slider
            }

            // Play bow release sound immediately after shooting
            PlayBowReleaseSound();  // Play the release sound immediately
        }

        // Cooldown between firing
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        // Hide the bow when idle and not charging
        if (!isCharging && playerController.IsIdle())
        {

        }
        
       if (isSpecialArrowOnCooldown)
    {
        specialArrowCooldownTimer += Time.deltaTime;
        if (cooldownSlider != null)
        {
            cooldownSlider.value = specialArrowCooldownTimer / specialArrowCooldown; // Update slider berdasarkan waktu cooldown
        }
        if (specialArrowCooldownTimer >= specialArrowCooldown)
        {
            isSpecialArrowOnCooldown = false;
            specialArrowCooldownTimer = 0f;
            if (cooldownSlider != null)
            {
                cooldownSlider.gameObject.SetActive(false); // Sembunyikan slider setelah cooldown selesai
            }

            // Hanya ubah jika saat ini menggunakan panah spesial
            if (usingSpecialArrow)
            {
                usingSpecialArrow = false; // Kembali ke panah normal
                UpdateArrowTypeUI(); // Perbarui UI untuk menunjukkan panah normal
            }
        }
    }

    // Jika cooldown aktif, pastikan menggunakan panah biasa
    if (isSpecialArrowOnCooldown && usingSpecialArrow)
    {
        usingSpecialArrow = false; // Ganti ke panah biasa
        UpdateArrowTypeUI(); // Perbarui UI
    }

    // Switch antara panah normal dan spesial dengan tombol X
    if (Input.GetKeyDown(KeyCode.X))
    {
        if (!isSpecialArrowOnCooldown) // Hanya ubah jika tidak dalam cooldown
        {
            usingSpecialArrow = !usingSpecialArrow; // Toggle antara panah normal dan spesial
            UpdateArrowTypeUI(); // Perbarui UI
        }
        else
        {
            Debug.Log("Special arrow is on cooldown!");
        }
    }
}

    // Function to update the slider color based on charge
    private void UpdateSliderColor(float charge)
    {
        if (charge < 0.5f) // 1%-49%
        {
            chargeSlider.fillRect.GetComponent<Image>().color = Color.green; // Set to green
        }
        else if (charge < 1f) // 50%-99%
        {
            chargeSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else // 100%
        {
            chargeSlider.fillRect.GetComponent<Image>().color = Color.red; // Set to red
        }
    }

    // Function to fire the arrow
private void FireArrow(float charge)
{
    if (usingSpecialArrow && isSpecialArrowOnCooldown)
    {
        Debug.Log("Cannot fire special arrow: still on cooldown.");
        return; // Jangan lanjutkan jika special arrow masih cooldown
    }

    // Aktifkan cooldown jika menggunakan panah spesial
    if (usingSpecialArrow)
    {
        isSpecialArrowOnCooldown = true;
        if (cooldownSlider != null)
        {
            cooldownSlider.gameObject.SetActive(true); // Tampilkan slider cooldown
        }
        if (arrowBackground != null)
        {
            arrowBackground.color = Color.gray; // Indikasikan panah spesial sedang cooldown
        }
    }
        // Instantiate the appropriate arrow prefab
        GameObject arrow = Instantiate(usingSpecialArrow ? specialArrowPrefab : arrowPrefab, bowTransform.position, bowTransform.rotation);

        // Get Rigidbody2D component to apply velocity
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // Calculate arrow direction based on mouse position
        Vector2 shootDirection = (mousePos - bowTransform.position).normalized;

        // Set arrow velocity (constant speed)
        rb.velocity = shootDirection * maxArrowSpeed;

        // Calculate arrow damage based on charge level
        float arrowDamage = baseDamage; // Default damage

        // Update damage based on charge percentage
        if (charge < 0.5f) // 1%-49%
        {
            arrowDamage = baseDamage; // 25
        }
        else if (charge < 1f) // 50%-99%
        {
            arrowDamage = midDamage; // 50
        }
        else // 100%
        {
            arrowDamage = maxDamage; // 100
        }

        // Assign damage to the arrow
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDamage(Mathf.RoundToInt(arrowDamage)); // Set damage
            arrowScript.SetChargeLevel(charge); // Set charge level for the arrow
        }

        // Flip the arrow sprite if player is facing left
        if (playerTransform.localScale.x < 0)
        {
            Vector3 arrowScale = arrow.transform.localScale;
            arrowScale.x *= -1; // Flip on X axis for sprite
            arrow.transform.localScale = arrowScale;
        }
    }

    // Function to play the charging sound
    private void PlayBowChargeSound()
    {
        if (audioSource != null && bowChargeSound != null)
        {
            audioSource.clip = bowChargeSound;
            audioSource.loop = false;  // Jangan loop
            audioSource.Play();
        }
    }

    // Function to play the release sound immediately after firing the arrow
    private void PlayBowReleaseSound()
    {
        if (audioSource != null && bowReleaseSound != null)
        {
            audioSource.PlayOneShot(bowReleaseSound);  // Play release sound once, immediately
        }
    }
    public Image arrowBackground; // Kotak latar belakang

private void UpdateArrowTypeUI()
{
    if (arrowTypeUI != null)
    {
        // Perbarui ikon panah
        arrowTypeUI.sprite = usingSpecialArrow ? specialArrowSprite : normalArrowSprite;

        // Jangan gunakan SetNativeSize(); biarkan ukuran RectTransform tetap
        // arrowTypeUI.SetNativeSize(); (hapus baris ini)

        // Perbarui warna latar belakang
        if (arrowBackground != null)
        {
            arrowBackground.color = usingSpecialArrow ? Color.yellow : Color.white; // Kuning untuk special arrow
        }
    }
}



    
}
