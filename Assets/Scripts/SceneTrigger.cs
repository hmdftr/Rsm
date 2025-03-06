using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    public string nextSceneName; // Name of the scene to load
    public float splashScreenDuration = 5f; // Duration of the splash screen

    public GameObject splashScreen; // Assign your splash screen GameObject in the Inspector

    private bool playerInRange = false;

    private void Start()
    {
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
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) // E to interact (or any key)
        {
            StartCoroutine(LoadSceneWithSplash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            Debug.Log("Player entered the trigger area.");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger area.");
            playerInRange = false;
        }
    }

    private IEnumerator LoadSceneWithSplash()
    {
        // 1. Splash Screen:
        if (splashScreen != null)
        {
            splashScreen.SetActive(true);
        }

        yield return new WaitForSeconds(splashScreenDuration);

        // 2. Load Next Scene:
        SceneManager.LoadScene(nextSceneName);
    }
}