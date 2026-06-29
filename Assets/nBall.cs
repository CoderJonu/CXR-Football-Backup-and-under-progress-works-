using UnityEngine;

public class nBall : MonoBehaviour
{
    [Header("Realistic Grass Physics")]
    public float grassFriction = 0.6f;

    [Header("Boundary Reset Settings")]
    public float resetHeightLimit = -5f;

    [Header("Defender Clearance Settings")]
    public Transform goalCenter;
    public float defenderClearPower = 12f;
    public float defenderClearUpwardForce = 1.2f;
    public float defenderClearSidePower = 3f;
    public float defenderClearCooldown = 0.25f;

    private Rigidbody rb;
    private nGameManager gameManager;
    private float lastDefenderClearTime = -999f;
    private bool isScoring = false;

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

    void OnCollisionEnter(Collision collision)
    {
        if (IsGoalCollision(collision.gameObject))
        {
            ScoreGoal();
            return;
        }

        if (!IsDefenderCollision(collision.gameObject))
            return;

        ClearBallAwayFromGoal();
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsGoalCollision(other.gameObject))
            ScoreGoal();
    }

    bool IsGoalCollision(GameObject other)
    {
        if (other == null)
            return false;

        if (HasTag(other, "Goal") || other.GetComponentInParent<nGoalDetector>() != null)
            return true;

        Transform current = other.transform;
        while (current != null)
        {
            string objectName = current.name.ToLower();
            if (objectName.Contains("goal"))
                return true;

            current = current.parent;
        }

        return false;
    }

    bool HasTag(GameObject other, string tagName)
    {
        try
        {
            return other.CompareTag(tagName);
        }
        catch (UnityException)
        {
            return false;
        }
    }

    void ScoreGoal()
    {
        if (isScoring)
            return;

        isScoring = true;

        if (gameManager == null)
            gameManager = Object.FindFirstObjectByType<nGameManager>();

        if (gameManager != null)
        {
            gameManager.GoalScored(gameObject);
        }

        Invoke(nameof(ResetScoringLock), 0.5f);
    }

    void ResetScoringLock()
    {
        isScoring = false;
    }

    bool IsDefenderCollision(GameObject other)
    {
        if (other.GetComponentInParent<DefenderBoardAI>() != null)
            return true;

        string objectName = other.name.ToLower();
        return objectName.Contains("defender") || objectName.Contains("board");
    }

    void ClearBallAwayFromGoal()
    {
        if (rb == null || Time.time - lastDefenderClearTime < defenderClearCooldown)
            return;

        lastDefenderClearTime = Time.time;
        HasRegisteredShot = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 clearDirection = GetClearDirection();
        Vector3 sideDirection = Vector3.Cross(Vector3.up, clearDirection).normalized;

        if (Random.value < 0.5f)
            sideDirection = -sideDirection;

        Vector3 clearance = clearDirection * defenderClearPower
            + sideDirection * defenderClearSidePower
            + Vector3.up * defenderClearUpwardForce;

        rb.AddForce(clearance, ForceMode.Impulse);
        Debug.Log("Defender cleared the ball away from goal.");
    }

    Vector3 GetClearDirection()
    {
        if (goalCenter == null)
        {
            GameObject foundGoal = GameObject.Find("goal");
            if (foundGoal != null)
                goalCenter = foundGoal.transform;
        }

        Vector3 direction;

        if (goalCenter != null)
        {
            direction = transform.position - goalCenter.position;
        }
        else if (gameManager != null && gameManager.spawnPoint != null)
        {
            direction = gameManager.spawnPoint.position - transform.position;
        }
        else
        {
            direction = -transform.forward;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            direction = -transform.forward;

        return direction.normalized;
    }

    void ResetBall()
    {
        if (gameManager != null)
        {
            gameManager.RespawnNewBall(gameObject);
        }
    }
}
