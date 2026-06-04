using UnityEngine;

public class FollowController : MonoBehaviour
{
    public Transform leftController;

    void Update()
    {
        if (leftController != null)
        {
            transform.position = leftController.position;
        }
    }
}