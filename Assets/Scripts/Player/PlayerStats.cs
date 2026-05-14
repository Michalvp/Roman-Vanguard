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

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        skillPoints++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);

        // Increase stats on level up
        maxHealth += 10;
        currentHealth = maxHealth;
        bonusDamage += 2;

        Debug.Log($"Level Up! Now at level {currentLevel}");
    }

    public void AddDenarii(int amount) => denarii += amount;

    public bool TrySpendDenarii(int amount)
    {
        if (denarii >= amount)
        {
            denarii -= amount;
            return true;
        }
        return false;
    }
}