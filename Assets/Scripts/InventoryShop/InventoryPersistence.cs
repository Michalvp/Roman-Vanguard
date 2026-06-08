using UnityEngine;
using UnityEngine.SceneManagement;

namespace InventoryShop
{
    // Ensures the inventory is (re)loaded whenever a new scene finishes loading.
    // Attach this to any GameObject in the initial scene (e.g., the same holder used by InventorySingleton).
    public class InventoryPersistence : MonoBehaviour
    {
        private void Awake()
        {
            // Subscribe to the sceneLoaded event.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks.
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Ensure the singleton instance exists before loading.
            if (PlayerInventory.Instance != null)
            {
                SaveLoadManager.LoadInventory(PlayerInventory.Instance);
                Debug.Log($"Inventory reloaded after scene '{scene.name}' loaded.");
            }
            else
            {
                Debug.LogWarning("PlayerInventory instance not found during scene load. Inventory not reloaded.");
            }
        }
    }
}
