using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    PlayerMovement playerMovement; // Reference connection to your movement script

    int hashRunning;
    int hashKicking;
    int hashAccelerating;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Automatically find the player movement component on this object
        playerMovement = GetComponent<PlayerMovement>();

        Debug.Log(animator);
        hashRunning = Animator.StringToHash("isRunning");
        hashKicking = Animator.StringToHash("isKicking");
        hashAccelerating = Animator.StringToHash("isAccelerating");
    }

    void Update()
    {
        bool isKicking = animator.GetBool(hashKicking);
        bool isRunning = animator.GetBool(hashRunning);

        bool forwardpressed = Input.GetKey(KeyCode.W);
        bool spacepressed = Input.GetKey(KeyCode.Space);

        // Read the sprinting state straight from the movement script 
        if (playerMovement != null)
        {
            animator.SetBool(hashAccelerating, playerMovement.IsSprinting);
        }

        if (spacepressed)
        {
            animator.SetBool(hashKicking, true);
        }
        if (!spacepressed)
        {
            animator.SetBool(hashKicking, false);
        }
        if (!isRunning && forwardpressed)
        {
            animator.SetBool(hashRunning, true);
        }
        if (isRunning && !forwardpressed)
        {
            animator.SetBool(hashRunning, false);
        }
        if (!isKicking && (isRunning && spacepressed))
        {
            animator.SetBool(hashKicking, true);
        }
        if (isRunning && !spacepressed)
        {
            animator.SetBool(hashKicking, false);
        }
    }
}
