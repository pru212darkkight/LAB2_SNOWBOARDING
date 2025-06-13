using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffect = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeFollowSpeed = 2f;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float currentRotation = 0f;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        // Get the slope angle at the camera position
        RaycastHit2D hit = Physics2D.Raycast(cameraTransform.position, Vector2.down, 10f, groundLayer);
        if (hit.collider != null)
        {
            // Calculate the slope angle
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopeAngle *= Mathf.Sign(hit.normal.x); // Make angle negative for uphill slopes
            slopeAngle = Mathf.Clamp(slopeAngle, -maxSlopeAngle, maxSlopeAngle);

            // Smoothly rotate the background to match the slope
            float targetRotation = -slopeAngle * 0.5f; // Use half the slope angle for subtle effect
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * slopeFollowSpeed);
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);

            // Calculate movement based on slope
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            Vector3 slopeAdjustedMovement = Quaternion.Euler(0, 0, slopeAngle) * deltaMovement;
            transform.position += new Vector3(
                slopeAdjustedMovement.x * parallaxEffect, 
                slopeAdjustedMovement.y * parallaxEffect, 
                0
            );
        }
        else
        {
            // If no ground is detected, just move normally
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);
        }
        
        lastCameraPosition = cameraTransform.position;

        // Handle background looping
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
} 