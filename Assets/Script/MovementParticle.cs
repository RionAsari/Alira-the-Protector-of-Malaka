using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] ParticleSystem movementParticle;
    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;
    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;
    [SerializeField] Rigidbody2D playerRB;

    private PlayerController playerController; // Reference to PlayerController
    private float counter;

    private void Awake()
    {
        // Get reference to the PlayerController component
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        counter += Time.deltaTime;

        if (playerController != null)
        {
            // Access the private 'facingRight' and 'isGrounded' fields using reflection
            bool facingRight = (bool)playerController.GetType().GetField("facingRight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerController);
            bool isGrounded = (bool)playerController.GetType().GetField("isGrounded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerController);

            // Flip particle system based on player's facing direction
            var main = movementParticle.main;
            main.startRotationY = facingRight ? 0f : Mathf.PI; // Flip on Y-axis

            // Only play particles if the player is moving, grounded, and exceeds the velocity threshold
            if (isGrounded && Mathf.Abs(playerRB.velocity.x) > occurAfterVelocity && playerRB.velocity.x != 0)
            {
                if (counter > dustFormationPeriod)
                {
                    movementParticle.Play();
                    counter = 0;
                }
            }
            else
            {
                // Stop the particle system if the player is not moving or not grounded
                movementParticle.Stop();
            }
        }
    }
}
