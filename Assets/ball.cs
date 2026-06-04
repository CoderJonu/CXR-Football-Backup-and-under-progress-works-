/*using UnityEngine;

public class ball : MonoBehaviour
{
    [Header("Realistic Grass Physics")]
    [Tooltip("Slows down rolling so it acts like real grass. Higher = slower pitch.")]
    public float grassFriction = 0.6f;

    [Header("Boundary Reset Settings")]
    [Tooltip("If the ball falls beneath this height, it respawns at the center spot.")]
    public float resetHeightLimit = -5f;

    private Vector3 spawnPosition;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Save the exact center position where the match started
        spawnPosition = transform.position;

        // Ensure physics engine configuration is perfectly set
        if (rb != null)
        {
            rb.mass = 1.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void FixedUpdate()
    {
        // 1. Natural Grass Drag Friction
        // Uses standard velocity compatible with your Unity version
        if (rb != null && rb.velocity.magnitude > 0.05f)
        {
            // Apply a continuous counter-force pushing back against the roll direction
            Vector3 frictionForce = -rb.velocity * grassFriction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        // 2. Safety Void Trigger
        if (transform.position.y < resetHeightLimit)
        {
            ResetBallToCenter();
        }
    }

    public void ResetBallToCenter()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = spawnPosition;
        Debug.Log("Ball went out of bounds! Resetting to kick-off center.");
    }
}
*/
using UnityEngine;

public class ball : MonoBehaviour
{
    [Header("Realistic Grass Physics")]
    public float grassFriction = 0.6f;

    [Header("Boundary Reset Settings")]
    public float resetHeightLimit = -5f;

    private Rigidbody rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Automatically find the game manager in the scene
        gameManager = Object.FindFirstObjectByType<GameManager>();

        if (rb != null)
        {
            rb.mass = 1.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void FixedUpdate()
    {
        if (rb != null && rb.velocity.magnitude > 0.05f)
        {
            Vector3 frictionForce = -rb.velocity * grassFriction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);
        }

        if (transform.position.y < resetHeightLimit)
        {
            ResetBall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "goal")
        {
            if (gameManager != null)
            {
                gameManager.GoalScored(gameObject); // Alert manager to change scores
            }
        }
    }

    void ResetBall()
    {
        if (gameManager != null)
        {
            gameManager.RespawnNewBall(gameObject);
        }
    }
}
