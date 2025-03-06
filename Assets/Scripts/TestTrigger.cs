using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test Trigger: Object entered: " + other.gameObject.name + ", Tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Test Trigger: Player entered!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Test Trigger: Object exited: " + other.gameObject.name + ", Tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Test Trigger: Player exited!");
        }
    }
}