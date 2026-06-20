using UnityEngine;
using TMPro;

public class nGameManager : MonoBehaviour
{
    [Header("UI Text Displays")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI goalsLeftText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI accuracyText;

    [Header("Match Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint; // Assign TrainingSpawnPoint here

    private const float MatchDuration = 240f;
    private const int GoalsToWin = 10;

    private float timeRemaining = MatchDuration;
    private int goalsRemaining = GoalsToWin;
    private int shotsTaken = 0;
    private int goalsScored = 0;

    private bool isGameOver = false;
    private bool isSpawningBall = false;

    private GameObject currentBall;

    void Start()
    {
        if (spawnPoint != null)
        {
            Debug.Log("Training Spawn Position: " + spawnPoint.position);
        }

        nBall existingBall = Object.FindFirstObjectByType<nBall>();

        if (existingBall != null)
        {
            currentBall = existingBall.gameObject;
        }

        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }

        UpdateUIDisplays();
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUIDisplays();
        }
        else
        {
            timeRemaining = 0;
            EndGame(false);
        }
    }

    public void GoalScored(GameObject scoredBall)
    {
        if (isGameOver || isSpawningBall) return;

        RemoveBall(scoredBall);

        goalsScored++;
        goalsRemaining--;

        Debug.Log("Goals Remaining: " + goalsRemaining);
        Debug.Log("Goals Scored: " + goalsScored);

        UpdateUIDisplays();

        if (goalsRemaining <= 0)
        {
            EndGame(true);
        }
        else
        {
            isSpawningBall = true;
            Invoke(nameof(SpawnFreshBall), 0.5f);
        }
    }

    public void RegisterShot(GameObject shotBall)
    {
        if (isGameOver || shotBall == null)
            return;

        nBall trackedBall = shotBall.GetComponent<nBall>();

        if (trackedBall != null && trackedBall.HasRegisteredShot)
            return;

        if (trackedBall != null)
            trackedBall.HasRegisteredShot = true;

        shotsTaken++;

        UpdateUIDisplays();
    }

    void SpawnFreshBall()
    {
        isSpawningBall = false;

        if (ballPrefab != null && !isGameOver)
        {
            Vector3 spawnPos = spawnPoint != null
                ? spawnPoint.position
                : Vector3.zero;

            currentBall = Instantiate(
                ballPrefab,
                spawnPos,
                Quaternion.identity
            );
        }
    }

    public void RespawnNewBall(GameObject oldBall)
    {
        if (isGameOver || isSpawningBall) return;

        RemoveBall(oldBall);

        isSpawningBall = true;

        Invoke(nameof(SpawnFreshBall), 0.2f);
    }

    void RemoveBall(GameObject ballToRemove)
    {
        if (ballToRemove == null)
            ballToRemove = currentBall;

        if (ballToRemove != null)
        {
            if (ballToRemove == currentBall)
                currentBall = null;

            Destroy(ballToRemove);
        }
    }

    void UpdateUIDisplays()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        if (timerText != null)
            timerText.text = $"Time Left: {minutes:00}:{seconds:00}";

        if (goalsLeftText != null)
            goalsLeftText.text = "Goals Needed: " + goalsRemaining;

        if (accuracyText != null)
        {
            float accuracy = shotsTaken > 0
                ? (goalsScored / (float)shotsTaken) * 100f
                : 0f;

            accuracyText.text =
                $"Shots: {shotsTaken} Goals: {goalsScored} Accuracy: {accuracy:0}%";
        }
    }

    void EndGame(bool playerWon)
    {
        isGameOver = true;

        CancelInvoke(nameof(SpawnFreshBall));

        RemoveBall(currentBall);

        if (playerWon)
        {
            Debug.Log("YOU WON!");
        }
        else
        {
            Debug.Log("YOU LOSE!");
        }
    }
}