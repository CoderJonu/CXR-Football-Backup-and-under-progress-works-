using UnityEngine;

public class DoorPanelB : MonoBehaviour
{
    public float moveDistance = 2f;
    public float speed = 2f;

    Vector3 closedPos;
    Vector3 openPos;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.right * moveDistance;
    }

    public void OpenDoor()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            openPos,
            speed * Time.deltaTime
        );
    }
}