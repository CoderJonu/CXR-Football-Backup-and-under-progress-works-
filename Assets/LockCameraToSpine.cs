using UnityEngine;

public class LockCamaraToSpine : MonoBehaviour
{
    // Drag your mixamorig:Head bone into this slot in the Inspector
    public Transform botHeadBone;

    // Adjust this height offset to match eye-level perfectly
    public float heightOffset = -1.5f;

    public bool flipForwardDirection = true;
    public float yawOffsetDegrees = 45f;

    void LateUpdate()
    {
        if (botHeadBone != null)
        {
            // 1. Lock position cleanly to the head, matching its forward travel
            Vector3 targetPosition = botHeadBone.position + (Vector3.up * heightOffset);
            transform.position = targetPosition;

            // 2. ZERO SHAKE: Force the camera to look straight ahead based on the Bot's main body rotation
            // This completely ignores the head bone's animated bobbing and twisting!
            Quaternion forwardCorrection = flipForwardDirection
                ? Quaternion.Euler(0f, 180f, 0f)
                : Quaternion.identity;

            Quaternion yawCorrection = Quaternion.Euler(0f, yawOffsetDegrees, 0f);

            transform.rotation = botHeadBone.root.rotation * forwardCorrection * yawCorrection;
        }
    }
}
