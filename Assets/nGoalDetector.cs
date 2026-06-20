using UnityEngine;
using TMPro; // Use this if you're using TextMeshPro

public class nGoalDetector : MonoBehaviour
{
    public GameObject goalTextUI; // Drag your 'Goal!' text object here in the Inspector
    private nGameManager gameManager;

    void Start()
    {
        Debug.Log("Manager Found: " + gameManager);
        gameManager = Object.FindFirstObjectByType<nGameManager>();

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

        if (other.CompareTag("nBall"))
        {
            Debug.Log("GOAL SCORED!");

            if (gameManager != null)
            {
                GameObject scoredBall = other.attachedRigidbody != null
                    ? other.attachedRigidbody.gameObject
                    : other.gameObject;

                gameManager.GoalScored(scoredBall);

                Debug.Log("GoalScored() called successfully.");
            }

            if (goalTextUI != null)
            {
                Debug.Log("Displaying GOAL text.");

                goalTextUI.SetActive(true);
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


    void HideGoalText()
    {
        if (goalTextUI != null)
        {
            goalTextUI.SetActive(false);
        }
    }
}
