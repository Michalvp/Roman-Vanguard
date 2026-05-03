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

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Village") isHubVersion = true;
    }

    public void Interact()
    {
        if (isHubVersion)
        {
            //Manage the skill tree UI for the hub version
            SkillTreeManager uiManager = Object.FindFirstObjectByType<SkillTreeManager>();
            if (uiManager != null)
            {
                uiManager.OpenTree(deityName);
            }
        }
        else
        {
            //Open the class selection confirmation for the non-hub version
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