using UnityEngine;

// This attribute allows you to create new class files from the Right-Click menu
[CreateAssetMenu(fileName = "NewClassData", menuName = "Roman Vanguard/Class Data")]
public class CharacterClassData : ScriptableObject
{
    public static CharacterClassData SelectedClass;

    [Header("Visuals")]
    public string className;
    public Color classPreviewColor = Color.white; // Temporary, until you have sprites

    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 6f;
    public float dashForce = 12f;

    [Header("Stats")]
    public int maxHealth = 100;

    [Header("Combat")]
    public bool isRanged = false;
    public int damage = 20;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public float criticalChance = 0.1f; 
}