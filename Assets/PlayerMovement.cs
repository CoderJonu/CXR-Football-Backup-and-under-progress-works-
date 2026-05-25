using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("Foot Hardware Tracking")]
    public Transform rightFootController;
    public float kickVelocityThreshold = 1.8f;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
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

    // VR Controller Inputs
    public InputAction rawRightThumbstick;
    public InputAction rawRightTrigger;

    // Animation States
    public bool IsPhysicallyMoving { get; private set; }
    public bool IsVrKicking { get; private set; }

    private void Awake()
    {
        // RIGHT CONTROLLER INPUTS
        rawRightThumbstick = new InputAction(
            "RightStick",
            InputActionType.Value,
            "<XRController>{RightHand}/thumbstick"
        );

        rawRightTrigger = new InputAction(
            "RightTrigger",
            InputActionType.Button,
            "<XRController>{RightHand}/triggerPressed"
        );
    }

    private void OnEnable()
    {
        rawRightThumbstick.Enable();
        rawRightTrigger.Enable();
    }

    private void OnDisable()
    {
        rawRightThumbstick.Disable();
        rawRightTrigger.Disable();
    }

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (rightFootController != null)
        {
            lastFootPosition = rightFootController.position;
        }
    }

    void Update()
    {
        // ------------------------------------------------
        // FOOT SPEED TRACKING
        // ------------------------------------------------
        if (rightFootController != null && Time.deltaTime > 0)
        {
            Vector3 footDisplacement =
                rightFootController.position - lastFootPosition;

            physicalFootSpeed =
                footDisplacement.magnitude / Time.deltaTime;

            lastFootPosition = rightFootController.position;
        }

        // ------------------------------------------------
        // READ VR INPUTS
        // ------------------------------------------------
        Vector2 rightJoystick =
            rawRightThumbstick.ReadValue<Vector2>();

        IsVrKicking = rawRightTrigger.IsPressed();

        // ------------------------------------------------
        // INPUT VARIABLES
        // ------------------------------------------------
        float forwardInput = rightJoystick.y;
        float turnInput = rightJoystick.x;

        bool keyboardMoving = false;

        // ------------------------------------------------
        // KEYBOARD FALLBACKS
        // ------------------------------------------------
        if (Input.GetKey(KeyCode.W))
        {
            forwardInput = 1f;
            keyboardMoving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            forwardInput = -1f;
            keyboardMoving = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnInput = -1f;
            keyboardMoving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            turnInput = 1f;
            keyboardMoving = true;
        }

        // ------------------------------------------------
        // MOVEMENT DETECTION
        // ------------------------------------------------
        IsPhysicallyMoving =
            (rightJoystick.magnitude > 0.15f) || keyboardMoving;

        // ------------------------------------------------
        // ANIMATION CONTROL
        // ------------------------------------------------
        anim.SetBool("isRunning", IsPhysicallyMoving);

        if (IsVrKicking || Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("isKicking");
        }

        // ------------------------------------------------
        // PLAYER ROTATION
        // ------------------------------------------------
        transform.Rotate(
            0,
            turnInput * rotationSpeed * Time.deltaTime,
            0
        );

        // ------------------------------------------------
        // PLAYER MOVEMENT
        // ------------------------------------------------
        Vector3 targetDirection =
            transform.forward * forwardInput;

        if (targetDirection.magnitude > 0.05f)
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetDirection.normalized * walkSpeed,
                accelerationRate * Time.deltaTime
            );
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                brakingFriction * Time.deltaTime
            );
        }

        controller.Move(currentVelocity * Time.deltaTime);

        // ------------------------------------------------
        // GRAVITY
        // ------------------------------------------------
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (
            !hit.gameObject.name.ToLower().Contains("ball")
            && hit.collider.attachedRigidbody == null
        )
            return;

        Rigidbody ballBody = hit.collider.attachedRigidbody;

        if (ballBody != null && !ballBody.isKinematic)
        {
            // RESET BALL VELOCITY
            ballBody.velocity = Vector3.zero;
            ballBody.angularVelocity = Vector3.zero;

            Vector3 pushDir = transform.forward;

            bool isIntentionalKick =
                IsVrKicking
                || Input.GetKey(KeyCode.Space)
                || (physicalFootSpeed > kickVelocityThreshold);

            // ------------------------------------------------
            // POWER KICK
            // ------------------------------------------------
            if (isIntentionalKick)
            {
                pushDir.y = kickUpwardForce / maxKickPower;

                float calculatedPower = Mathf.Clamp(
                    physicalFootSpeed * 2.5f,
                    softDribblePower,
                    maxKickPower
                );

                if (IsVrKicking || Input.GetKey(KeyCode.Space))
                {
                    calculatedPower = maxKickPower;
                }

                ballBody.AddForce(
                    pushDir.normalized * calculatedPower,
                    ForceMode.Impulse
                );

                Debug.Log("VR Kick Executed!");
            }

            // ------------------------------------------------
            // SOFT DRIBBLE
            // ------------------------------------------------
            else
            {
                pushDir.y = 0f;

                ballBody.AddForce(
                    pushDir.normalized * softDribblePower,
                    ForceMode.Impulse
                );
            }
        }
    }
}