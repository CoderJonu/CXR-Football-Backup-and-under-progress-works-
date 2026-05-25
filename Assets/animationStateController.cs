using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    CharacterController botCharacterController;

    private int[] runningHashes;
    private int[] speedHashes;

    private int hashKickingLower;
    private int hashKickingUpper;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Grab the standard CharacterController component attached to the bot
        botCharacterController = GetComponent<CharacterController>();

        // Cache animator parameters to hit whatever naming convention you used
        runningHashes = new int[] {
            Animator.StringToHash("isRunning"),
            Animator.StringToHash("IsRunning"),
            Animator.StringToHash("running"),
            Animator.StringToHash("Running")
        };

        speedHashes = new int[] {
            Animator.StringToHash("speed"),
            Animator.StringToHash("Speed")
        };

        hashKickingLower = Animator.StringToHash("isKicking");
        hashKickingUpper = Animator.StringToHash("IsKicking");
    }

    void Update()
    {
        if (animator == null) return;

        // --- 1. Detect Movement Natively by Checking the Bot's Velocity Vector ---
        // If the CharacterController is physically moving through space at all, or W is pressed, trigger running
        bool isMoving = false;

        if (botCharacterController != null)
        {
            // check horizontal velocity magnitude (ignores gravity falls)
            Vector3 horizontalVelocity = new Vector3(botCharacterController.velocity.x, 0f, botCharacterController.velocity.z);
            isMoving = horizontalVelocity.magnitude > 0.1f;
        }

        // Keep keyboard fallback active just in case
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isMoving = true;
        }

        // --- 2. Track Kicking State via Keyboard Spacebar Fallback ---
        bool isKicking = Input.GetKey(KeyCode.Space);

        // --- 3. Push Clean Updates to the Animator State Machine ---
        foreach (int hash in runningHashes)
        {
            try { animator.SetBool(hash, isMoving); } catch { }
        }

        float blendTreeSpeed = isMoving ? 1.0f : 0.0f;
        foreach (int hash in speedHashes)
        {
            try { animator.SetFloat(hash, blendTreeSpeed); } catch { }
        }

        animator.SetBool(hashKickingLower, isKicking);
        animator.SetBool(hashKickingUpper, isKicking);
    }
}
