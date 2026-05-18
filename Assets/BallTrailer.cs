using UnityEngine;

public class BallTrailController : MonoBehaviour
{
    private Rigidbody rb;
    private TrailRenderer trail;

    // The minimum speed the ball must travel to turn on the trail
    public float movementThreshold = 0.5f;

    void Start()
    {
        // Get references to components on the ball
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        // Start with the trail turned off
        trail.emitting = false;
    }

    void Update()
    {
        // Check if the ball's current speed is greater than our threshold
        if (rb.velocity.magnitude > movementThreshold)
        {
            trail.emitting = true; // Turn trail on
        }
        else
        {
            trail.emitting = false; // Turn trail off
        }
    }
}
