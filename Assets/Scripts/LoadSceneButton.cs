using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName; // The name of the scene to load
    public Button loadButton; // Assign your button in the Inspector

    private void Start()
    {
        if (loadButton == null)
        {
            Debug.LogError("Load Button not assigned in the Inspector!");
            return; // Exit early if the button isn't assigned
        }

        // Add a listener to the button's onClick event
        loadButton.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}