using UnityEngine;

public class PartHighlighter : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private bool isHighlighting;

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private static readonly int ColorId = Shader.PropertyToID("_BaseColor"); // Standard URP property

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (!isHighlighting) return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        Color finalColor = Color.Lerp(Color.white, highlightColor, pulse);

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetColor(ColorId, finalColor);
            r.SetPropertyBlock(propBlock);
        }
    }

    public void SetHighlight(bool active)
    {
        isHighlighting = active;
        if (!active)
        {
            foreach (var r in renderers)
            {
                r.GetPropertyBlock(propBlock);
                propBlock.SetColor(ColorId, Color.white); // Reset to white/original
                r.SetPropertyBlock(propBlock);
            }
        }
    }
}
