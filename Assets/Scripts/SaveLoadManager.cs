using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static bool isRestoringState = false;
    private static string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, "save_slot_" + slotIndex + ".json");
    }

    public static void SaveGame(PlayerController playerController, PlayerStats playerStats, PlayerInventory playerInventory)
    {
        if (isRestoringState)
        {
            Debug.Log("Save ignored: Restoring game state.");
            return;
        }

        int slot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);
        if (slot <= 0)
        {
            Debug.LogWarning("No active save slot selected. Aborting save.");
            return;
        }

        GameSaveData data = new GameSaveData();
        data.lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // Serialize PlayerController state
        if (playerController.classData != null)
        {
            data.characterClassName = playerController.classData.className;
            data.classDataName = playerController.classData.name;
        }
        data.moveSpeed = playerController.moveSpeed;
        data.jumpForce = playerController.jumpForce;
        data.dashForce = playerController.dashForce;
        data.dashDuration = playerController.dashDuration;
        data.dashCooldown = playerController.dashCooldown;
        data.attackRate = playerController.attackRate;
        data.criticalChance = playerController.criticalChance;
        data.attackRange = playerController.attackRange;
        data.attackDamage = playerController.attackDamage;

        // Serialize PlayerStats state
        data.denarii = playerStats.denarii;
        data.currentLevel = playerStats.currentLevel;
        data.currentXP = playerStats.currentXP;
        data.xpToNextLevel = playerStats.xpToNextLevel;
        data.skillPoints = playerStats.skillPoints;
        data.baseMaxHealth = playerStats.baseMaxHealth;
        data.levelBonusDamage = playerStats.levelBonusDamage;
        data.levelBonusHealth = playerStats.levelBonusHealth;

        // Serialize Inventory slots (Accessing the list/collection from PlayerInventory)
        // Adjust 'inventorySlots' field name based on your exact implementation inside PlayerInventory
        if (playerInventory != null && playerInventory.slots!= null)
        {
            foreach (var slotItem in playerInventory.slots)
            {
                if (slotItem != null && slotItem.item != null)
                {
                    InventorySlotSaveData slotData = new InventorySlotSaveData();
                    slotData.itemAssetName = slotItem.item.name;
                    slotData.quantity = slotItem.quantity;
                    data.inventorySlots.Add(slotData);
                }
            }

            data.equippedWeaponName = playerInventory.equippedWeapon != null ? playerInventory.equippedWeapon.name : "";
            data.equippedArmorName = playerInventory.equippedArmor != null ? playerInventory.equippedArmor.name : "";
        }

        // Serialize Skills from SkillTreeManager if instance exists
        if (playerStats.unlockedSkills != null)
        {
            foreach (var skill in playerStats.unlockedSkills)
            {
                if (skill != null)
                {
                    data.unlockedSkillNames.Add(skill.name);
                }
            }
        }

        // Write to file system
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slot), json);
        Debug.Log("Game progress saved to slot " + slot);
    }

    public static void LoadGame(PlayerController playerController, PlayerStats playerStats, PlayerInventory playerInventory)
    {
        int slot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);
        string path = GetSaveFilePath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file for slot " + slot + " does not exist.");
            return;
        }

        playerInventory.equippedWeapon = null;
        playerInventory.equippedArmor = null;

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        // Deserialize PlayerController values
        playerController.moveSpeed = data.moveSpeed;
        playerController.jumpForce = data.jumpForce;
        playerController.dashForce = data.dashForce;
        playerController.dashDuration = data.dashDuration;
        playerController.dashCooldown = data.dashCooldown;
        playerController.attackRate = data.attackRate;
        playerController.criticalChance = data.criticalChance;
        playerController.attackRange = data.attackRange;
        playerController.attackDamage = data.attackDamage;

        // Dynamic restoration of the Class Configuration ScriptableObject
        if (!string.IsNullOrEmpty(data.classDataName))
        {
            playerController.classData = Resources.Load<CharacterClassData>("Classes/" + data.classDataName);
        }

        // Deserialize PlayerStats values
        playerStats.denarii = data.denarii;
        playerStats.currentLevel = data.currentLevel;
        playerStats.currentXP = data.currentXP;
        playerStats.xpToNextLevel = data.xpToNextLevel;
        playerStats.skillPoints = data.skillPoints;
        playerStats.baseMaxHealth = data.baseMaxHealth;
        playerStats.levelBonusDamage = data.levelBonusDamage;
        playerStats.levelBonusHealth = data.levelBonusHealth;

        // Deserialize and reconstruct the Inventory system
        if (playerInventory != null)
        {
            // Clear current inventory items first to prevent duplication
            playerInventory.slots.Clear();

            foreach (var slotData in data.inventorySlots)
            {
                ShopItemData originalItem = Resources.Load<ShopItemData>("ShopItems/" + slotData.itemAssetName);
                if (originalItem != null)
                {
                    // Use your existing inventory logic to append the item back into the list
                    playerInventory.AddItem(originalItem, slotData.quantity);
                    Debug.Log("Restored item: " + originalItem.name + " with quantity: " + slotData.quantity);
                }
            }

            // Restore equipped items
            playerInventory.equippedWeapon = !string.IsNullOrEmpty(data.equippedWeaponName) ? Resources.Load<ShopItemData>("ShopItems/" + data.equippedWeaponName) : null;
            playerInventory.equippedArmor = !string.IsNullOrEmpty(data.equippedArmorName) ? Resources.Load<ShopItemData>("ShopItems/" + data.equippedArmorName) : null;
        }

        // Deserialize and reconstruct Skill Tree progression
        if (playerStats != null && data.unlockedSkillNames != null)
        {
            playerStats.unlockedSkills.Clear();
            foreach (string skillName in data.unlockedSkillNames)
            {
                SkillData[] allSkills = Resources.LoadAll<SkillData>("Skills");
                if (allSkills == null || allSkills.Length == 0)
                {
                    Debug.LogWarning("No SkillData assets found in Resources/Skills.");
                    continue;
                }
                else
                {
                    Debug.Log("Found " + allSkills.Length + " SkillData assets in Resources/Skills.");
                }
                SkillData skill = System.Array.Find(allSkills, s => s.name == skillName);
                Debug.Log("Restoring skill: " + skillName + " - Found: " + (skill != null));
                if (skill != null)
                {
                    playerStats.unlockedSkills.Add(skill);
                    // Apply skill effects to the player stats
                    playerStats.ApplySkillEffects(skill);
                    skill.isUnlocked = true; // Mark the skill as unlocked
                }
            }
        }

        Debug.Log("Game progress loaded from slot " + slot);
    }
}