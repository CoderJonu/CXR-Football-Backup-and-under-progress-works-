using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("VR Input Mappings")]
    public InputActionProperty moveAction;   // Left Stick (Move/Strafe)
    public InputActionProperty turnAction;   // Right Stick (Disabled for turning)
    public InputActionProperty sprintAction; // Left Stick Click / Grip (Sprint)
    public InputActionProperty kickAction;   // Right Hand Trigger

    [Header("Foot Hardware Tracking")]
    public Transform rightFootController;
    public float kickVelocityThreshold = 1.8f;

    [Header("Movement Settings")]
    public float currentMaxSpeed = 2f;
    public float walkSpeed = 5f; // Bumped up slightly so it's easier to see movement
    public float sprintSpeed = 8f;
    public float rotationSpeed = 150f;
    public float gravity = -9.81f;

    [Header("Bot Breaking Friction")]
    public float brakingFriction = 15f;
    public float accelerationRate = 10f;

    [Header("Football Striking Settings")]
    public float maxKickPower = 12.0f;
    public float kickUpwardForce = 3.5f;
    public float softDribblePower = 3.0f;

    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;
    private Vector3 lastFootPosition;
    private float physicalFootSpeed;

    private int hashRunning;
    private int hashKicking;

    public bool IsSprinting { get; private set; }

    private void OnEnable()
    {
        if (moveAction.action != null) moveAction.action.Enable();
        if (turnAction.action != null) turnAction.action.Enable();
        if (sprintAction.action != null) sprintAction.action.Enable();
        if (kickAction.action != null) kickAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction.action != null) moveAction.action.Disable();
        if (turnAction.action != null) turnAction.action.Disable();
        if (sprintAction.action != null) sprintAction.action.Disable();
        if (kickAction.action != null) kickAction.action.Disable();
    }

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (anim == null) anim = GetComponent<Animator>();

        hashRunning = Animator.StringToHash("isRunning");
        hashKicking = Animator.StringToHash("isKicking");

        if (rightFootController != null)
        {
            lastFootPosition = rightFootController.position;
        }
    }

    void Update()
    {
        // --- 1. Track Physical Foot Velocity ---
        if (rightFootController != null && Time.deltaTime > 0)
        {
            Vector3 footDisplacement = rightFootController.position - lastFootPosition;
            physicalFootSpeed = footDisplacement.magnitude / Time.deltaTime;
            lastFootPosition = rightFootController.position;
        }

        // --- 2. Read VR Inputs ---
        Vector2 moveJoystick = Vector2.zero;
        bool vrSprintPressed = false;
        bool vrKickPressed = false;

        if (moveAction.action != null) moveJoystick = moveAction.action.ReadValue<Vector2>();
        if (sprintAction.action != null) vrSprintPressed = sprintAction.action.IsPressed();
        if (kickAction.action != null) vrKickPressed = kickAction.action.WasPressedThisFrame();

        // --- 3. Process Input & Setup Keyboard Fallbacks ---
        float forwardInput = moveJoystick.y;
        float strafeInput = moveJoystick.x;
        float turnInput = 0f;
        bool isSprintingPressed = vrSprintPressed;
        bool isKickTriggered = vrKickPressed;

        if (Input.GetKey(KeyCode.W)) forwardInput = 1f;
        if (Input.GetKey(KeyCode.S)) forwardInput = -1f;
        if (Input.GetKey(KeyCode.A)) turnInput = -1f;
        if (Input.GetKey(KeyCode.D)) turnInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) isSprintingPressed = true;
        if (Input.GetKeyDown(KeyCode.Space)) isKickTriggered = true;

        // --- 4. Speed & Sprint Control ---
        if (Mathf.Abs(forwardInput) > 0.1f && isSprintingPressed)
        {
            currentMaxSpeed = sprintSpeed;
            IsSprinting = true;
        }
        else
        {
            currentMaxSpeed = walkSpeed;
            IsSprinting = false;
        }

        // --- 5. Rotation & Translation Physics ---
        transform.Rotate(0, turnInput * rotationSpeed * Time.deltaTime, 0);

        Vector3 targetDirection = (transform.forward * forwardInput) + (transform.right * strafeInput);

        if (targetDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection.normalized * currentMaxSpeed, accelerationRate * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, brakingFriction * Time.deltaTime);
        }

        controller.Move(currentVelocity * Time.deltaTime);

        // --- 6. Animations ---
        if (isKickTriggered || physicalFootSpeed > kickVelocityThreshold)
        {
            anim.SetTrigger(hashKicking);
        }

        bool isMoving = currentVelocity.magnitude > 0.1f || Mathf.Abs(turnInput) > 0.1f;
        anim.SetBool(hashRunning, isMoving);

        // --- 7. Gravity ---
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.name.ToLower().Contains("ball") && hit.collider.attachedRigidbody == null) return;

        Rigidbody ballBody = hit.collider.attachedRigidbody;

        if (ballBody != null && !ballBody.isKinematic)
        {
            ballBody.velocity = Vector3.zero;
            ballBody.angularVelocity = Vector3.zero;

            Vector3 pushDir = transform.forward;
            bool vrTriggerPressed = (kickAction.action != null && kickAction.action.IsPressed());
            bool isIntentionalKick = vrTriggerPressed || Input.GetKey(KeyCode.Space) || (physicalFootSpeed > kickVelocityThreshold);

            if (isIntentionalKick)
            {
                pushDir.y = kickUpwardForce / maxKickPower;
                float calculatedPower = Mathf.Clamp(physicalFootSpeed * 2.5f, softDribblePower, maxKickPower);
                if (Input.GetKey(KeyCode.Space) || vrTriggerPressed) calculatedPower = maxKickPower;

                ballBody.AddForce(pushDir.normalized * calculatedPower, ForceMode.Impulse);
                Debug.Log("Goal Striking Executed!");
            }
            else
            {
                pushDir.y = 0f;
                ballBody.AddForce(pushDir.normalized * softDribblePower, ForceMode.Impulse);
            }
        }
    }
}