using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Roman Vanguard/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public int cost = 1; // Skill points required

    // Skill effects
    public int healthBonus;
    public int damageBonus;
    public float attackRangeBonus;
    public bool unlocksSpecialAction;

    [Header("Requirement")]
    public SkillData requiredSkill; // Optional: prerequisite skill
}