using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public GameObject uiPrompt;       // Drag the UI text here ("Press E")
    public Transform insideSpawnPoint; // Drag THIS room's spawn point here
    public GameObject player;          // Drag your player here

    [Header("Room UI Control")]
    public GameObject lobbyCanvas;     // Drag your main lobby canvas here
    public GameObject roomCanvas;      // Drag THIS room's canvas here (Timer, Goals, etc.)

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

        // --- ROOM CANVAS CONTROL ---
        // 1. Turn off the lobby screen so you can't see it anymore
        if (lobbyCanvas != null)
        {
            lobbyCanvas.SetActive(false);
        }

        // 2. Turn on this specific room's screen (Timer, goals, etc.)
        if (roomCanvas != null)
        {
            roomCanvas.SetActive(true);
        }

        // Trigger this specific room's gameplay loop
        Debug.Log(gameObject.name + " gameplay started!");
    }
}
