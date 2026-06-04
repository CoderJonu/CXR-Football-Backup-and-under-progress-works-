using UnityEngine;

public class CubeFollowTest : MonoBehaviour
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