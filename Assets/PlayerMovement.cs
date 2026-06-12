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

    [Header("Input Settings")]
    public bool invertJoystickForward = false;
    public float joystickDeadzone = 0.15f;

    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;
    private Vector3 lastFootPosition;
    private float physicalFootSpeed;
    private GameManager gameManager;

    public InputAction rawRightThumbstick;
    public InputAction rawRightTrigger;

    public bool IsPhysicallyMoving { get; private set; }
    public bool IsVrKicking { get; private set; }

    void Awake()
    {
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

    void OnEnable()
    {
        rawRightThumbstick.Enable();
        rawRightTrigger.Enable();
    }

    void OnDisable()
    {
        rawRightThumbstick.Disable();
        rawRightTrigger.Disable();
    }

    void Start()
    {
        if (controller == null || controller.gameObject != gameObject)
            controller = GetComponent<CharacterController>();

        if (anim == null)
            anim = GetComponent<Animator>();

        if (rightFootController != null)
            lastFootPosition = rightFootController.position;

        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (rightFootController != null && Time.deltaTime > 0f)
        {
            Vector3 footDisplacement = rightFootController.position - lastFootPosition;
            physicalFootSpeed = footDisplacement.magnitude / Time.deltaTime;
            lastFootPosition = rightFootController.position;
        }

        Vector2 rightJoystick = rawRightThumbstick.ReadValue<Vector2>();
        IsVrKicking = rawRightTrigger.IsPressed();

        float forwardInput = ApplyDeadzone(invertJoystickForward ? -rightJoystick.y : rightJoystick.y);
        float turnInput = ApplyDeadzone(rightJoystick.x);
        bool keyboardMoving = false;

        // Keyboard fallback
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

        IsPhysicallyMoving = Mathf.Abs(forwardInput) > 0f || Mathf.Abs(turnInput) > 0f || keyboardMoving;

        if (anim != null)
        {
            anim.SetBool("isRunning", IsPhysicallyMoving);

            if (IsVrKicking || Input.GetKeyDown(KeyCode.Space))
                anim.SetTrigger("isKicking");
        }

        transform.Rotate(0f, turnInput * rotationSpeed * Time.deltaTime, 0f);

        Vector3 targetDirection = transform.forward * forwardInput;

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

        controller.Move(
            currentVelocity * Time.deltaTime
        );

        if (controller.isGrounded &&
            verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y +=
            gravity * Time.deltaTime;

        controller.Move(
            verticalVelocity * Time.deltaTime
        );
    }

    float ApplyDeadzone(float value)
    {
        return Mathf.Abs(value) < joystickDeadzone ? 0f : value;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.name.ToLower().Contains("ball") && hit.collider.attachedRigidbody == null)
            return;

        Rigidbody ballBody = hit.collider.attachedRigidbody;

        if (ballBody == null || ballBody.isKinematic)
            return;

        ballBody.velocity = Vector3.zero;
        ballBody.angularVelocity = Vector3.zero;

        Vector3 pushDir = transform.forward;
        bool isIntentionalKick = IsVrKicking
            || Input.GetKey(KeyCode.Space)
            || physicalFootSpeed > kickVelocityThreshold;

        if (isIntentionalKick)
        {
            if (gameManager != null)
                gameManager.RegisterShot(ballBody.gameObject);

            pushDir.y = kickUpwardForce / maxKickPower;

            float calculatedPower = Mathf.Clamp(
                physicalFootSpeed * 2.5f,
                softDribblePower,
                maxKickPower
            );

            if (IsVrKicking || Input.GetKey(KeyCode.Space))
                calculatedPower = maxKickPower;

            ballBody.AddForce(pushDir.normalized * calculatedPower, ForceMode.Impulse);
            Debug.Log("VR Kick Executed!");
        }
        else
        {
            pushDir.y = 0f;
            ballBody.AddForce(pushDir.normalized * softDribblePower, ForceMode.Impulse);
        }
    }
}
