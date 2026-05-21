using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 360, 0);
    [SerializeField] private bool isRunning = true;

    public void SetRunning(bool running) => isRunning = running;

    private void Update()
    {
        if (isRunning)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}
