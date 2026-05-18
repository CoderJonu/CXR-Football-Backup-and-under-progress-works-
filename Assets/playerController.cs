using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables
    public float speed = 5.0f;
    public float gravity = -9.81f;
    public float kickPower = 10.0f;

    // Components
    Animator animator;
    CharacterController controller;
    Vector3 velocity;

    // Animation Hashes
    int hashRunning;
    int hashKicking;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // Set up the animation IDs
        hashRunning = Animator.StringToHash("isRunning");
        hashKicking = Animator.StringToHash("isKicking");
    }

    void Update()
    {
        // 1. Get Inputs
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool spacePressed = Input.GetKeyDown(KeyCode.Space); // GetKeyDown is better for a single kick

        // 2. Handle Animations
        animator.SetBool(hashRunning, forwardPressed);

        if (spacePressed)
        {
            animator.SetTrigger("isKicking"); // Use a Trigger if your animation is a single swing
        }

        // 3. Handle Movement
        if (forwardPressed)
        {
            Vector3 move = transform.forward * speed * Time.deltaTime;
            controller.Move(move);
        }

        // 4. Handle Gravity (so he stays on the floor)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // 5. The Kick Logic: This fires when you bump into the ball
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // If the object we hit has a Rigidbody (the ball!)
        if (body != null && !body.isKinematic)
        {
            // Only kick if we are moving toward it or pressing space
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * kickPower;
        }
    }
}
