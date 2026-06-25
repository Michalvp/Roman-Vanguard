using System;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlotSaveData
{
    public string itemAssetName;
    public int quantity;
}

[System.Serializable]
public class GameSaveData
{
    public string lastPlayedDate;
    public string characterClassName;

    // PlayerController configuration
    public string classDataName;
    public float moveSpeed;
    public float jumpForce;
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public float attackRate;
    public float criticalChance;
    public float attackRange;
    public int attackDamage;

    // PlayerStats configuration 
    public int denarii;
    public int currentLevel;
    public int currentXP;
    public int xpToNextLevel;
    public int skillPoints;
    public int baseMaxHealth;
    public int levelBonusDamage;
    public int levelBonusHealth;

    // Inventory lists
    public List<InventorySlotSaveData> inventorySlots = new List<InventorySlotSaveData>();

    // Equipped items metadata (critical addition for proper state recovery)
    public string equippedWeaponName;
    public string equippedArmorName;

    // Skill tree progression
    public List<string> unlockedSkillNames = new List<string>();
}
