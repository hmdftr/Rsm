using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ObjectivesManager : MonoBehaviour
{
    public Color completedColor = Color.green;

    [System.Serializable]
    public struct Objective
    {
        public string description;
        public bool isCompleted;
        public GameObject tickImage;
        public Text objectiveTextUI;
    }

    public List<Objective> objectives = new List<Objective>();

    public Transform leftMirror;
    public Transform rightMirror;
    public Transform backMirror;
    public Transform seatTransform;

    public Camera mainCamera;
    public Camera zoomCamera;
    public float zoomDistance = 1f;
    public float zoomSpeed = 2f;

    public float correctSeatX = 0f;
    public float correctSeatY = 0f;
    public float correctSeatZ = 0f;

    public float leftMirrorCorrectX = 0f;
    public float leftMirrorCorrectZ = 0f;
    public float rightMirrorCorrectX = 0f;
    public float rightMirrorCorrectZ = 0f;
    public float backMirrorCorrectX = 0f;
    public float backMirrorCorrectZ = 0f;

    public GameObject mainMenuButton; // Add a public variable for your Main Menu Button
    public GameObject pausePanel;      // Add a public variable for your Pause Panel

    private bool gamePaused = false;   // Add a flag to track pause state

    private ObjectiveState currentObjective = ObjectiveState.AdjustSeat;

    private bool seatCorrect = false;
    private bool mirrorsCorrect = false;
    private bool zoomChecked = false;

    private Vector3 originalZoomCameraPosition;
    private Quaternion originalZoomCameraRotation;
    private bool hasZoomed = false;
    private bool hasZoomedBack = false;

    private bool seatControlsEnabled = true;
    private bool mirrorControlsEnabled = true;
    private bool zoomControlsEnabled = true;

    private bool leftMirrorCorrect = false;
    private bool rightMirrorCorrect = false;
    private bool backMirrorCorrect = false;

    private const float POSITION_TOLERANCE = 0.05f;
    private const float ROTATION_TOLERANCE = 0.5f;
    private const float SEAT_MOVE_SPEED = 2f;
    private const float MIRROR_ROTATE_SPEED = 50f;


    private enum ObjectiveState
    {
        AdjustSeat,
        AdjustMirrors,
        CheckMeterZoom,
        AllComplete
    }

    private void Start()
    {
        originalZoomCameraPosition = zoomCamera.transform.localPosition;
        originalZoomCameraRotation = zoomCamera.transform.localRotation;
        zoomCamera.enabled = false;

        while (objectives.Count < 4) // Ensure at least 4 objectives
        {
            objectives.Add(new Objective { description = "All objectives complete! Go to Main Menu", isCompleted = false });
        }

        InitializeObjectives();
        Cursor.lockState = CursorLockMode.Locked;

        mainMenuButton.SetActive(false); // Hide the button initially
        pausePanel.SetActive(false);     // Hide the pause panel initially
    }

    private void InitializeObjectives()
    {
        foreach (Objective objective in objectives)
        {
            if (objective.tickImage != null)
            {
                objective.tickImage.SetActive(false);
            }
        }
        UpdateObjectiveText();
    }

    private void Update()
    {
        if (currentObjective != ObjectiveState.AllComplete)
        {
            switch (currentObjective)
            {
                case ObjectiveState.AdjustSeat:
                    HandleSeatAdjustment();
                    break;
                case ObjectiveState.AdjustMirrors:
                    HandleMirrorAdjustment();
                    break;
                case ObjectiveState.CheckMeterZoom:
                    HandleMeterZoom();
                    break;
            }
        }
        else
        {
            objectives[3].objectiveTextUI.text = "All objectives complete! Go to Main Menu";
            objectives[3].objectiveTextUI.color = completedColor; // Future/Unstarted Objective

            if (objectives[3].tickImage != null)
            {
                objectives[3].tickImage.SetActive(false);
            }
            if (!gamePaused) // Pause only once
            {
                PauseGame();
            }
            // "Go to Main Menu" logic here (e.g., SceneManager.LoadScene("MainMenu");)
        }
    }

    private void HandleSeatAdjustment()
    {
        if (!seatCorrect && seatControlsEnabled)
        {
            float moveDirection = 0f;
            if (Input.GetKey(KeyCode.W)) moveDirection = 1f;
            if (Input.GetKey(KeyCode.S)) moveDirection = -1f;

            seatTransform.localPosition += seatTransform.forward * moveDirection * SEAT_MOVE_SPEED * Time.deltaTime;

            Vector3 localPosition = seatTransform.localPosition;
            localPosition.x = Mathf.Clamp(localPosition.x, -0.5f, 0.5f);
            localPosition.y = Mathf.Clamp(localPosition.y, -0.2f, 0.2f);
            localPosition.z = Mathf.Clamp(localPosition.z, -0.3f, 0.3f);
            seatTransform.localPosition = localPosition;

            if (Mathf.Abs(localPosition.x - correctSeatX) < POSITION_TOLERANCE &&
                Mathf.Abs(localPosition.y - correctSeatY) < POSITION_TOLERANCE &&
                Mathf.Abs(localPosition.z - correctSeatZ) < POSITION_TOLERANCE)
            {
                seatCorrect = true;
                seatControlsEnabled = false;

                int seatObjectiveIndex = (int)ObjectiveState.AdjustSeat;
                Objective currentObjectiveStruct = objectives[seatObjectiveIndex];
                currentObjectiveStruct.isCompleted = true;
                currentObjectiveStruct.tickImage.SetActive(true);
                objectives[seatObjectiveIndex] = currentObjectiveStruct;


                currentObjective = ObjectiveState.AdjustMirrors;
                UpdateObjectiveText();
            }
        }
    }

    private void HandleMirrorAdjustment()
    {
        if (!mirrorsCorrect && mirrorControlsEnabled)
        {
            HandleMirrorRotation(leftMirror, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, ref leftMirrorCorrect);
            HandleMirrorRotation(rightMirror, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, ref rightMirrorCorrect);
            HandleMirrorRotation(backMirror, KeyCode.O, KeyCode.P, KeyCode.K, KeyCode.L, ref backMirrorCorrect);

            if (leftMirrorCorrect && rightMirrorCorrect && backMirrorCorrect)
            {
                mirrorsCorrect = true;
                mirrorControlsEnabled = false;

                int mirrorObjectiveIndex = (int)ObjectiveState.AdjustMirrors;
                Objective currentObjectiveStruct = objectives[mirrorObjectiveIndex];
                currentObjectiveStruct.isCompleted = true;
                currentObjectiveStruct.tickImage.SetActive(true);
                objectives[mirrorObjectiveIndex] = currentObjectiveStruct;

                currentObjective = ObjectiveState.CheckMeterZoom;
                UpdateObjectiveText();
            }
        }
    }

    private void HandleMeterZoom()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!hasZoomed)
            {
                hasZoomed = true;
                mainCamera.enabled = false;
                zoomCamera.enabled = true;
                zoomCamera.transform.localPosition = originalZoomCameraPosition + zoomCamera.transform.forward * zoomDistance;
            }
            else if (hasZoomed && !hasZoomedBack)
            {
                hasZoomedBack = true;
                mainCamera.enabled = true;
                zoomCamera.enabled = false;
                zoomCamera.transform.localPosition = originalZoomCameraPosition;
                zoomCamera.transform.localRotation = originalZoomCameraRotation;
            }
            else if (hasZoomed && hasZoomedBack)
            {
                zoomChecked = true;

                int zoomObjectiveIndex = (int)ObjectiveState.CheckMeterZoom;
                Objective currentObjectiveStruct = objectives[zoomObjectiveIndex];
                currentObjectiveStruct.isCompleted = true;
                currentObjectiveStruct.tickImage.SetActive(true);
                objectives[zoomObjectiveIndex] = currentObjectiveStruct;

                currentObjective = ObjectiveState.AllComplete;
                UpdateObjectiveText();
            }
        }

        if (zoomCamera.enabled)
        {
            zoomCamera.transform.localPosition = Vector3.Lerp(zoomCamera.transform.localPosition, originalZoomCameraPosition + zoomCamera.transform.forward * zoomDistance, zoomSpeed * Time.deltaTime);
        }
    }

    private void UpdateObjectiveText()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].objectiveTextUI != null)
            {
                if ((ObjectiveState)i == currentObjective)
                {
                    objectives[i].objectiveTextUI.color = Color.white; // Current Objective
                }
                else if (objectives[i].isCompleted)
                {
                    objectives[i].objectiveTextUI.color = completedColor; // Completed Objective
                }
                else
                {
                    objectives[i].objectiveTextUI.color = Color.grey; // Future/Unstarted Objective
                }

                objectives[i].objectiveTextUI.text = objectives[i].description; // Update the text itself
            }
        }
    }

    private void HandleMirrorRotation(Transform mirror, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey, ref bool mirrorCorrect)
    {
        if (!mirror || !mirrorControlsEnabled || mirrorCorrect) return;

        float rotationY = Input.GetKey(leftKey) ? MIRROR_ROTATE_SPEED * Time.deltaTime : Input.GetKey(rightKey) ? -MIRROR_ROTATE_SPEED * Time.deltaTime : 0f; // Left/Right now control Y
        float rotationZ = Input.GetKey(upKey) ? MIRROR_ROTATE_SPEED * Time.deltaTime : Input.GetKey(downKey) ? -MIRROR_ROTATE_SPEED * Time.deltaTime : 0f;   // Up/Down now control Z

        if (rotationY != 0 || rotationZ != 0)
        {
            Vector3 currentRotation = mirror.localRotation.eulerAngles;
            float newRotationY = Mathf.Clamp(currentRotation.y + rotationY, -5f, 5f); // Clamp around Y-axis
            float newRotationZ = Mathf.Clamp(currentRotation.z + rotationZ, -5f, 5f); // Clamp around Z-axis
            mirror.localRotation = Quaternion.Euler(currentRotation.x, newRotationY, newRotationZ); // Keep original X

            float targetY = (mirror == leftMirror) ? leftMirrorCorrectZ : (mirror == rightMirror) ? rightMirrorCorrectZ : backMirrorCorrectZ; // Use Z correct values for Y now
            float targetZ = (mirror == leftMirror) ? leftMirrorCorrectX : (mirror == rightMirror) ? rightMirrorCorrectX : backMirrorCorrectX; // Use X correct values for Z now

            if (Mathf.Abs(newRotationY - targetY) < ROTATION_TOLERANCE &&
                Mathf.Abs(newRotationZ - targetZ) < ROTATION_TOLERANCE)
            {
                mirrorCorrect = true;
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Freeze the game
        gamePaused = true;
        Cursor.lockState = CursorLockMode.None; // Show the cursor
        Cursor.visible = true;

        mainMenuButton.SetActive(true); // Show the Main Menu button
        pausePanel.SetActive(true);     // Show the pause panel
    }

    public void GoToMainMenu() // This function will be called by the button
    {
        Time.timeScale = 1f; // Resume the game (important for future scene loads)
        SceneManager.LoadScene("MainMenu"); // Load your main menu scene
    }
}