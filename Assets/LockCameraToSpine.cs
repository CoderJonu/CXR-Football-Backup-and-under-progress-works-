using UnityEngine;

public class LockCamaraToSpine : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform botHeadBone;
    public Transform bodyFollowTarget;

    [Header("Position")]
    public float heightOffset = -1.5f;
    public bool useStableBodyPosition = true;
    [Range(0f, 1f)] public float headMotionAmount = 0.18f;
    [Min(0.01f)] public float positionSmoothTime = 0.08f;

    [Header("Rotation")]
    public bool flipForwardDirection = true;
    public float yawOffsetDegrees = 45f;
    public bool lockRotationToBody = false;
    [Min(0f)] public float rotationSmoothSpeed = 10f;

    private Vector3 stableLocalOffset;
    private Vector3 positionVelocity;
    private bool hasInitializedPosition;

    void Start()
    {
        ResolveBodyFollowTarget();

        if (botHeadBone != null && bodyFollowTarget != null)
            stableLocalOffset = bodyFollowTarget.InverseTransformPoint(botHeadBone.position);

        SnapToTarget();
    }

    void LateUpdate()
    {
        if (botHeadBone == null)
            return;

        ResolveBodyFollowTarget();

        Vector3 targetPosition = CalculateTargetPosition();
        if (!hasInitializedPosition || positionSmoothTime <= 0.01f)
        {
            transform.position = targetPosition;
            hasInitializedPosition = true;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref positionVelocity,
                positionSmoothTime
            );
        }

        if (!lockRotationToBody)
            return;

        Quaternion forwardCorrection = flipForwardDirection
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.identity;
        Quaternion yawCorrection = Quaternion.Euler(0f, yawOffsetDegrees, 0f);
        Quaternion targetRotation = botHeadBone.root.rotation * forwardCorrection * yawCorrection;

        float blend = rotationSmoothSpeed <= 0f
            ? 1f
            : 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, blend);
    }

    Vector3 CalculateTargetPosition()
    {
        Vector3 animatedHeadPosition = botHeadBone.position;
        if (!useStableBodyPosition || bodyFollowTarget == null)
            return animatedHeadPosition + Vector3.up * heightOffset;

        Vector3 stableHeadPosition = bodyFollowTarget.TransformPoint(stableLocalOffset);
        Vector3 reducedHeadMotion = Vector3.Lerp(
            stableHeadPosition,
            animatedHeadPosition,
            headMotionAmount
        );

        return reducedHeadMotion + Vector3.up * heightOffset;
    }

    void SnapToTarget()
    {
        if (botHeadBone == null)
            return;

        transform.position = CalculateTargetPosition();
        hasInitializedPosition = true;
    }

    void ResolveBodyFollowTarget()
    {
        if (bodyFollowTarget != null || botHeadBone == null)
            return;

        CharacterController characterController = botHeadBone.GetComponentInParent<CharacterController>();
        bodyFollowTarget = characterController != null
            ? characterController.transform
            : botHeadBone.root;
    }
}
