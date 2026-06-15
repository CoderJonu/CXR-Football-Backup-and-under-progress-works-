using UnityEngine;

public class DoorPanelA : MonoBehaviour
{
    public float moveDistance = 2f;
    public float speed = 2f;

    Vector3 closedPos;
    Vector3 openPos;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.left * moveDistance;
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