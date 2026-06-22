using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public PlayerTransformManager playerManager;

    public enum RoomType
    {
        Room1,
        Room2,
        Exit
    }

    public RoomType room;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Object is NOT tagged Player");
            return;
        }

        Debug.Log("Player entered " + room);

        switch (room)
        {
            case RoomType.Room1:
                Debug.Log("Switching to Room 1");
                playerManager.EnterRoom1();
                break;

            case RoomType.Room2:
                Debug.Log("Switching to Room 2");
                playerManager.EnterRoom2();
                break;

            case RoomType.Exit:
                Debug.Log("Returning to Lobby");
                playerManager.ExitToLobby();
                break;
        }
    }
}