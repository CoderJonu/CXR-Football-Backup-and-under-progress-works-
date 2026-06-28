using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public GameObject uiPrompt;       // Drag the UI text here ("Press E")
    public Transform insideSpawnPoint; // Drag THIS room's spawn point here
    public GameObject player;          // Drag your player here

    [Header("Room Number Customisation")]
    public int roomNumber = 1;         // Set this to 1 for Room 1, and 2 for Room 2
    public GameObject lobbyCanvas;     // Drag your main lobby canvas here

    private bool isPlayerNearby = false;
    private bool isInRoom = false;
    private CharacterController charController;

    void Start()
    {
        if (player != null)
        {
            charController = player.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (isPlayerNearby && !isInRoom && Input.GetKeyDown(KeyCode.E))
        {
            EnterRoom();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInRoom)
        {
            isPlayerNearby = true;
            uiPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            uiPrompt.SetActive(false);
        }
    }

    void EnterRoom()
    {
        isInRoom = true;
        uiPrompt.SetActive(false);

        // Disabling CharacterController prevents teleportation bugs
        if (charController != null) charController.enabled = false;

        player.transform.position = insideSpawnPoint.position;

        if (charController != null) charController.enabled = true;

        // Hide the lobby canvas completely
        if (lobbyCanvas != null)
        {
            lobbyCanvas.SetActive(false);
        }

        // Send the room number directly to the player script!
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.InitializeRoomFunctionality(roomNumber);
        }

        Debug.Log(gameObject.name + " entered! Initialized functionality for Room " + roomNumber);
    }
}
