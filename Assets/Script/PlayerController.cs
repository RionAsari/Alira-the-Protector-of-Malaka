using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool facingRight = true; // To track which direction player is facing
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Move();
        HandleJump();
        AimAtMouse(); // Still keep the function to face the cursor
    }

    // Move using A and D keys
    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // A = -1, D = 1

        Vector3 moveVelocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, 0);
        rb.velocity = moveVelocity;

        // Flip character if moving in the opposite direction
        if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight))
        {
            Flip();
        }
    }

    // Aim at the mouse position (only for character rotation)
    private void AimAtMouse()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        // Flip character sprite to face the cursor
        if (mousePosition.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    // Get the mouse position in the world
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0; // Zero out Z-axis for 2D game
        return worldPosition;
    }

    // Flip the character direction
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the character
        transform.localScale = scale;
    }

    // Handle jump input with space
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false; // Jump once when space is pressed
        }
    }

    // Detect if the player is grounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
