using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("Movement Settings")]
    public float currentMaxSpeed = 2f; // Changed to dynamic variable
    public float walkSpeed = 2f;       // Base walking speed
    public float sprintSpeed = 5f;     // Increased speed to 5
    public float rotationSpeed = 150f;
    public float gravity = -9.81f;

    [Header("Bot Breaking Friction")]
    public float brakingFriction = 15f;
    public float accelerationRate = 2f;

    [Header("Football Striking Settings")]
    public float maxKickPower = 12.0f;
    public float kickUpwardForce = 3.5f;
    public float softDribblePower = 3.0f;

    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;

    private int hashRunning;
    private int hashKicking;

    // A public property so your Animation script can check if the player is sprinting
    public bool IsSprinting { get; private set; }

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (anim == null) anim = GetComponent<Animator>();

        hashRunning = Animator.StringToHash("isRunning");
        hashKicking = Animator.StringToHash("isKicking");
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Vertical");
        float turnInput = Input.GetAxisRaw("Horizontal");
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Handle dynamic speed alteration (2f to 5f)
        if (moveInput > 0.1f && shiftPressed)
        {
            currentMaxSpeed = sprintSpeed;
            IsSprinting = true;
        }
        else
        {
            currentMaxSpeed = walkSpeed;
            IsSprinting = false;
        }

        transform.Rotate(0, turnInput * rotationSpeed * Time.deltaTime, 0);

        Vector3 targetDirection = transform.forward * moveInput;

        if (targetDirection.magnitude > 0.1f)
        {
            // Uses currentMaxSpeed (which dynamically shifts between 2 and 5)
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection * currentMaxSpeed, accelerationRate * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, brakingFriction * Time.deltaTime);
        }

        controller.Move(currentVelocity * Time.deltaTime);

        if (spacePressed)
        {
            anim.SetTrigger(hashKicking);
        }

        bool isMoving = currentVelocity.magnitude > 0.1f || Mathf.Abs(turnInput) > 0.1f;
        anim.SetBool(hashRunning, isMoving);

        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody ballBody = hit.collider.attachedRigidbody;

        if (ballBody != null && !ballBody.isKinematic)
        {
            ballBody.velocity = Vector3.zero;
            ballBody.angularVelocity = Vector3.zero;

            Vector3 pushDir = transform.forward;
            bool isIntentionalKick = Input.GetKey(KeyCode.Space);

            if (isIntentionalKick)
            {
                pushDir.y = kickUpwardForce / maxKickPower;
                ballBody.AddForce(pushDir.normalized * maxKickPower, ForceMode.Impulse);
                Debug.Log("Intentional Strike Executed!");
            }
            else
            {
                pushDir.y = 0f;
                ballBody.AddForce(pushDir.normalized * softDribblePower, ForceMode.Impulse);
                Debug.Log("Gently Dribbling Ball.");
            }
        }
    }
}
