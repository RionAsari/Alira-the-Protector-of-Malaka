using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    private Animator bowAnimator; // Reference to the bow's Animator
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform playerTransform;

    public GameObject arrowPrefab; // Prefab for the normal arrow
    public GameObject specialArrowPrefab; // Prefab for the special arrow
    public Transform bowTransform; // Position where arrows are spawned
    public float maxArrowSpeed = 15f; // Maximum arrow speed (constant)
    public float chargeSpeed = 5f; // Speed of charging (how fast charge builds)
    
    private float currentCharge = 0f; // Amount of charge
    private bool isCharging = false; // Is the player currently charging?
    private bool canFire = true;
    private float timer;
    public float timeBetweenFiring = 0.5f; // Cooldown time between firing
    private PlayerController playerController;

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

    private void Start()
    {
        mainCam = Camera.main; // Get the main camera reference
        playerTransform = transform.parent; // Get the reference of the player (parent)
        playerController = playerTransform.GetComponent<PlayerController>(); // Get PlayerController

        // Reset slider at the start
        if (chargeSlider != null)
        {
            chargeSlider.value = 0f; // Set slider to 0 initially
            chargeSlider.maxValue = 1f; // Slider range between 0 and 1
            chargeSlider.gameObject.SetActive(false); // Hide slider at the start
        }

        // Get reference to the bow's Animator
        bowAnimator = bowTransform.GetComponent<Animator>();
    }

    private void Update()
    {
        // Get mouse position in world coordinates
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Set z to 0 since we're in 2D

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Flip rotation if player is facing left
        if (playerTransform.localScale.x < 0)
        {
            rotZ += 180;
        }

        // Rotate the bow
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

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
            playerController.ShowBow(true); // Show bow on mouse click

            // Show slider
            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(true);
            }

            // Start charging animation
            bowAnimator.SetBool("isCharging", true);
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
            playerController.ShowBow(false); // Hide bow when idle
        }

        // Switch between normal and special arrow on 'X' key press
        if (Input.GetKeyDown(KeyCode.X))
        {
            usingSpecialArrow = !usingSpecialArrow; // Toggle between normal and special arrow
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

    public bool IsCharging()
    {
        return isCharging; // Return charging state
    }
}
