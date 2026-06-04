using UnityEngine;

public class FollowTracker : MonoBehaviour
{
    public Transform tracker;

    void Update()
    {
        transform.position = tracker.position;
        transform.rotation = tracker.rotation;
    }
}