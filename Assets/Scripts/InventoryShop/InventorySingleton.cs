using UnityEngine;
using UnityEngine.SceneManagement;

// This component should be placed on an empty GameObject in the initial scene.
// It creates a persistent holder for the PlayerInventory, ensuring the player GameObject itself is not persisted.
public class InventorySingleton : MonoBehaviour
{
    private const string HolderName = "InventoryHolder";

    void Awake()
    {
        // If another holder already exists, destroy this duplicate.
        var existing = GameObject.Find(HolderName);
        if (existing != null && existing != this.gameObject)
        {
            Destroy(gameObject);
            return;
        }

        // Designate this GameObject as the holder and keep it across scenes.
        gameObject.name = HolderName;
        DontDestroyOnLoad(gameObject);

        // Ensure the PlayerInventory component is present on the holder.
        var inventory = GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = gameObject.AddComponent<PlayerInventory>();
        }

        // Load saved inventory data now (useful on first scene load).
        SaveLoadManager.LoadInventory(inventory);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Reload inventory after each scene load to ensure UI reflects saved data.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var inventory = GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            SaveLoadManager.LoadInventory(inventory);
        }
    }
}
