using UnityEngine;
using UnityEngine.SceneManagement; 

public class ClassStatue : MonoBehaviour, IInteractable
{
    public bool isHubVersion = false;
    public CharacterClassData classToGrant;
    public string deityName;
    [TextArea(3, 10)] // Allows for multi-line editing in Inspector
    public string classDescription;
    public GameObject glowObject;

    public void Interact()
    {
        if (isHubVersion)
        {
            // TUTAJ: Wywo³amy otwarcie drzewka umiejêtnoœci
            Debug.Log($"Opening Skill Tree for {deityName}!");
            // OpenSkillTree(); // To napiszemy w kolejnym kroku
        }
        else
        {
            // Logika wyboru klasy (to co ju¿ masz)
            ClassSelectionManager manager = Object.FindFirstObjectByType<ClassSelectionManager>();
            if (manager != null) manager.OpenConfirmation(classToGrant, deityName, classDescription);
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