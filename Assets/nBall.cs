using UnityEngine;

public class nBall : MonoBehaviour
{
    [Header("Realistic Grass Physics")]
    public float grassFriction = 0.6f;

    [Header("Boundary Reset Settings")]
    public float resetHeightLimit = -5f;

    private Rigidbody rb;
    private nGameManager gameManager;

    public bool HasRegisteredShot { get; set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gameManager = Object.FindFirstObjectByType<nGameManager>();

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

    void ResetBall()
    {
        if (gameManager != null)
        {
            gameManager.RespawnNewBall(gameObject);
        }
    }
}