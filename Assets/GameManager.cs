using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Text Displays")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI goalsLeftText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI accuracyText;

    [Header("Match Settings")]
    public GameObject ballPrefab; // Put your clean ball asset template here
    public Vector3 kickOffPosition; // Coordinates where the ball spawns

    private const float MatchDuration = 240f; // 4 minutes total
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
        // Save the start position based on where you place this in the scene
        ball existingBall = Object.FindFirstObjectByType<ball>();
        if (existingBall != null)
        {
            currentBall = existingBall.gameObject;
            kickOffPosition = existingBall.transform.position;
        }

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        UpdateUIDisplays();
    }

    void Update()
    {
        if (isGameOver) return;

        // Run down the match timer clock
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUIDisplays();
        }
        else
        {
            timeRemaining = 0;
            EndGame(false); // Lost due to timeout
        }
    }

    public void GoalScored(GameObject scoredBall)
    {
        if (isGameOver || isSpawningBall) return;

        RemoveBall(scoredBall);

        goalsScored++;
        goalsRemaining--; // Reduce targets left
        UpdateUIDisplays();

        if (goalsRemaining <= 0)
        {
            EndGame(true); // Won the game
        }
        else
        {
            // Call the spawn loop for a fresh physical ball object
            isSpawningBall = true;
            Invoke(nameof(SpawnFreshBall), 0.5f);
        }
    }

    public void RegisterShot(GameObject shotBall)
    {
        if (isGameOver || shotBall == null)
            return;

        ball trackedBall = shotBall.GetComponent<ball>();
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
            currentBall = Instantiate(ballPrefab, kickOffPosition, Quaternion.identity);
        }
    }

    public void RespawnNewBall(GameObject oldBall)
    {
        if (isGameOver || isSpawningBall) return;

        // Out of bounds handler: Destroy old and drop new
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
        // Format decimal time into a readable MM:SS layout
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        if (timerText != null)
            timerText.text = string.Format("Time Left: {0:00}:{1:00}", minutes, seconds);

        if (goalsLeftText != null)
            goalsLeftText.text = "Goals Needed: " + goalsRemaining;

        if (accuracyText == null)
            accuracyText = CreateAccuracyText();

        if (accuracyText != null)
        {
            float accuracy = shotsTaken > 0
                ? (goalsScored / (float)shotsTaken) * 100f
                : 0f;

            accuracyText.text = string.Format(
                "Shots: {0}  Goals: {1}  Accuracy: {2:0}%",
                shotsTaken,
                goalsScored,
                accuracy
            );
        }
    }

    void EndGame(bool playerWon)
    {
        isGameOver = true;
        CancelInvoke(nameof(SpawnFreshBall));
        RemoveBall(currentBall);

        if (playerWon)
        {
            Debug.Log("VICTORY! You scored 10 goals in time.");
            ShowResult(BuildResultMessage("YOU WON!"));
        }
        else
        {
            Debug.Log("GAME OVER! Time ran out.");
            ShowResult(BuildResultMessage("YOU LOSE!"));
        }
    }

    string BuildResultMessage(string result)
    {
        float accuracy = shotsTaken > 0
            ? (goalsScored / (float)shotsTaken) * 100f
            : 0f;

        return string.Format(
            "{0}\nShots: {1}  Goals: {2}  Accuracy: {3:0}%",
            result,
            shotsTaken,
            goalsScored,
            accuracy
        );
    }

    void ShowResult(string message)
    {
        if (resultText == null)
            resultText = CreateResultText();

        if (resultText != null)
        {
            resultText.text = message;
            resultText.gameObject.SetActive(true);
        }
    }

    TextMeshProUGUI CreateResultText()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        GameObject resultObject = new GameObject("ResultText");
        resultObject.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI text = resultObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 0.18f;
        text.color = Color.yellow;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(1.1f, 0.35f);

        return text;
    }

    TextMeshProUGUI CreateAccuracyText()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return null;

        GameObject accuracyObject = new GameObject("AccuracyText");
        accuracyObject.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI text = accuracyObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 0.07f;
        text.color = Color.black;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 0.18f);
        rect.sizeDelta = new Vector2(1.4f, 0.14f);

        return text;
    }
}
