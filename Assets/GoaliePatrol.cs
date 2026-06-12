using UnityEngine;

public class GoaliePatrol : MonoBehaviour
{
    public enum PatrolAxis
    {
        GoalLocalX,
        GoalLocalZ,
        WorldX,
        WorldZ
    }

    [Header("Goal Reference")]
    public Transform goalPost;
    public PatrolAxis patrolAxis = PatrolAxis.GoalLocalX;
    public bool preferGoalInSameRoom = true;
    public float fallbackGoalWidth = 4f;
    public float postPadding = 0.35f;
    public float goalieRadius = 0.45f;

    [Header("Movement")]
    public float sideStepSpeed = 1.5f;
    public bool startMovingRight = true;

    [Header("Animation")]
    public Animator animator;
    public string sideStepBoolParameter = "isSidestepping";
    public string sideStepStateName = "";
    public float animationFadeTime = 0.15f;

    private Vector3 centerPosition;
    private Vector3 moveAxis;
    private float halfPatrolDistance;
    private int direction;
    private int sideStepBoolHash;
    private bool hasSideStepBool;
    private bool hasSideStepState;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (preferGoalInSameRoom)
            goalPost = ResolveGoalPostForThisRoom();

        centerPosition = goalPost != null ? goalPost.position : transform.position;
        centerPosition.y = transform.position.y;

        moveAxis = GetMoveAxis();
        halfPatrolDistance = CalculateHalfPatrolDistance();
        direction = startMovingRight ? 1 : -1;

        ConfigureAnimator();
    }

    Transform ResolveGoalPostForThisRoom()
    {
        Transform roomRoot = transform.root;

        if (goalPost != null && goalPost.root == roomRoot)
            return goalPost;

        foreach (Transform child in roomRoot.GetComponentsInChildren<Transform>(true))
        {
            string lowerName = child.name.ToLowerInvariant();
            if (lowerName == "goal" || lowerName.Contains("goalpost") || lowerName.Contains("goal post"))
                return child;
        }

        return goalPost;
    }

    void Update()
    {
        if (halfPatrolDistance <= 0.01f)
            return;

        Vector3 leftLimit = centerPosition - moveAxis * halfPatrolDistance;
        Vector3 rightLimit = centerPosition + moveAxis * halfPatrolDistance;
        Vector3 target = direction > 0 ? rightLimit : leftLimit;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            sideStepSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= 0.02f)
            direction *= -1;

        UpdateAnimation();
    }

    Vector3 GetMoveAxis()
    {
        if (goalPost == null)
            return Vector3.right;

        switch (patrolAxis)
        {
            case PatrolAxis.GoalLocalZ:
                return Flatten(goalPost.forward).normalized;
            case PatrolAxis.WorldX:
                return Vector3.right;
            case PatrolAxis.WorldZ:
                return Vector3.forward;
            default:
                return Flatten(goalPost.right).normalized;
        }
    }

    float CalculateHalfPatrolDistance()
    {
        float halfWidth = GetHalfWidthFromBounds();

        if (halfWidth <= 0.01f && goalPost != null)
        {
            Vector3 scale = goalPost.lossyScale;
            halfWidth = patrolAxis == PatrolAxis.GoalLocalZ || patrolAxis == PatrolAxis.WorldZ
                ? Mathf.Abs(scale.z) * 0.5f
                : Mathf.Abs(scale.x) * 0.5f;
        }

        if (halfWidth <= 0.01f)
            halfWidth = fallbackGoalWidth * 0.5f;

        return Mathf.Max(0f, halfWidth - postPadding - goalieRadius);
    }

    float GetHalfWidthFromBounds()
    {
        if (goalPost == null)
            return 0f;

        Bounds bounds;
        Collider collider = goalPost.GetComponentInChildren<Collider>();
        Renderer renderer = goalPost.GetComponentInChildren<Renderer>();

        if (collider != null)
        {
            bounds = collider.bounds;
        }
        else if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            return 0f;
        }

        Vector3 extents = bounds.extents;
        Vector3 axis = GetMoveAxis();
        return Mathf.Abs(extents.x * axis.x)
            + Mathf.Abs(extents.y * axis.y)
            + Mathf.Abs(extents.z * axis.z);
    }

    void ConfigureAnimator()
    {
        if (animator == null)
            return;

        hasSideStepState = !string.IsNullOrEmpty(sideStepStateName.Trim())
            && animator.HasState(0, Animator.StringToHash(sideStepStateName));
        sideStepBoolHash = Animator.StringToHash(sideStepBoolParameter);

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == sideStepBoolHash && parameter.type == AnimatorControllerParameterType.Bool)
            {
                hasSideStepBool = true;
                break;
            }
        }
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (hasSideStepBool)
            animator.SetBool(sideStepBoolHash, true);

        if (hasSideStepState)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(sideStepStateName))
                animator.CrossFade(sideStepStateName, animationFadeTime);
        }
    }

    Vector3 Flatten(Vector3 vector)
    {
        vector.y = 0f;
        return vector.sqrMagnitude > 0.001f ? vector : Vector3.right;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 previewCenter = goalPost != null ? goalPost.position : transform.position;
        previewCenter.y = transform.position.y;

        Vector3 previewAxis = Application.isPlaying ? moveAxis : GetMoveAxis();
        float previewHalfDistance = Application.isPlaying ? halfPatrolDistance : CalculateHalfPatrolDistance();

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            previewCenter - previewAxis * previewHalfDistance,
            previewCenter + previewAxis * previewHalfDistance
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(previewCenter, 0.08f);
    }
}
