using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public string nextSceneName; // Name of the scene to load
    public float animationDuration = 2f; // Duration of the door opening animation
    public float splashScreenDuration = 5f; // Duration of the splash screen

    private bool playerInRange = false;
    private bool doorOpen = false;
    private Animator doorAnimator; // Reference to the door's animator

    public GameObject splashScreen; // Assign your splash screen GameObject in the Inspector

    private void Start()
    {
        doorAnimator = GetComponent<Animator>(); // Get the animator component
        if (splashScreen != null)
        {
            splashScreen.SetActive(false); // Hide splash screen initially
        }
        else
        {
            Debug.LogError("Splash Screen not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !doorOpen) // E to interact
        {
            Debug.Log("Trying to open door"); // Add this line
            StartCoroutine(OpenDoorAndLoadScene());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            Debug.Log("Player entered door trigger"); // Add this line
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited door trigger"); // Add this line
            playerInRange = false;
        }
    }

    private IEnumerator OpenDoorAndLoadScene()
    {
        Debug.Log("Coroutine started"); // Add this line
        doorOpen = true;

        // 1. Door Animation:
        if (doorAnimator != null)
        {
            doorAnimator.Play("DoorOpen"); // Play your door opening animation.  Make sure you have a "DoorOpen" animation set up in the Animator.
            yield return new WaitForSeconds(animationDuration); // Wait for the animation to finish
        }
        else
        {
            Debug.LogWarning("No Animator component found on the door.");
            // If no animator, you could use transform changes to open the door, like:
            // transform.Rotate(0, 90, 0); // Example rotation
            yield return null; // Or yield return new WaitForSeconds(1f); if you have some other delay.
        }


        // 2. Splash Screen:
        if (splashScreen != null)
        {
            splashScreen.SetActive(true);
        }

        yield return new WaitForSeconds(splashScreenDuration);

        Debug.Log("Scene loading: " + nextSceneName); // Add this line
        // 3. Load Next Scene:
        SceneManager.LoadScene(nextSceneName);
        Debug.Log("Scene loaded"); // Add this line
    }
}