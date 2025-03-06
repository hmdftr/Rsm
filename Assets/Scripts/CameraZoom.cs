using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    public Camera zoomCamera;
    public float zoomDistance = 1f;
    public float zoomSpeed = 2f;

    private Vector3 originalZoomCameraPosition;
    private Quaternion originalZoomCameraRotation;
    private bool hasZoomed = false;
    private bool hasZoomedBack = false;

    private void Start()
    {
        originalZoomCameraPosition = zoomCamera.transform.localPosition;
        originalZoomCameraRotation = zoomCamera.transform.localRotation;

        zoomCamera.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!hasZoomed) // Zoom In
            {
                hasZoomed = true;
                mainCamera.enabled = false;
                zoomCamera.enabled = true;
                zoomCamera.transform.localPosition = originalZoomCameraPosition + zoomCamera.transform.forward * zoomDistance;
                Debug.Log("Zoom In activated."); // Debug message
            }
            else if (hasZoomed && !hasZoomedBack) // Zoom Out
            {
                hasZoomedBack = true;
                mainCamera.enabled = true;
                zoomCamera.enabled = false;
                zoomCamera.transform.localPosition = originalZoomCameraPosition;
                zoomCamera.transform.localRotation = originalZoomCameraRotation;
                Debug.Log("Zoom Out activated."); // Debug message
            }
            else if (hasZoomed && hasZoomedBack)
            {
                Debug.Log("Zoom functionality has been used and is now disabled."); // Final debug message
            }
        }

        if (zoomCamera.enabled)
        {
            zoomCamera.transform.localPosition = Vector3.Lerp(zoomCamera.transform.localPosition, originalZoomCameraPosition + zoomCamera.transform.forward * zoomDistance, zoomSpeed * Time.deltaTime);
        }
    }
}