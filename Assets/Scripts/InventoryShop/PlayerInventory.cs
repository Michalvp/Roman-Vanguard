using System.IO;
using UnityEngine;

using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Inventory")]
    [Min(1)] public int capacity = 24;
    public List<InventorySlot> slots = new List<InventorySlot>();

    [Header("Equipped Items")]
    public ShopItemData equippedWeapon;
    public ShopItemData equippedArmor;

    public event Action OnInventoryChanged;

    private PlayerStats stats;
    private PlayerController controller;

    private void Awake()
    {
        // Ensure singleton pattern and persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad removed to keep player non-persistent
        // Load saved inventory if exists
        SaveLoadManager.LoadInventory(this);
        EnsureSlotCount();
    }

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        RecalculateEquipmentStats();
        NotifyChanged();
    }

    public void EnsureSlotCount()
    {
        while (slots.Count < capacity)
            slots.Add(new InventorySlot());

        while (slots.Count > capacity)
            slots.RemoveAt(slots.Count - 1);
    }

    public bool AddItem(ShopItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        EnsureSlotCount();

        if (item.stackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item && slot.quantity < item.maxStack)
                {
                    int space = item.maxStack - slot.quantity;
                    int added = Mathf.Min(space, amount);
                    slot.quantity += added;
                    amount -= added;

                    if (amount <= 0)
                    {
                        NotifyChanged();
                        return true;
                    }
                }
            }
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
            {
                int added = item.stackable ? Mathf.Min(item.maxStack, amount) : 1;
                slot.item = item;
                slot.quantity = added;
                amount -= added;

                if (amount <= 0)
                {
                    NotifyChanged();
                    return true;
                }
            }
        }

        Debug.LogWarning("Inventory is full. Could not add all items.");
        NotifyChanged();
        return false;
    }

    public bool HasFreeSpaceFor(ShopItemData item)
    {
        if (item == null)
            return false;

        EnsureSlotCount();

        if (item.stackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item && slot.quantity < item.maxStack)
                    return true;
            }
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
                return true;
        }

        return false;
    }

    public void UseSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return;

        InventorySlot slot = slots[slotIndex];

        if (slot.IsEmpty)
            return;

        ShopItemData item = slot.item;

        if (!item.CanUseWithClass(GetCurrentClass()))
        {
            Debug.Log($"{item.itemName} cannot be used by this class.");
            return;
        }

        if (item.itemType == RomanItemType.Weapon || item.itemType == RomanItemType.Armor)
        {
            Equip(item);
            return;
        }

        if (item.itemType == RomanItemType.Consumable)
        {
            if (stats == null)
                stats = GetComponent<PlayerStats>();

            if (stats != null && item.healAmount > 0)
                stats.Heal(item.healAmount);

            RemoveOne(slotIndex);
            return;
        }

        Debug.Log($"{item.itemName} is a material. It cannot be used yet, but it can be saved for crafting later.");
    }

    public void Equip(ShopItemData item)
    {
        if (item == null || !item.IsEquipment)
            return;

        if (!item.CanUseWithClass(GetCurrentClass()))
        {
            Debug.Log($"{item.itemName} is not allowed for this class.");
            return;
        }

        if (item.itemType == RomanItemType.Weapon)
            equippedWeapon = item;
        else if (item.itemType == RomanItemType.Armor)
            equippedArmor = item;

        RecalculateEquipmentStats();
        NotifyChanged();

        Debug.Log($"Equipped {item.itemName}");
    }

    public void RemoveOne(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return;

        InventorySlot slot = slots[slotIndex];

        if (slot.IsEmpty)
            return;

        slot.quantity--;

        if (slot.quantity <= 0)
            slot.Clear();

        NotifyChanged();
    }

    private CharacterClassData GetCurrentClass()
    {
        if (controller == null)
            controller = GetComponent<PlayerController>();

        if (controller != null && controller.classData != null)
            return controller.classData;

        return CharacterClassData.SelectedClass;
    }

    private void RecalculateEquipmentStats()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (stats != null)
            stats.ApplyEquipmentBonuses(equippedWeapon, equippedArmor);
    }

    public void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
        // Persist inventory state after any change
        SaveLoadManager.SaveInventory(this);
    }
}
