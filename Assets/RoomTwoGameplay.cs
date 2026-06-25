using UnityEngine;
using TMPro; // Using TextMeshPro for UI display

public class RoomTwoGameplay : MonoBehaviour
{
    public TextMeshProUGUI timerText;   // Drag your Canvas Timer Text here
    public TextMeshProUGUI goalsText;   // Drag your Canvas Goal Counter Text here

    public float timeRemaining = 60f;   // 1 minute timer
    public int totalGoalsRequired = 5;

    private int currentGoals = 0;
    private bool isRoomActive = false;

    // This runs the exact frame this body gets awakened by the door swap!
    void OnEnable()
    {
        isRoomActive = true;
        UpdateUI();
    }

    void Update()
    {
        if (!isRoomActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUI();
        }
        else
        {
            timeRemaining = 0;
            isRoomActive = false;
            RoomFailed();
        }
    }

    // Call this function whenever a goal is scored in Room 2!
    public void PlayerScoredGoal()
    {
        if (!isRoomActive) return;

        currentGoals++;
        UpdateUI();

        if (currentGoals >= totalGoalsRequired)
        {
            isRoomActive = false;
            RoomCompleted();
        }
    }

    void UpdateUI()
    {
        if (timerText != null) timerText.text = "Time Left: " + Mathf.CeilToInt(timeRemaining) + "s";
        if (goalsText != null) goalsText.text = "Goals: " + currentGoals + " / " + totalGoalsRequired;
    }

    void RoomCompleted()
    {
        Debug.Log("Room 2 Won! All goals scored.");
        // Add win actions here (Open door, show celebratory UI)
    }

    void RoomFailed()
    {
        Debug.Log("Room 2 Failed! Time ran out.");
        // Add failure actions here (Reset room, teleport back out)
    }
}
