using UnityEngine;
using UnityEngine.SceneManagement;

public class Returntovillage : MonoBehaviour, IInteractable
{
    public GameObject glowObject;

    public GameObject retreatPopup;

    public void SetHighlight(bool isActive)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(isActive);
        }
    }

    public void Interact()
    {
        if (retreatPopup != null)
            {
            retreatPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("Retreat popup is not assigned in the inspector.");
        }
        
    }

    public void CancelRetreat()
    {
        if (retreatPopup != null)
        {
            retreatPopup.SetActive(false);
        }
    }

    public void ConfirmRetreat()
    {
        Debug.Log("Player interacted with retreat statue. Saving progress...");

        PlayerController controller = FindFirstObjectByType<PlayerController>();
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();

        // Heal player to max before capturing save state
        if (stats != null)
        {
            stats.currentHealth = stats.maxHealth;
        }

        // Reset dungeon progression counter
        Mappicker.completedLevels = -1;

        if (controller != null && stats != null && inventory != null)
        {
            SaveLoadManager.SaveGame(controller, stats, inventory);
        }
        else
        {
            Debug.LogError("Retreat save aborted: missing player references.");
        }

        SceneManager.LoadScene("Village");
    }
}