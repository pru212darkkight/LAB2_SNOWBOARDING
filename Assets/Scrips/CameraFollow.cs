using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(2f, 2f, -10f);
    [SerializeField] private float lookAheadFactor = 3f;
    
    private void LateUpdate()
    {
        if (player == null) return;
        
        // Calculate the target position with look-ahead based on player velocity
        Vector3 playerVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        float lookAheadX = Mathf.Sign(playerVelocity.x) * lookAheadFactor;
        
        Vector3 targetPosition = player.position + offset;
        targetPosition.x += lookAheadX;
        
        // Smooth camera movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
} 