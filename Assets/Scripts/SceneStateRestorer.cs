using UnityEngine;
using UnityEngine.SceneManagement;

// This attribute forces Unity to execute this script before any standard Start() in the project
[DefaultExecutionOrder(-100)]
public class SceneStateRestorer : MonoBehaviour
{
    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu" || sceneName == "ClassSelection") return;

        Debug.Log($"Scene '{sceneName}' initialized. Injecting save file data...");

        PlayerController controller = GetComponent<PlayerController>();
        PlayerStats stats = GetComponent<PlayerStats>();
        PlayerInventory inventory = GetComponent<PlayerInventory>();

        if (controller != null && stats != null && inventory != null)
        {
            // Block Saving while we are restoring the state to prevent overwriting the loaded data
            SaveLoadManager.isRestoringState = true;

            // Load the saved game data into the player components
            SaveLoadManager.LoadGame(controller, stats, inventory);
            stats.ApplyEquipmentBonuses(inventory.equippedWeapon, inventory.equippedArmor);

            if (controller.classData != null)
            {
                CharacterClassData.SelectedClass = controller.classData;
            }

            // Notify the inventory that it has changed so that any UI or other systems can update accordingly
            inventory.NotifyChanged();

            // Unblock Saving after the state has been restored
            SaveLoadManager.isRestoringState = false;
        }
    }
}