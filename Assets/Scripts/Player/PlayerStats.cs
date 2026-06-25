using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Currency")]
    public int denarii = 0;

    [Header("Leveling System")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int skillPoints = 0;
    public int levelBonusDamage = 0;
    public int levelBonusHealth = 0;

    [Header("Health")]
    public int baseMaxHealth = 100;
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Equipment Bonuses")]
    public int bonusDamage = 0;
    public int bonusDefense = 0;
    public int bonusMaxHealth = 0;
    public float bonusMoveSpeed = 0f;
    public float bonusAttackRate = 0f;
    public float bonusAttackRange = 0f;
    public float bonusCriticalChance = 0f;

    [Header("Skill Bonuses")]
    public int skillHealthBonus = 0;
    public int skillDamageBonus = 0;
    public float skillattackRangeBonus = 0f;
    public float skillspeedBonus = 0f;
    public float skilldashBonus = 0f;
    public float skillattackSpeedBonus = 0f;
    public float skillcriticalChanceBonus = 0f;

    [Header("Save Data")]
    public List<SkillData> unlockedSkills = new List<SkillData>();

    [Header("Class Special Abilities")]
    public bool hasSpecialAbility = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (maxHealth <= 0)
            maxHealth = baseMaxHealth;

        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }

    public void SetClassBaseStats(CharacterClassData data)
    {
        if (data != null)
            baseMaxHealth = data.maxHealth;

        RecalculateMaxHealth(healToFull: true);
    }

    public void ApplyEquipmentBonuses(ShopItemData weapon, ShopItemData armor)
    {
        bonusDamage = 0;
        bonusDefense = 0;
        bonusMaxHealth = 0;
        bonusMoveSpeed = 0f;
        bonusAttackRate = 0f;
        bonusAttackRange = 0f;
        bonusCriticalChance = 0f;

        AddItemBonuses(weapon);
        AddItemBonuses(armor);

        RecalculateMaxHealth(healToFull: false);
    }

    private void AddItemBonuses(ShopItemData item)
    {
        if (item == null)
            return;

        bonusDamage += item.damageBonus;
        bonusDefense += item.defenseBonus;
        bonusMaxHealth += item.maxHealthBonus;
        bonusMoveSpeed += item.moveSpeedBonus;
        bonusAttackRate += item.attackRateBonus;
        bonusAttackRange += item.attackRangeBonus;
        bonusCriticalChance += item.criticalChanceBonus;
    }

    public void ApplySkillEffects(SkillData skill)
    {
        if (skill == null) return;
        skillHealthBonus += skill.healthBonus;
        skillDamageBonus += skill.damageBonus;
        skillattackRangeBonus += skill.attackRangeBonus;
        skillspeedBonus += skill.speedBonus;
        skilldashBonus += skill.dashBonus;
        skillattackSpeedBonus += skill.attackSpeedBonus;
        skillcriticalChanceBonus += skill.criticalChanceBonus;

        RecalculateMaxHealth(true);

        if (skill.unlocksSpecialAbility)
        {
            hasSpecialAbility = true;
        }
    }

    public void RecalculateMaxHealth(bool healToFull)
    {
        int oldMax = maxHealth;
        maxHealth = Mathf.Max(1, baseMaxHealth + bonusMaxHealth + skillHealthBonus + levelBonusHealth);

        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            int difference = maxHealth - oldMax;

            if (difference > 0)
                currentHealth += difference;

            currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Healed {amount}. Current HP: {currentHealth}/{maxHealth}");
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        skillPoints++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);

        levelBonusHealth += 10;
        levelBonusDamage += 2;
        RecalculateMaxHealth(healToFull: true);

        Debug.Log($"Level Up! Now at level {currentLevel}");
    }

    public void AddDenarii(int amount)
    {
        denarii += amount;
    }

    public bool TrySpendDenarii(int amount)
    {
        if (amount < 0)
            return false;

        if (denarii >= amount)
        {
            denarii -= amount;
            return true;
        }

        return false;
    }
}
