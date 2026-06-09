using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This manager provides simple JSON save/load functionality for the player inventory.
// It stores each non-empty inventory slot as the item name (must match a ShopItemData asset) and quantity.
public static class SaveLoadManager
{
    private const string SaveFileName = "inventory_save.json";

    [Serializable]
    private class SlotData
    {
        public string itemName;
        public int quantity;
    }

    [Serializable]
    private class InventorySaveData
    {
        public List<SlotData> slots = new List<SlotData>();
        public string equippedWeaponName;
        public string equippedArmorName;
    }

    // Saves the current inventory to a JSON file.
    public static void SaveInventory(PlayerInventory inventory)
    {
        if (inventory == null) return;
        var data = new InventorySaveData();
        foreach (var slot in inventory.slots)
        {
            if (slot.IsEmpty) continue; // skip empty slots
            data.slots.Add(new SlotData { itemName = slot.item.itemName, quantity = slot.quantity });
        }
        // Save equipped items if present
        data.equippedWeaponName = inventory.equippedWeapon != null ? inventory.equippedWeapon.itemName : null;
        data.equippedArmorName = inventory.equippedArmor != null ? inventory.equippedArmor.itemName : null;
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"Inventory saved to {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save inventory: {ex.Message}");
        }
    }

    // Loads the inventory from a JSON file. Existing slots are cleared and repopulated.
    public static void LoadInventory(PlayerInventory inventory)
    {
        if (inventory == null) return;
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(path))
        {
            Debug.Log("No saved inventory file found.");
            return;
        }
        try
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<InventorySaveData>(json);
            if (data == null) return;
            if (data == null) return;
            // Ensure slot count matches capacity
            inventory.EnsureSlotCount();
            // Clear existing slots
            foreach (var slot in inventory.slots)
                slot.Clear();
            // Load all ShopItemData assets from Resources for lookup
            var allItems = Resources.LoadAll<ShopItemData>("");
            var lookup = new Dictionary<string, ShopItemData>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in allItems)
                if (!string.IsNullOrEmpty(item.itemName))
                    lookup[item.itemName] = item;

            // Load equipped items
            if (!string.IsNullOrEmpty(data.equippedWeaponName) && lookup.TryGetValue(data.equippedWeaponName, out var weaponItem))
                inventory.equippedWeapon = weaponItem;
            if (!string.IsNullOrEmpty(data.equippedArmorName) && lookup.TryGetValue(data.equippedArmorName, out var armorItem))
                inventory.equippedArmor = armorItem;
            int index = 0;
            foreach (var savedSlot in data.slots)
            {
                if (index >= inventory.slots.Count) break;
                if (lookup.TryGetValue(savedSlot.itemName, out var itemData))
                {
                    var slot = inventory.slots[index];
                    slot.item = itemData;
                    slot.quantity = savedSlot.quantity;
                }
                else
                {
                    Debug.LogWarning($"Item '{savedSlot.itemName}' not found in Resources. Skipping slot.");
                }
                index++;
            }
            inventory.NotifyChanged();
            Debug.Log($"Inventory loaded from {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load inventory: {ex.Message}");
        }
    }
    public static void DeleteSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Save file deleted at {path}");
        }
        else
        {
            Debug.LogWarning($"No save file found at {path} to delete.");
        }
    }
}

