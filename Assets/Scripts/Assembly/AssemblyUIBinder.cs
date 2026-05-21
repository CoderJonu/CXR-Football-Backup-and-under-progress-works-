using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyUIBinder : MonoBehaviour
{
    [Header("Optional UI")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private float errorVisibleSeconds = 2f;

    private float errorExpireTime;

    private void Update()
    {
        if (errorText == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(errorText.text) && Time.time >= errorExpireTime)
        {
            errorText.text = string.Empty;
        }
    }

    public void SetInstruction(string message)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
        }
    }

    public void SetInfo(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
        }
    }

    public void SetProgress(int currentStep, int totalSteps)
    {
        var clampedTotal = Mathf.Max(1, totalSteps);
        var normalized = Mathf.Clamp01((float)currentStep / clampedTotal);

        if (progressSlider != null)
        {
            progressSlider.normalizedValue = normalized;
        }

        if (progressText != null)
        {
            progressText.text = $"Step {Mathf.Min(currentStep + 1, totalSteps)} / {totalSteps}";
        }
    }

    public void ShowError(string message)
    {
        if (errorText == null)
        {
            return;
        }

        errorText.text = message;
        errorExpireTime = Time.time + errorVisibleSeconds;
    }
}

