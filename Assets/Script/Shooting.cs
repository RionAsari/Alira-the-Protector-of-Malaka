using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform playerTransform;
    public GameObject arrowPrefab; // Prefab for the arrow
    public Transform bowTransform; // Position where arrows are spawned
    public float maxArrowSpeed = 15f; // Maximum arrow speed
    public float chargeSpeed = 5f; // Speed of charging
    private float currentCharge = 0f; // Amount of charge
    private bool isCharging = false; // Is the player currently charging?
    private bool canFire = true;
    private float timer;
    public float timeBetweenFiring = 0.5f; // Cooldown time between firing
    private PlayerController playerController;

    private void Start()
    {
        mainCam = Camera.main; // Get the main camera reference
        playerTransform = transform.parent; // Get the reference of the player (parent)
        playerController = playerTransform.GetComponent<PlayerController>(); // Get PlayerController
    }

    private void Update()
    {
        // Get mouse position in world coordinates
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Flip rotation if player is facing left
        if (playerTransform.localScale.x < 0)
        {
            rotZ += 180;
        }

        // Rotate the bow
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Start charging when left mouse button is pressed
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            isCharging = true;
            currentCharge = 0f;
            playerController.ShowBow(true); // Show bow on mouse click
        }

        // Charging while holding left mouse button
        if (Input.GetMouseButton(0) && isCharging)
        {
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxArrowSpeed);
        }

        // Release arrow when mouse button is released
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            FireArrow(currentCharge); // Shoot arrow with accumulated charge
            canFire = false;
            currentCharge = 0f; // Reset charge
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
    }

    // Function to fire the arrow
    private void FireArrow(float charge)
    {
        // Instantiate the arrow
        GameObject arrow = Instantiate(arrowPrefab, bowTransform.position, bowTransform.rotation);

        // Get Rigidbody2D component to apply velocity
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // Calculate arrow direction based on bow direction
        Vector2 shootDirection = (mousePos - bowTransform.position).normalized;

        // Set arrow velocity based on direction and charge
        rb.velocity = shootDirection * charge;

        // Flip the arrow sprite if player is facing left
        if (playerTransform.localScale.x < 0)
        {
            Vector3 arrowScale = arrow.transform.localScale;
            arrowScale.x *= -1; // Flip on X axis for sprite
            arrow.transform.localScale = arrowScale;
        }

        // Reset charge after firing
        currentCharge = 0;
    }
}
