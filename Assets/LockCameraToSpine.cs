using UnityEngine;

public class LockCamaraToSpine : MonoBehaviour
{
    // Drag your mixamorig:Head bone into this slot in the Inspector
    public Transform botHeadBone;
    public Transform bodyFollowTarget;

    // Adjust this height offset to match eye-level perfectly
    public float heightOffset = -1.5f;

    public bool flipForwardDirection = true;
    public float yawOffsetDegrees = 45f;
    public bool lockRotationToBody = false;
    public bool useStableBodyPosition = true;

    private Vector3 stableLocalOffset;

    void Start()
    {
        ResolveBodyFollowTarget();

        if (botHeadBone != null && bodyFollowTarget != null)
            stableLocalOffset = bodyFollowTarget.InverseTransformPoint(botHeadBone.position);
    }

    void LateUpdate()
    {
        if (botHeadBone != null)
        {
            ResolveBodyFollowTarget();

            Vector3 basePosition = useStableBodyPosition
                && bodyFollowTarget != null
                    ? bodyFollowTarget.TransformPoint(stableLocalOffset)
                : botHeadBone.position;

            Vector3 targetPosition = basePosition + (Vector3.up * heightOffset);
            transform.position = targetPosition;

            if (!lockRotationToBody)
                return;

            Quaternion forwardCorrection = flipForwardDirection
                ? Quaternion.Euler(0f, 180f, 0f)
                : Quaternion.identity;
            Quaternion yawCorrection = Quaternion.Euler(0f, yawOffsetDegrees, 0f);
            transform.rotation = botHeadBone.root.rotation * forwardCorrection * yawCorrection;
        }
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
