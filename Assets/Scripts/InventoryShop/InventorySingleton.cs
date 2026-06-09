using UnityEngine;
using UnityEngine.SceneManagement;

public class InventorySingleton : MonoBehaviour
{
    private const string HolderName = "InventoryHolder";

    void Awake()
    {
        var existing = GameObject.Find(HolderName);
        if (existing != null && existing != this.gameObject)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.name = HolderName;
        DontDestroyOnLoad(gameObject);

        var inventory = GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = gameObject.AddComponent<PlayerInventory>();
        }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var inventory = GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            SaveLoadManager.LoadInventory(inventory);
        }
    }
}
