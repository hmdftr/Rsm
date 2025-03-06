using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Transform headTransform;

    private float verticalLookRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Movement (WASD or arrow keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        transform.position += moveDirection * movementSpeed * Time.deltaTime;


        // Rotation (Mouse)
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

        // ***CORRECTED HORIZONTAL ROTATION***
        transform.Rotate(Vector3.up * mouseX); // Rotate the BODY horizontally

        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        // ***CORRECTED VERTICAL ROTATION***
        headTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f); // Rotate the HEAD vertically
        cameraTransform.localRotation = headTransform.localRotation; // Camera inherits head rotation

        // Camera Follow Head (position)
        cameraTransform.position = headTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger: " + other.gameObject.name); // Debug log

        // Check if the other object has a specific tag (recommended)
        if (other.CompareTag("Interactable")) // Replace "Interactable" with your tag
        {
            Debug.Log("Player entered interactable trigger: " + other.gameObject.name);
            // Do something when the player enters the trigger (e.g., show a message, change a variable)
            // Example:
            // other.GetComponent<InteractableObject>().OnPlayerEnter(); // If you have a script on the interactable object
        }

        // Or, check if the other object has a specific component (less recommended, but can work)
        /*
        SceneTrigger trigger = other.GetComponent<SceneTrigger>(); // Example: if the trigger loads a scene
        if (trigger != null)
        {
            Debug.Log("Player entered scene trigger: " + other.gameObject.name);
            // trigger.LoadNextScene(); // Call a function on the trigger script
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited trigger: " + other.gameObject.name); // Debug log

        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Player exited interactable trigger: " + other.gameObject.name);
            // Do something when the player exits the trigger (e.g., hide a message)
        }

        /*
        SceneTrigger trigger = other.GetComponent<SceneTrigger>();
        if (trigger != null)
        {
            Debug.Log("Player exited scene trigger: " + other.gameObject.name);
        }
        */
    }
}