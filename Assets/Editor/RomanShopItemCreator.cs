#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RomanShopItemCreator
{
    private const string FolderPath = "Assets/Data/ShopItems";

    [MenuItem("Tools/Roman Vanguard/Create Default Shop Items")]
    public static void CreateDefaultShopItems()
    {
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        List<ShopItemData> createdItems = new List<ShopItemData>();

        createdItems.Add(CreateItem("Gladius Hispaniensis", RomanItemType.Weapon, RomanClassRequirement.Legionary, 35,
            "Balanced short sword for close Roman infantry fighting.", damage: 8, attackRate: 0.05f));

        createdItems.Add(CreateItem("Pugio", RomanItemType.Weapon, RomanClassRequirement.Any, 12,
            "Small side dagger. Light, cheap, and usable by any class.", damage: 3, crit: 0.05f, attackRate: 0.10f));

        createdItems.Add(CreateItem("Pilum", RomanItemType.Weapon, RomanClassRequirement.Legionary, 24,
            "Heavy Roman javelin. In this system it works as a strong legionary weapon.", damage: 12, attackRate: -0.10f));

        createdItems.Add(CreateItem("Spatha", RomanItemType.Weapon, RomanClassRequirement.Legionary, 55,
            "Longer sword with better reach and high damage.", damage: 14, attackRange: 0.15f, attackRate: -0.05f));

        createdItems.Add(CreateItem("Sica", RomanItemType.Weapon, RomanClassRequirement.Gladiator, 32,
            "Curved gladiator blade. Good for critical strikes.", damage: 9, crit: 0.08f));

        createdItems.Add(CreateItem("Rudis Training Sword", RomanItemType.Weapon, RomanClassRequirement.Gladiator, 10,
            "Wooden training sword. Weak but fast.", damage: 2, attackRate: 0.20f));

        createdItems.Add(CreateItem("Retiarius Trident", RomanItemType.Weapon, RomanClassRequirement.Gladiator, 48,
            "Long gladiator trident. Strong reach and damage.", damage: 12, attackRange: 0.25f));

        createdItems.Add(CreateItem("Arcus Romanus", RomanItemType.Weapon, RomanClassRequirement.Archer, 38,
            "Standard Roman bow for archers.", damage: 7, attackRate: 0.12f));

        createdItems.Add(CreateItem("Eastern Composite Bow", RomanItemType.Weapon, RomanClassRequirement.Archer, 60,
            "Expensive composite bow with high power and critical chance.", damage: 11, attackRate: 0.08f, crit: 0.05f));

        createdItems.Add(CreateItem("Funda Sling", RomanItemType.Weapon, RomanClassRequirement.Archer, 18,
            "Simple sling. Cheap, fast, and light.", damage: 4, attackRate: 0.20f));

        createdItems.Add(CreateItem("Scutum", RomanItemType.Armor, RomanClassRequirement.Legionary, 40,
            "Large Roman shield. Great protection but slightly heavy.", defense: 12, health: 8, moveSpeed: -0.15f));

        createdItems.Add(CreateItem("Lorica Hamata", RomanItemType.Armor, RomanClassRequirement.Legionary, 45,
            "Chainmail armor. Reliable protection with moderate weight.", defense: 9, health: 10, moveSpeed: -0.05f));

        createdItems.Add(CreateItem("Lorica Segmentata", RomanItemType.Armor, RomanClassRequirement.Legionary, 75,
            "Segmented Roman armor. Excellent defense but heavy.", defense: 16, health: 20, moveSpeed: -0.15f));

        createdItems.Add(CreateItem("Manica", RomanItemType.Armor, RomanClassRequirement.Gladiator, 22,
            "Gladiator arm guard. Light armor that preserves speed.", defense: 7, health: 5));

        createdItems.Add(CreateItem("Galea Gladiatoria", RomanItemType.Armor, RomanClassRequirement.Gladiator, 28,
            "Gladiator helmet. Good defensive boost.", defense: 8, health: 8));

        createdItems.Add(CreateItem("Padded Leather Tunic", RomanItemType.Armor, RomanClassRequirement.Archer, 20,
            "Light archer armor. Lower defense but better movement.", defense: 5, health: 6, moveSpeed: 0.10f));

        createdItems.Add(CreateConsumable("Panis", 5, 15, "Bread ration. Restores a small amount of health."));
        createdItems.Add(CreateConsumable("Puls", 7, 22, "Porridge meal. Restores a steady amount of health."));
        createdItems.Add(CreateConsumable("Moretum", 6, 18, "Herbed cheese spread. Restores health."));
        createdItems.Add(CreateConsumable("Garum Ration", 4, 14, "Fish sauce ration. Salty but useful."));
        createdItems.Add(CreateConsumable("Posca", 6, 20, "Common soldier drink. Restores health."));
        createdItems.Add(CreateConsumable("Mulsum", 10, 28, "Sweetened wine. Stronger healing drink."));

        createdItems.Add(CreateMaterial("Ferrum Ingot", 15, "Iron ingot for future crafting or upgrades."));
        createdItems.Add(CreateMaterial("Lignum", 8, "Wood bundle for future crafting."));
        createdItems.Add(CreateMaterial("Corium", 10, "Leather material for future crafting."));
        createdItems.Add(CreateMaterial("Linum Cloth", 9, "Linen cloth for future crafting."));
        createdItems.Add(CreateMaterial("Cos Sharpening Stone", 12, "Sharpening stone for future weapon upgrades."));
        createdItems.Add(CreateMaterial("Fascia Bandage", 7, "Bandage material for future medicine crafting."));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created or updated {createdItems.Count} shop item assets in {FolderPath}. Assign icons manually in the Inspector.");
    }

    private static ShopItemData CreateConsumable(string name, int price, int heal, string description)
    {
        return CreateItem(name, RomanItemType.Consumable, RomanClassRequirement.Any, price, description, heal: heal);
    }

    private static ShopItemData CreateMaterial(string name, int price, string description)
    {
        return CreateItem(name, RomanItemType.Material, RomanClassRequirement.Any, price, description);
    }

    private static ShopItemData CreateItem(
        string name,
        RomanItemType type,
        RomanClassRequirement classRequirement,
        int price,
        string description,
        int damage = 0,
        int defense = 0,
        int health = 0,
        float moveSpeed = 0f,
        float attackRate = 0f,
        float attackRange = 0f,
        float crit = 0f,
        int heal = 0)
    {
        string safeName = name.Replace(" ", "_").Replace("/", "_");
        string assetPath = $"{FolderPath}/SO_Item_{safeName}.asset";

        ShopItemData item = AssetDatabase.LoadAssetAtPath<ShopItemData>(assetPath);

        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ShopItemData>();
            AssetDatabase.CreateAsset(item, assetPath);
        }

        item.itemName = name;
        item.itemType = type;
        item.classRequirement = classRequirement;
        item.priceDenarii = price;
        item.description = description;

        item.damageBonus = damage;
        item.defenseBonus = defense;
        item.maxHealthBonus = health;
        item.moveSpeedBonus = moveSpeed;
        item.attackRateBonus = attackRate;
        item.attackRangeBonus = attackRange;
        item.criticalChanceBonus = crit;
        item.healAmount = heal;

        item.stackable = type == RomanItemType.Consumable || type == RomanItemType.Material;
        item.maxStack = item.stackable ? 20 : 1;

        EditorUtility.SetDirty(item);
        return item;
    }
}
#endif
