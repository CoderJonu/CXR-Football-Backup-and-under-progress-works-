using UnityEngine;

public class PCComponentMetadata : MonoBehaviour
{
    public PCComponentData data;
    public Transform uiAnchor; // Optional point where UI should appear

    private void Reset()
    {
        // Default anchor to above the object if not set
        if (uiAnchor == null) uiAnchor = transform;
    }
}
