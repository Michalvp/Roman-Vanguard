using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public Button button;

    private InventoryUI inventoryUI;
    private int slotIndex;

    public void Setup(InventoryUI inventoryUI, int slotIndex, InventorySlot slot)
    {
        this.inventoryUI = inventoryUI;
        this.slotIndex = slotIndex;

        bool hasItem = slot != null && !slot.IsEmpty;

        if (iconImage != null)
        {
            iconImage.enabled = hasItem && slot.item.icon != null;
            iconImage.sprite = hasItem ? slot.item.icon : null;
            iconImage.preserveAspect = true;
        }

        if (quantityText != null)
        {
            quantityText.text = hasItem && slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (this.inventoryUI != null)
                    this.inventoryUI.SelectSlot(this.slotIndex);
            });
        }
    }
}
