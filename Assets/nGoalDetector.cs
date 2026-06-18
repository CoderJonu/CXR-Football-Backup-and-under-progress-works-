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
            Debug.Log("The Ball tag was detected!");

            if (gameManager != null)
            {
                GameObject scoredBall = other.attachedRigidbody != null
                    ? other.attachedRigidbody.gameObject
                    : other.gameObject;

                gameManager.GoalScored(scoredBall);
            }

            if (goalTextUI != null)
            {
                goalTextUI.SetActive(true);
                Invoke(nameof(HideGoalText), 3f);
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
