using UnityEngine;
using UnityEngine.UI;

public class SkillButtonUI : MonoBehaviour
{
    public SkillData skill; // Reference to the skill asset
    private Image buttonImage;
    private Button buttonComponent;

    // Defined colors for skill states
    private Color unlockedColor = new Color(1f, 0.84f, 0f); // Gold/Yellow
    private Color lockedColor = new Color(0.3f, 0.3f, 0.3f);   // Dark Grey
    private Color availableColor = Color.white;             // Ready to buy

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonComponent = GetComponent<Button>();
    }

    // Refresh the visual state of the button
    public void UpdateVisuals()
    {
        if (skill == null) return;

        if (skill.isUnlocked)
        {
            buttonImage.color = unlockedColor;
        }
        else if (skill.requiredSkill != null && !skill.requiredSkill.isUnlocked)
        {
            buttonImage.color = lockedColor;
        }
        else
        {
            // Prerequisite met, but not yet purchased
            buttonImage.color = availableColor;
        }
    }
}