using UnityEngine;

public class GoalieAI : MonoBehaviour
{
    [Header("Target Tracking Configuration")]
    [Tooltip("Type the exact name/tag of the ball used in this room (e.g., 'nBall' or 'Ball')")]
    public string ballNameAndTag = "nBall";

    public Transform ballTransform;
    public Transform goalCenterMarker;

    [Header("Movement Properties")]
    public float lateralSpeed = 6f;
    public float maxSlideDistance = 3f;
    public float saveRadius = 1.5f;

    private Vector3 startingPosition;
    private Rigidbody ballRb;
    private float reFindTimer = 0f;

    void Start()
    {
        startingPosition = transform.position;
        LocateBallReference();
    }

    void Update()
    {
        // BUG FIX: Uses the custom inspector string to auto-find the correct ball when it resets
        if (ballTransform == null)
        {
            reFindTimer += Time.deltaTime;
            if (reFindTimer >= 0.1f)
            {
                LocateBallReference();
                reFindTimer = 0f;
            }
            return;
        }

        if (goalCenterMarker == null) return;

        float targetX = ballTransform.position.x;
        float clampedX = Mathf.Clamp(targetX, goalCenterMarker.position.x - maxSlideDistance, goalCenterMarker.position.x + maxSlideDistance);

        Vector3 targetPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, lateralSpeed * Time.deltaTime);

        float distanceToBall = Vector3.Distance(transform.position, ballTransform.position);
        if (distanceToBall <= saveRadius && ballRb != null)
        {
            PerformSave();
        }
    }

    void LocateBallReference()
    {
        // Uses the customizable string field to dynamically find the correct ball object
        GameObject foundBall = GameObject.Find(ballNameAndTag);
        if (foundBall == null) foundBall = GameObject.FindWithTag(ballNameAndTag);

        if (foundBall != null)
        {
            ballTransform = foundBall.transform;
            ballRb = foundBall.GetComponent<Rigidbody>();
            Debug.Log(gameObject.name + " locked onto the active ball tracking tag: " + ballNameAndTag);
        }
    }

    void PerformSave()
    {
        if (ballRb == null) return;

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 clearDirection = (ballTransform.position - transform.position).normalized;
        clearDirection.y = 0.2f;

        ballRb.AddForce(clearDirection * 8f, ForceMode.Impulse);
        Debug.Log(gameObject.name + " made a save in the room!");
    }
}
