using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("Top Info")]
    public TextMeshProUGUI levelAndClassText;

    [Header("Bars")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    [Header("Stats & Currency")]
    public TextMeshProUGUI skillPointsText;
    public TextMeshProUGUI denariiText;

    [Header("Skill Special Section")]
    public GameObject skillRootObject;
    public TextMeshProUGUI skillNameText;
    public Slider skillCooldownSlider;

    private PlayerController player;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerController>();
        UpdateStaticInfo();
    }

    void Update()
    {
        if (PlayerStats.Instance == null) return;

        UpdateDynamicUI();
    }

    private void UpdateStaticInfo()
    {
        // Handle "Lvl: 0" if no class is selected yet
        string className = (CharacterClassData.SelectedClass != null) ? CharacterClassData.SelectedClass.className : "";
        int level = (PlayerStats.Instance != null) ? PlayerStats.Instance.currentLevel : 0;

        if (string.IsNullOrEmpty(className))
            levelAndClassText.text = $"Lvl: 0";
        else
            levelAndClassText.text = $"Lvl: {level} {className}";

        // Set skill name if available
        if (CharacterClassData.SelectedClass != null)
        {
            skillNameText.text = CharacterClassData.SelectedClass.className == "Archer" ? "MultiShot" :
                                 CharacterClassData.SelectedClass.className == "Legionary" ? "Vanguard Strike" : "Spartan Rage";
        }
    }

    private void UpdateDynamicUI()
    {
        // 1. Level & Class (Update in case of level up)
        string className = (CharacterClassData.SelectedClass != null) ? CharacterClassData.SelectedClass.className : "";
        levelAndClassText.text = $"Lvl: {PlayerStats.Instance.currentLevel} {className}";

        // 2. Health Bar
        healthSlider.maxValue = PlayerStats.Instance.maxHealth;
        healthSlider.value = PlayerStats.Instance.currentHealth;
        healthText.text = $"{PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth}";

        // 3. XP Bar
        xpSlider.maxValue = PlayerStats.Instance.xpToNextLevel;
        xpSlider.value = PlayerStats.Instance.currentXP;
        xpText.text = $"{PlayerStats.Instance.currentXP} / {PlayerStats.Instance.xpToNextLevel} XP";

        // 4. Skill Points & Denarii
        skillPointsText.text = "Skill Points: " + PlayerStats.Instance.skillPoints;
        denariiText.text = "Denarii: " + PlayerStats.Instance.denarii;

        // 5. Special Skill Visibility & Cooldown
        if (PlayerStats.Instance.hasSpecialAbility)
        {
            skillRootObject.SetActive(true);
            UpdateCooldownSlider();
        }
        else
        {
            skillRootObject.SetActive(false);
        }
    }

    private void UpdateCooldownSlider()
    {
        if (player == null) return;

        float timeLeft = player.GetNextSpecialAttackTime() - Time.time;

        if (timeLeft > 0)
        {
            float maxCD = player.GetLastSpecialCooldownDuration();
            float progress = (maxCD - timeLeft) / maxCD;
            skillCooldownSlider.value = Mathf.Clamp01(progress);
        }
        else
        {
            skillCooldownSlider.value = 1f;
        }
    }
}