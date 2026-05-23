using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;


    //Data found here can be later used for displaying in the UI

    [Header("Currency")]
    public int denarii = 0;

    [Header("Leveling System")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int skillPoints = 0;

    [Header("Calculated RPG Stats")]
    public int maxHealth;
    public int currentHealth;
    public int bonusDamage = 0;

    [Header("Class Special Abilities")]
    public bool hasSpecialAbility = false;



    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddXP(int amount)
    {
        Instance.currentXP += amount;
        if (Instance.currentXP >= Instance.xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        Instance.currentLevel++;
        Instance.skillPoints++;
        Instance.currentXP -= Instance.xpToNextLevel;
        Instance.xpToNextLevel = Mathf.RoundToInt(Instance.xpToNextLevel * 1.3f);

        // Increase stats on level up
        Instance.maxHealth += 10;
        Instance.currentHealth = Instance.maxHealth;
        Instance.bonusDamage += 2;

        Debug.Log($"Level Up! Now at level {Instance.currentLevel}");
    }

    public void AddDenarii(int amount) => Instance.denarii += amount;

    public bool TrySpendDenarii(int amount)
    {
        if (Instance.denarii >= amount)
        {
            Instance.denarii -= amount;
            return true;
        }
        return false;
    }
}