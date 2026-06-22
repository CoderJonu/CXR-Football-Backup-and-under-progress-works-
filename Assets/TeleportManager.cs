using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public Transform player;
    public Transform gameSpawnPoint;

    public void StartGame()
    {
        if (player != null && gameSpawnPoint != null)
        {
            player.SetPositionAndRotation(
                gameSpawnPoint.position,
                gameSpawnPoint.rotation
            );
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Exiting Game...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}