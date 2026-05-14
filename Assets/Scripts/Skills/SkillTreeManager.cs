using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Confirmation Popup")]
    public GameObject confirmPopup;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText;
    public TextMeshProUGUI costText;
    public Button unlockButton;
    public TextMeshProUGUI unlockButtonText;

    private SkillData selectedSkill;

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

        RefreshAllButtons();
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

        RefreshAllButtons();
    }

    public void CloseTree()
    {
        fullScreenOverlay.SetActive(false);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }

    public void RefreshAllButtons()
    {
        SkillButtonUI[] buttons = Object.FindObjectsByType<SkillButtonUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var btn in buttons)
        {
            btn.UpdateVisuals();
        }
    }

    // Triggered when clicking a skill icon
    public void SelectSkill(SkillData skill)
    {
        selectedSkill = skill;
        confirmPopup.SetActive(true);

        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;
        costText.text = "Required Skill Points: " + skill.cost;

        UpdateConfirmButtonState();
    }

    private void UpdateConfirmButtonState()
    {
        if (selectedSkill.isUnlocked)
        {
            unlockButton.interactable = false;
            unlockButtonText.text = "Already unlocked";
        }
        else if (selectedSkill.requiredSkill != null && !selectedSkill.requiredSkill.isUnlocked)
        {
            unlockButton.interactable = false;
            unlockButtonText.text = "Prerequisite required";
        }
        else if (PlayerStats.Instance.skillPoints < selectedSkill.cost)
        {
            unlockButton.interactable = false;
            unlockButtonText.text = "Not enough skill points";
        }
        else
        {
            unlockButton.interactable = true;
            unlockButtonText.text = "Unlock Skill";
        }
    }

    // Triggered by the "Unlock Skill" button in the popup
    public void ConfirmPurchase()
    {
        if (selectedSkill == null) return;

        PlayerStats.Instance.skillPoints -= selectedSkill.cost;
        selectedSkill.isUnlocked = true;

        // Apply effects (Logic from previous steps)
        ApplySkillEffects(selectedSkill);

        confirmPopup.SetActive(false);
        RefreshAllButtons();
    }

    private void ApplySkillEffects(SkillData skill)
    {
        // Permanently add bonuses to global stats
        PlayerStats.Instance.maxHealth += skill.healthBonus;
        PlayerStats.Instance.currentHealth += skill.healthBonus;
        PlayerStats.Instance.bonusDamage += skill.damageBonus;
        PlayerStats.Instance.hasSpecialAbility = skill.unlocksSpecialAbility;


        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.attackRange += skill.attackRangeBonus;
            player.attackRate += skill.attackSpeedBonus;
            player.moveSpeed += skill.speedBonus;
            player.dashForce += skill.dashBonus;
            player.criticalChance += skill.criticalChanceBonus;

        }
    }
}