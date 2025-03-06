using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float rotationSpeed = 100f; // Adjust this value

    public Transform cameraTransform; // Assign your camera here
    public Transform headTransform;   // Assign the character's head Transform here

    public Transform seatTransform; // Assign the seat Transform here
    public float seatAdjustmentSpeed = 2f; // Adjust this value

    public float minSeatX = -0.5f;  // Minimum X position of the seat
    public float maxSeatX = 0.5f;   // Maximum X position of the seat
    public float minSeatY = -0.2f; // Minimum Y position of the seat
    public float maxSeatY = 0.2f; // Maximum Y position of the seat
    public float minSeatZ = -0.3f; // Minimum Z position of the seat
    public float maxSeatZ = 0.3f; // Maximum Z position of the seat

    // Correct seat position (adjust these values)
    public float correctSeatX = 0f;
    public float correctSeatY = 0f;
    public float correctSeatZ = 0f;

    private bool seatCorrect = false;

    private float horizontalLookRotation;
    private float verticalLookRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Hide the cursor
    }

    private void Update()
    {
        // Horizontal Rotation (Left/Right) - Applied to the HEAD (limited)
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        horizontalLookRotation += mouseX;
        horizontalLookRotation = Mathf.Clamp(horizontalLookRotation, -60f, 60f); // Limit to 180 degrees total

        headTransform.localRotation = Quaternion.Euler(0f, horizontalLookRotation, 0f); // Rotate the head horizontally

        // Vertical Look Rotation (Up/Down) - Applied to the HEAD and CAMERA
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -20f, 40f); // Clamp to 180 degrees total

        headTransform.localRotation = Quaternion.Euler(verticalLookRotation, horizontalLookRotation, 0f); // Rotate the head (combining both)
        cameraTransform.localRotation = headTransform.localRotation; // Camera inherits head rotation

        // Camera Follow Head (position)
        cameraTransform.position = headTransform.position;

        if (!seatCorrect)
        {
            if (Input.GetKey(KeyCode.W))
            {
                seatTransform.localPosition += seatTransform.forward * seatAdjustmentSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                seatTransform.localPosition -= seatTransform.forward * seatAdjustmentSpeed * Time.deltaTime;
            }

            // Limit seat movement range (using Mathf.Clamp)
            Vector3 localPosition = seatTransform.localPosition;

            localPosition.x = Mathf.Clamp(localPosition.x, minSeatX, maxSeatX);
            localPosition.y = Mathf.Clamp(localPosition.y, minSeatY, maxSeatY);
            localPosition.z = Mathf.Clamp(localPosition.z, minSeatZ, maxSeatZ);

            seatTransform.localPosition = localPosition; // Apply the clamped position

            // Check if the seat is at the correct position (with tolerance)
            if (Mathf.Abs(localPosition.x - correctSeatX) < 0.05f &&  // Adjust tolerance (0.05f) as needed
                Mathf.Abs(localPosition.y - correctSeatY) < 0.05f &&
                Mathf.Abs(localPosition.z - correctSeatZ) < 0.05f)
            {
                seatCorrect = true;
                Debug.Log("Seat is now correctly positioned!");
            }
        }
    }
}