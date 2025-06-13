using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]public Transform cameraTransform; // camera
    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += deltaMovement;
        lastCameraPosition = cameraTransform.position;
    }
}
