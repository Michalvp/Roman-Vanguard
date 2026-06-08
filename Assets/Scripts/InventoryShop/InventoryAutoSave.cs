using UnityEngine;
using UnityEngine.SceneManagement;

namespace InventoryShop
{
    // Saves inventory when a scene is unloaded and loads it when a new scene is loaded.
    // Attach this script to the same GameObject as InventorySingleton (the persistent holder).
    public class InventoryAutoSave : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (PlayerInventory.Instance != null)
            {
                SaveLoadManager.LoadInventory(PlayerInventory.Instance);
                Debug.Log($"Inventory reloaded after scene '{scene.name}'.");
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (PlayerInventory.Instance != null)
            {
                SaveLoadManager.SaveInventory(PlayerInventory.Instance);
                Debug.Log($"Inventory saved before unloading scene '{scene.name}'.");
            }
        }
    }
}
