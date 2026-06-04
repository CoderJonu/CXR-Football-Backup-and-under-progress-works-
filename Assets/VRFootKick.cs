using UnityEngine;

public class VRFootKick : MonoBehaviour
{
    [Header("VR Controller")]
    public Transform leftController;

    [Header("Movement")]
    public float footMoveSpeed = 15f;

    // Automatic offset
    private Vector3 initialOffset;

    void Start()
    {
        if (leftController != null)
        {
            // Calculate initial relative offset
            initialOffset = transform.position - leftController.position;
        }
    }

    void Update()
    {
        if (leftController == null)
            return;

        // Desired foot position
        Vector3 targetPosition =
            leftController.position;

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            footMoveSpeed * Time.deltaTime
        );
    }
}