using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;

public class FloatingUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI specsText;
    public TextMeshProUGUI descriptionText;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float fadeSpeed = 5f;
    public float followSpeed = 10f;
    public Vector3 offset = new Vector3(0, 0.3f, 0);

    private Transform targetAnchor;
    private Camera mainCamera;
    private bool isVisible = true;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        mainCamera = Camera.main;
        if (canvasGroup != null) canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Show(PCComponentMetadata metadata)
    {
        if (metadata == null || metadata.data == null) return;

        UpdateContent(metadata.data);
        targetAnchor = metadata.uiAnchor != null ? metadata.uiAnchor : metadata.transform;
        
        // Initial position
        transform.position = targetAnchor.position + offset;
        
        gameObject.SetActive(true);
        Fade(true);
    }

    public void Hide()
    {
        Fade(false);
    }

    private void UpdateContent(PCComponentData data)
    {
        if (nameText) nameText.text = data.componentName;
        if (typeText) typeText.text = data.componentType;
        if (descriptionText) descriptionText.text = data.description;

        if (specsText)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var spec in data.specifications)
            {
                sb.AppendLine("• " + spec);
            }
            specsText.text = sb.ToString();
        }
    }

    private void LateUpdate()
    {
        if (targetAnchor == null || !gameObject.activeSelf) return;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetAnchor.position + offset, Time.deltaTime * followSpeed);

        // Billboard effect
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void Fade(bool show)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(show));
    }

    private IEnumerator FadeRoutine(bool show)
    {
        float targetAlpha = show ? 1 : 0;
        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        if (!show)
        {
            gameObject.SetActive(false);
            targetAnchor = null;
        }
    }
}
