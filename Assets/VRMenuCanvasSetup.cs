using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class VRMenuCanvasSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private EventSystem eventSystem;

    [Header("Placement")]
    [SerializeField] private float distanceFromCamera = 2.5f;
    [SerializeField] private float verticalOffset = -0.05f;
    [SerializeField] private Vector2 canvasSize = new Vector2(800f, 500f);
    [SerializeField] private float worldScale = 0.002f;
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool parentToCamera = true;

    [Header("Input")]
    [SerializeField] private bool addTrackedDeviceRaycaster = true;
    [SerializeField] private bool keepMouseInputInEditor = true;

    private Canvas canvas;
    private CanvasScaler canvasScaler;

    private void Reset()
    {
        targetCamera = Camera.main;
        eventSystem = FindObjectOfType<EventSystem>();
        ApplySetup();
    }

    private void Awake()
    {
        ApplySetup();
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        ApplySetup();
    }

    [ContextMenu("Apply VR Menu Setup")]
    public void ApplySetup()
    {
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();

        ResolveReferences();
        ConfigureCanvas();
        ConfigureRaycasters();
        ConfigureEventSystem();
        PlaceInFrontOfCamera();
    }

    private void ResolveReferences()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (eventSystem == null)
            eventSystem = FindObjectOfType<EventSystem>();
    }

    private void ConfigureCanvas()
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = targetCamera;
        canvas.planeDistance = distanceFromCamera;
        canvas.sortingOrder = 100;

        var rectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = canvasSize;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one * worldScale;

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = 1f;
        canvasScaler.referencePixelsPerUnit = 100f;
        canvasScaler.dynamicPixelsPerUnit = 10f;
    }

    private void ConfigureRaycasters()
    {
        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        if (addTrackedDeviceRaycaster && GetComponent<TrackedDeviceGraphicRaycaster>() == null)
            gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
    }

    private void ConfigureEventSystem()
    {
        if (eventSystem == null)
            return;

        var xrInputModule = eventSystem.GetComponent<XRUIInputModule>();
        if (xrInputModule == null)
            xrInputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();

        xrInputModule.enableXRInput = true;
        xrInputModule.enableMouseInput = keepMouseInputInEditor;

        var standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (standaloneInputModule != null)
            standaloneInputModule.enabled = false;
    }

    private void PlaceInFrontOfCamera()
    {
        if (targetCamera == null)
            return;

        Transform cameraTransform = targetCamera.transform;

        if (parentToCamera && transform.parent != cameraTransform)
            transform.SetParent(cameraTransform, false);

        if (parentToCamera)
        {
            transform.localPosition = new Vector3(0f, verticalOffset, distanceFromCamera);
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Vector3 forward = cameraTransform.forward;
            transform.position = cameraTransform.position + forward * distanceFromCamera + cameraTransform.up * verticalOffset;

            if (faceCamera)
                transform.rotation = Quaternion.LookRotation(forward, cameraTransform.up);
        }
    }
}
