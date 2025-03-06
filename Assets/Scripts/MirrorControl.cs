using UnityEngine;

public class MirrorControl : MonoBehaviour
{
    public Transform leftMirror;
    public Transform rightMirror;
    public Transform backMirror;

    public float rotationSpeed = 50f;
    public float maxRotation = 5f;

    // Target rotation angles for each mirror (adjust these values)
    public float leftMirrorTargetX = 0f; // Changed to X
    public float leftMirrorTargetZ = 0f;
    public float rightMirrorTargetX = 0f; // Changed to X
    public float rightMirrorTargetZ = 0f;
    public float backMirrorTargetX = 0f; // Changed to X
    public float backMirrorTargetZ = 0f;

    // Correct rotation angles (adjust these values - these are the "correct" angles)
    public float leftMirrorCorrectX = 0f; // Changed to X
    public float leftMirrorCorrectZ = 0f;
    public float rightMirrorCorrectX = 0f; // Changed to X
    public float rightMirrorCorrectZ = 0f;
    public float backMirrorCorrectX = 0f; // Changed to X
    public float backMirrorCorrectZ = 0f;

    // Boolean flags to track if the mirrors are correctly aligned
    private bool leftMirrorCorrect = false;
    private bool rightMirrorCorrect = false;
    private bool backMirrorCorrect = false;

    private void Update()
    {
        // Left Mirror Controls
        HandleMirrorRotation(leftMirror, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            ref leftMirrorTargetX, ref leftMirrorTargetZ, ref leftMirrorCorrect, leftMirrorCorrectX, leftMirrorCorrectZ); // Changed to X

        // Right Mirror Controls
        HandleMirrorRotation(rightMirror, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8,
            ref rightMirrorTargetX, ref rightMirrorTargetZ, ref rightMirrorCorrect, rightMirrorCorrectX, rightMirrorCorrectZ); // Changed to X

        // Back Mirror Controls
        HandleMirrorRotation(backMirror, KeyCode.O, KeyCode.P, KeyCode.K, KeyCode.L,
            ref backMirrorTargetX, ref backMirrorTargetZ, ref backMirrorCorrect, backMirrorCorrectX, backMirrorCorrectZ); // Changed to X
    }

    private void HandleMirrorRotation(Transform mirror, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey,
        ref float targetRotationX, ref float targetRotationZ, ref bool isCorrect, float correctRotationX, float correctRotationZ) // Changed to X
    {
        if (mirror == null || isCorrect) return; // If mirror is null or already correct, do nothing

        float rotationX = 0f; // Changed to X
        float rotationZ = 0f;

        if (Input.GetKey(upKey))
        {
            rotationX = rotationSpeed * Time.deltaTime; // Changed to X
        }
        if (Input.GetKey(downKey))
        {
            rotationX = -rotationSpeed * Time.deltaTime; // Changed to X
        }
        if (Input.GetKey(leftKey))
        {
            rotationZ = rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(rightKey))
        {
            rotationZ = -rotationSpeed * Time.deltaTime;
        }

        if (rotationX != 0 || rotationZ != 0)
        {
            targetRotationX = Mathf.Clamp(targetRotationX + rotationX, -maxRotation, maxRotation); // Changed to X
            targetRotationZ = Mathf.Clamp(targetRotationZ + rotationZ, -maxRotation, maxRotation);


            Vector3 currentRotation = mirror.localRotation.eulerAngles;
            float newRotationX = Mathf.LerpAngle(currentRotation.x, targetRotationX, rotationSpeed * Time.deltaTime); // Changed to X
            float newRotationZ = Mathf.LerpAngle(currentRotation.z, targetRotationZ, rotationSpeed * Time.deltaTime);

            mirror.localRotation = Quaternion.Euler(newRotationX, currentRotation.y, newRotationZ); // Keep original Y


            // Check if the mirror is at the correct angle (you might want a tolerance here)
            if (Mathf.Abs(newRotationX - correctRotationX) < 0.5f && Mathf.Abs(newRotationZ - correctRotationZ) < 0.5f) // Changed to X
            {
                isCorrect = true;
                Debug.Log(mirror.name + " is now correctly aligned!");
            }
        }
    }
}