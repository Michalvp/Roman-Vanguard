using UnityEngine;

public enum RomanItemType
{
    Weapon,
    Armor,
    Consumable,
    Material
}

public enum RomanClassRequirement
{
    Any,
    Legionary,
    Gladiator,
    Archer
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Roman Vanguard/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Basic Item Info")]
    public string itemName;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    public RomanItemType itemType = RomanItemType.Material;
    public RomanClassRequirement classRequirement = RomanClassRequirement.Any;

    [Header("Shop")]
    [Min(0)] public int priceDenarii = 1;
    public bool canBeSoldInShop = true;

    [Header("Inventory")]
    public bool stackable = true;
    [Min(1)] public int maxStack = 20;

    [Header("Equipment Stats")]
    public int damageBonus;
    public int defenseBonus;
    public int maxHealthBonus;
    public float moveSpeedBonus;
    public float attackRateBonus;
    public float attackRangeBonus;
    public float criticalChanceBonus;

    [Header("Consumable Stats")]
    public int healAmount;

    public bool IsEquipment => itemType == RomanItemType.Weapon || itemType == RomanItemType.Armor;
    public bool IsConsumable => itemType == RomanItemType.Consumable;

    public bool CanUseWithClass(CharacterClassData currentClass)
    {
        if (classRequirement == RomanClassRequirement.Any)
            return true;

        if (currentClass == null)
            return false;

        return currentClass.className == classRequirement.ToString();
    }

    public string GetClassText()
    {
        return classRequirement == RomanClassRequirement.Any ? "Any class" : classRequirement.ToString();
    }

    public string GetPriceText()
    {
        return RomanCurrency.FormatDenarii(priceDenarii);
    }

    public string GetStatsText()
    {
        string text = "";

        if (damageBonus != 0) text += $"+{damageBonus} Damage\n";
        if (defenseBonus != 0) text += $"+{defenseBonus} Defense\n";
        if (maxHealthBonus != 0) text += $"+{maxHealthBonus} Max HP\n";
        if (healAmount != 0) text += $"Restores {healAmount} HP\n";
        if (moveSpeedBonus != 0) text += $"{moveSpeedBonus:+0.##;-0.##} Move Speed\n";
        if (attackRateBonus != 0) text += $"{attackRateBonus:+0.##;-0.##} Attack Rate\n";
        if (attackRangeBonus != 0) text += $"{attackRangeBonus:+0.##;-0.##} Attack Range\n";
        if (criticalChanceBonus != 0) text += $"{criticalChanceBonus * 100f:+0;-0}% Critical Chance\n";

        if (string.IsNullOrWhiteSpace(text))
            text = "Utility material.";

        return text.TrimEnd();
    }
}
