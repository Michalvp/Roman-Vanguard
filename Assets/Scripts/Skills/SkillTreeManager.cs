using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject fullScreenOverlay;
    public GameObject welcomePanel;
    public GameObject mainTreePanel;

    [Header("Text References")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI mainTreeCaptionText;
    public TextMeshProUGUI pointsText;

    [Header("Deity Skill Trees")]
    public GameObject minervaTree;
    public GameObject dianaTree;
    public GameObject herculesTree;

    private static bool hasSeenWelcome = false; // Static variable to track if the welcome message has been seen across all instances
    private string currentDeity;

    void Update()
    {
        // Refresh points when the tree is open
        if (mainTreePanel.activeSelf && PlayerStats.Instance != null)
        {
            pointsText.text = "Available Points: " + PlayerStats.Instance.skillPoints;
        }
    }

    public void OpenTree(string deityName)
    {
        currentDeity = deityName;
        fullScreenOverlay.SetActive(true);

        GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;

        if (!hasSeenWelcome)
        {
            ShowWelcome();
        }
        else
        {
            ShowMainTree();
        }
    }

    public void ShowWelcome()
    {
        welcomeText.text = $"The statue of {currentDeity} welcomes you. " +
                           "Here you can improve your skills by spending skill points that you get through leveling up.";
        welcomePanel.SetActive(true);
        mainTreePanel.SetActive(false);
    }

    public void ShowMainTree()
    {
        hasSeenWelcome = true;
        mainTreeCaptionText.text = $"Skill Tree of {currentDeity}";
        welcomePanel.SetActive(false);
        mainTreePanel.SetActive(true);
        minervaTree.SetActive(false);
        dianaTree.SetActive(false);
        herculesTree.SetActive(false);

        string selected = CharacterClassData.SelectedClass.className;
        if (selected == "Legionary") minervaTree.SetActive(true);
        else if (selected == "Archer") dianaTree.SetActive(true);
        else if (selected == "Gladiator") herculesTree.SetActive(true);
    }

    public void CloseTree()
    {
        fullScreenOverlay.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }

    public void BuySkill(SkillData skill)
    {
        // Check if player has enough points and if prerequisites are met
        if (PlayerStats.Instance.skillPoints >= skill.cost)
        {
            // Deduct points
            PlayerStats.Instance.skillPoints -= skill.cost;

            // Apply bonuses to PlayerStats or Controller
            ApplySkillEffects(skill);

            Debug.Log("Purchased: " + skill.skillName);
        }
        else
        {
            Debug.Log("Not enough skill points!");
        }
    }

    private void ApplySkillEffects(SkillData skill)
    {
        // Accessing PlayerStats instance to add permanent bonuses
        PlayerStats.Instance.maxHealth += skill.healthBonus;
        PlayerStats.Instance.currentHealth += skill.healthBonus; // Heal by the bonus amount
        PlayerStats.Instance.bonusDamage += skill.damageBonus;

        // Accessing PlayerController for physical bonuses[cite: 1]
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.attackRange += skill.attackRangeBonus;
        }
    }
}