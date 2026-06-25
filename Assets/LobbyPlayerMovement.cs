using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class LobbyPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Animator anim;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float rotationSpeed = 120f;
    public float gravity = -9.81f;

    [Header("Input")]
    public bool invertJoystickForward = false;
    public float joystickDeadzone = 0.15f;

    private Vector3 verticalVelocity;

    private InputAction rightThumbstick;

    void Awake()
    {
        rightThumbstick = new InputAction(
            "RightStick",
            InputActionType.Value,
            "<XRController>{RightHand}/thumbstick"
        );
    }

    void OnEnable()
    {
        rightThumbstick.Enable();
    }

    void OnDisable()
    {
        rightThumbstick.Disable();
    }

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 stick = rightThumbstick.ReadValue<Vector2>();

        float forwardInput =
            ApplyDeadzone(
                invertJoystickForward ? -stick.y : stick.y
            );

        float turnInput =
            ApplyDeadzone(stick.x);

        // WASD fallback
        if (Input.GetKey(KeyCode.W))
            forwardInput = 1f;

        if (Input.GetKey(KeyCode.S))
            forwardInput = -1f;

        if (Input.GetKey(KeyCode.A))
            turnInput = -1f;

        if (Input.GetKey(KeyCode.D))
            turnInput = 1f;

        // Animator
        if (anim != null)
        {
            anim.SetBool(
                "isRunning",
                Mathf.Abs(forwardInput) > 0.05f
            );
        }

        // Rotation
        transform.Rotate(
            0f,
            turnInput * rotationSpeed * Time.deltaTime,
            0f
        );

        // Movement
        Vector3 move =
            transform.forward *
            forwardInput *
            walkSpeed;

        controller.Move(
            move * Time.deltaTime
        );

        // Gravity
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
        return Mathf.Abs(value) < joystickDeadzone
            ? 0f
            : value;
    }
}