using UnityEngine;
using UnityEngine.SceneManagement; 

public class ClassStatue : MonoBehaviour, IInteractable
{
    public CharacterClassData classToGrant;
    public string deityName;
    [TextArea(3, 10)] // Allows for multi-line editing in Inspector
    public string classDescription;
    public GameObject glowObject;

    public void Interact()
    {
        // Find the manager and tell it to show this class
        ClassSelectionManager manager = Object.FindFirstObjectByType<ClassSelectionManager>();
        if (manager != null)
        {
            manager.OpenConfirmation(classToGrant, deityName, classDescription);
        }

    }

    public void SetHighlight(bool isActive)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(isActive);
        }
    }
}