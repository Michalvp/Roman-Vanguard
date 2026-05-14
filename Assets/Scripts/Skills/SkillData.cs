using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Roman Vanguard/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public int cost = 1;

    [Header("Bonuses")]
    public int healthBonus;
    public int damageBonus;
    public float attackRangeBonus;
    public float speedBonus;
    public float dashBonus;
    public float attackSpeedBonus;
    public float criticalChanceBonus;

    [Header("Special Ability")]
    public bool unlocksSpecialAbility;

    [Header("Requirements")]
    public SkillData requiredSkill; // The skill that must be purchased first

    [Header("State")]
    public bool isUnlocked = false; // Persistent only during one session
}