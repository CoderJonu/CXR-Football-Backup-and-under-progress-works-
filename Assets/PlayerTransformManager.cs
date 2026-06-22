using UnityEngine;

public class PlayerTransformManager : MonoBehaviour
{
    [Header("Players")]
    public GameObject lobbyPlayer;
    public GameObject room1Player;
    public GameObject room2Player;

    void Start()
    {
        lobbyPlayer.SetActive(true);
        room1Player.SetActive(false);
        room2Player.SetActive(false);
    }

    public void EnterRoom1()
    {
        lobbyPlayer.SetActive(false);
        room1Player.SetActive(true);
        room2Player.SetActive(false);

        Debug.Log("Entered Room 1");
    }

    public void EnterRoom2()
    {
        lobbyPlayer.SetActive(false);
        room1Player.SetActive(false);
        room2Player.SetActive(true);

        Debug.Log("Entered Room 2");
    }

    public void ExitToLobby()
    {
        lobbyPlayer.SetActive(true);
        room1Player.SetActive(false);
        room2Player.SetActive(false);

        Debug.Log("Returned to Lobby");
    }
}
