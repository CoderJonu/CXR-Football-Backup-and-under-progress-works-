using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticSlidingDoors : MonoBehaviour
{
    [Header("Door Panels")]
    [SerializeField] private Transform panelA;
    [SerializeField] private Transform panelB;

    [Header("Opening")]
    [SerializeField] private float openingDistance = 1.55f;
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float activationDistance = 5f;
    [SerializeField] private float closeDelay = 0.75f;

    private Transform player;

    private Vector3 panelAClosedPosition;
    private Vector3 panelBClosedPosition;

    private Vector3 panelAOpenPosition;
    private Vector3 panelBOpenPosition;

    private Vector3 doorwayCenter;

    private float lastPlayerNearbyTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SetupSceneDoors()
    {
        SetupDoor("Door_L_Panel_A", "Door_L_Panel_B");
        SetupDoor("Door_R_Panel_A", "Door_R_Panel_B");
    }

    private static void SetupDoor(string panelAName, string panelBName)
    {
        Transform panelA = FindSceneTransform(panelAName);
        Transform panelB = FindSceneTransform(panelBName);

        if (panelA == null || panelB == null)
        {
            Debug.LogError($"Door not found: {panelAName} / {panelBName}");
            return;
        }

        AutomaticSlidingDoors door =
            panelA.GetComponent<AutomaticSlidingDoors>();

        if (door == null)
            door = panelA.gameObject.AddComponent<AutomaticSlidingDoors>();

        door.Initialize(panelA, panelB);
    }

    private static Transform FindSceneTransform(string objectName)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            Transform match =
                FindChildRecursive(root.transform, objectName);

            if (match != null)
                return match;
        }

        return null;
    }

    private static Transform FindChildRecursive(
        Transform parent,
        string objectName)
    {
        if (parent.name == objectName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform match =
                FindChildRecursive(child, objectName);

            if (match != null)
                return match;
        }

        return null;
    }

    private void Initialize(
        Transform firstPanel,
        Transform secondPanel)
    {
        panelA = firstPanel;
        panelB = secondPanel;

        panelAClosedPosition = panelA.position;
        panelBClosedPosition = panelB.position;

        Vector3 firstCenter = GetPanelCenter(panelA);
        Vector3 secondCenter = GetPanelCenter(panelB);

        doorwayCenter = (firstCenter + secondCenter) * 0.5f;

        Vector3 slideAxis = secondCenter - firstCenter;
        slideAxis.y = 0f;

        if (slideAxis.sqrMagnitude < 0.001f)
            slideAxis = Vector3.right;
        else
            slideAxis.Normalize();

        panelAOpenPosition =
            panelAClosedPosition - slideAxis * openingDistance;

        panelBOpenPosition =
            panelBClosedPosition + slideAxis * openingDistance;

        Debug.Log("Door Initialized");
    }

    private void Update()
    {
        if (panelA == null || panelB == null)
            return;

        ResolvePlayer();

        bool playerNearby =
            player != null &&
            HorizontalDistance(
                player.position,
                doorwayCenter
            ) <= activationDistance;

        if (playerNearby)
            lastPlayerNearbyTime = Time.time;

        bool shouldOpen =
            playerNearby ||
            Time.time - lastPlayerNearbyTime < closeDelay;

        panelA.position = Vector3.MoveTowards(
            panelA.position,
            shouldOpen ? panelAOpenPosition : panelAClosedPosition,
            movementSpeed * Time.deltaTime
        );

        panelB.position = Vector3.MoveTowards(
            panelB.position,
            shouldOpen ? panelBOpenPosition : panelBClosedPosition,
            movementSpeed * Time.deltaTime
        );
    }

    private void ResolvePlayer()
    {
        if (player != null)
            return;

        GameObject playerObj =
            GameObject.Find("Y Bot@Idle");

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player Found!");
        }
    }

    private static Vector3 GetPanelCenter(Transform panel)
    {
        Renderer r = panel.GetComponent<Renderer>();

        return r != null
            ? r.bounds.center
            : panel.position;
    }

    private static float HorizontalDistance(
        Vector3 a,
        Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}