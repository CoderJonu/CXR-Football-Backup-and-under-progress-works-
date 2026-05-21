using UnityEngine;
using UnityEngine.XR.Management;

public class MixedRealityConfigurator : MonoBehaviour
{
    [SerializeField] private bool enablePassthroughOnStart = true;
    [SerializeField] private Color passthroughBackgroundColor = new Color(0, 0, 0, 0);

    private void Start()
    {
        if (enablePassthroughOnStart)
        {
            SetupPassthrough();
        }
    }

    public void SetupPassthrough()
    {
        // For standard Unity AR/OpenXR, we often need to set the camera clear flags
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = passthroughBackgroundColor;
        }

        // Note: For Meta Quest 3 specifically, you would typically use:
        // OVRManager.instance.isInsightPassthroughEnabled = true;
        // This requires the Meta XR Core SDK to be installed.
        
        Debug.Log("Mixed Reality: Camera configured for Passthrough background.");
    }
}
