using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    private PlayerController playerController;  // Reference to the PlayerController script
    private AudioSource footstepAudio;          // AudioSource for footstep sound
    public AudioClip footstepSound;             // Footstep sound clip
    public float stepDelay = 0.5f;             // Time delay between footsteps

    private bool isWalking;                    // Check if the player is walking
    private float nextStepTime = 0f;            // Time to next step

    private void Start()
    {
        // Get reference to the PlayerController and AudioSource components
        playerController = GetComponent<PlayerController>();
        footstepAudio = GetComponent<AudioSource>();

        if (footstepSound != null)
        {
            footstepAudio.clip = footstepSound;  // Set the footstep sound clip
        }
    }

    private void Update()
    {
        // Check if the player is grounded and moving (walking)
        isWalking = playerController.GetIsGrounded() && Mathf.Abs(playerController.GetMoveInput()) > 0;

        // Play the footstep sound if walking and time is right
        if (isWalking && Time.time >= nextStepTime)
        {
            footstepAudio.Play();
            nextStepTime = Time.time + stepDelay;  // Set the next step time
        }
    }
}
