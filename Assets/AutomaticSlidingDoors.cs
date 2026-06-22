using UnityEngine;

public class AutomaticSlidingDoors : MonoBehaviour
{
    [Header("Door Panels")]
    [SerializeField] private Transform panelA;
    [SerializeField] private Transform panelB;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Door Settings")]
    [SerializeField] private float openingDistance = 1.55f;
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float activationDistance = 5f;
    [SerializeField] private float closeDelay = 0.75f;

    private Vector3 panelAClosedPosition;
    private Vector3 panelBClosedPosition;

    private Vector3 panelAOpenPosition;
    private Vector3 panelBOpenPosition;

    private Vector3 doorwayCenter;

    private float lastPlayerNearbyTime;

    private void Start()
    {
        if (panelA == null || panelB == null)
        {
            Debug.LogError("Assign Panel A and Panel B in the Inspector on " + gameObject.name);
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject lobbyPlayer = GameObject.Find("Lobby Player");

            if (lobbyPlayer != null)
            {
                player = lobbyPlayer.transform;
            }
            else
            {
                Debug.LogWarning("Lobby Player not found. Assign it manually in the Inspector.");
            }
        }

        panelAClosedPosition = panelA.position;
        panelBClosedPosition = panelB.position;

        Vector3 centerA = GetPanelCenter(panelA);
        Vector3 centerB = GetPanelCenter(panelB);

        doorwayCenter = (centerA + centerB) * 0.5f;

        Vector3 slideAxis = centerB - centerA;
        slideAxis.y = 0f;

        if (slideAxis.sqrMagnitude < 0.001f)
            slideAxis = Vector3.right;
        else
            slideAxis.Normalize();

        panelAOpenPosition = panelAClosedPosition - slideAxis * openingDistance;
        panelBOpenPosition = panelBClosedPosition + slideAxis * openingDistance;

        Debug.Log(gameObject.name + " initialized successfully.");
    }

    private void Update()
    {
        if (player == null)
            return;

        bool playerNearby =
            HorizontalDistance(player.position, doorwayCenter) <= activationDistance;

        if (playerNearby)
            lastPlayerNearbyTime = Time.time;

        bool shouldOpen =
            playerNearby ||
            Time.time - lastPlayerNearbyTime < closeDelay;

        panelA.position = Vector3.MoveTowards(
            panelA.position,
            shouldOpen ? panelAOpenPosition : panelAClosedPosition,
            movementSpeed * Time.deltaTime);

        panelB.position = Vector3.MoveTowards(
            panelB.position,
            shouldOpen ? panelBOpenPosition : panelBClosedPosition,
            movementSpeed * Time.deltaTime);
    }

    private Vector3 GetPanelCenter(Transform panel)
    {
        Renderer r = panel.GetComponent<Renderer>();

        if (r != null)
            return r.bounds.center;

        return panel.position;
    }

    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }
}