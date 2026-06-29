using UnityEngine;
using TMPro; // Use this if you're using TextMeshPro

public class nGoalDetector : MonoBehaviour
{
    public GameObject goalTextUI; // Drag your 'Goal!' text object here in the Inspector
    private nGameManager gameManager;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<nGameManager>();
        Debug.Log("Manager Found: " + gameManager);

        // This makes sure the "Goal!" text is hidden the second the game runs
        if (goalTextUI != null)
        {
            goalTextUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // This will print the name of ANY object that touches the goal
        Debug.Log("Something hit the goal: " + other.gameObject.name);

        GameObject scoredBall = GetScoredBall(other);

        if (scoredBall != null)
        {
            Debug.Log("GOAL SCORED!");

            // --- 1. UPDATE ROOM 2 GOAL COUNTER ON PLAYER CANVAS ---
            PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                player.IncreaseRoomTwoGoalCount();
            }

            // --- 2. SWITCH 10 DEFENDER BOARDS TO THE NEXT BLUE LOCK PATTERN ---
            if (DefensiveSystemManager.Instance != null)
            {
                DefensiveSystemManager.Instance.CycleDefensivePattern();
            }
            else
            {
                Debug.LogWarning("DefensiveSystemManager is missing from the scene! Make sure it is attached to an empty GameObject.");
            }

            // --- ORIGINAL GAME MANAGER CALLS ---
            if (gameManager == null)
                gameManager = Object.FindFirstObjectByType<nGameManager>();

            if (gameManager != null)
            {
                gameManager.GoalScored(scoredBall);

                Debug.Log("GoalScored() called successfully.");
            }

            // --- ORIGINAL CELEBRATION TEXT CONTROL ---
            if (goalTextUI != null)
            {
                Debug.Log("Displaying GOAL text.");

                goalTextUI.SetActive(true);
                Debug.Log("Goal text active state: " + goalTextUI.activeSelf);

                Invoke(nameof(HideGoalText), 5f);
            }
            else
            {
                Debug.LogWarning("goalTextUI is NOT assigned!");
            }
        }
    }

    GameObject GetScoredBall(Collider other)
    {
        if (other.CompareTag("nBall"))
            return other.attachedRigidbody != null
                ? other.attachedRigidbody.gameObject
                : other.gameObject;

        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<nBall>() != null)
            return other.attachedRigidbody.gameObject;

        nBall ball = other.GetComponentInParent<nBall>();
        return ball != null ? ball.gameObject : null;
    }

    void HideGoalText()
    {
        if (goalTextUI != null)
        {
            goalTextUI.SetActive(false);
        }
    }
}
