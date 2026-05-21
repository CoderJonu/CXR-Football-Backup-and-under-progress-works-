using UnityEngine;
using TMPro; // Use this if you're using TextMeshPro

public class GoalDetector : MonoBehaviour
{
    public GameObject goalTextUI; // Drag your 'Goal!' text object here in the Inspector
    void Start()
    {
        // This makes sure the "Goal!" text is hidden the second the game runs
        goalTextUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // This will print the name of ANY object that touches the goal
        Debug.Log("Something hit the goal: " + other.gameObject.name);

        if (other.CompareTag("Ball"))
        {
            Debug.Log("The Ball tag was detected!");
            goalTextUI.SetActive(true);
            Invoke("HideGoalText", 3f);
        }
    }


    void HideGoalText()
    {
        goalTextUI.SetActive(false);
    }
}
