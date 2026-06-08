using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    // UI references
    public Image iconImage;
    public GameObject selectionHighlight;
    public TextMeshProUGUI quantityText;
    public Button button;

    // Internal state
    private InventoryUI inventoryUI;
    private int slotIndex;

    /// <summary>
    /// Initialize this slot UI with the inventory UI, its index, and the data for the slot.
    /// </summary>
    public void Setup(InventoryUI inventoryUI, int slotIndex, InventorySlot slot)
    {
        this.inventoryUI = inventoryUI;
        this.slotIndex = slotIndex;

        bool hasItem = slot != null && !slot.IsEmpty;

        // Icon handling
        if (iconImage != null)
        {
            iconImage.enabled = hasItem && slot.item.icon != null;
            iconImage.sprite = hasItem ? slot.item.icon : null;
            iconImage.preserveAspect = true;
        }

        // Quantity text for stackable items
        if (quantityText != null)
        {
            quantityText.text = hasItem && slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

        // Ensure we have a Button component
        if (button == null)
            button = GetComponent<Button>();

        // Configure the click listener to notify InventoryUI of selection
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (this.inventoryUI != null)
                    this.inventoryUI.SelectSlot(this.slotIndex);
            });
        }

        // Initialise highlight to inactive
        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    /// <summary>
    /// Called by InventoryUI to toggle the visual highlight for the selected slot.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(isSelected);
    }
}
