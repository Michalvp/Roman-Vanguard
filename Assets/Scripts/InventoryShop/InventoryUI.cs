using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Input")]
    public KeyCode inventoryKey = KeyCode.E;

    [Header("Main UI")]
    public GameObject rootPanel;
    public Transform slotParent;
    public InventorySlotUI slotPrefab;

    [Header("Selected Item Info")]
    public Image selectedIcon;
    public TextMeshProUGUI selectedNameText;
    public TextMeshProUGUI selectedDescriptionText;
    public TextMeshProUGUI selectedStatsText;
    public Button useOrEquipButton;

    [Header("Equipped Text")]
    public TextMeshProUGUI equippedWeaponText;
    public TextMeshProUGUI equippedArmorText;

    private int selectedSlotIndex = -1;

    private void Awake()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (useOrEquipButton != null)
            useOrEquipButton.onClick.AddListener(UseSelectedSlot);
    }

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (rootPanel == null)
            return;

        bool newState = !rootPanel.activeSelf;
        rootPanel.SetActive(newState);

        if (newState)
            Refresh();
    }

    public void Refresh()
    {
        if (PlayerInventory.Instance == null || slotParent == null || slotPrefab == null)
            return;

        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < PlayerInventory.Instance.slots.Count; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotParent);
            slotUI.Setup(this, i, PlayerInventory.Instance.slots[i]);
        }

        UpdateEquippedText();
        UpdateSelectedInfo();
    }

    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        UpdateSelectedInfo();
    }

    private void UpdateSelectedInfo()
    {
        ShopItemData item = null;

        if (PlayerInventory.Instance != null &&
            selectedSlotIndex >= 0 &&
            selectedSlotIndex < PlayerInventory.Instance.slots.Count)
        {
            InventorySlot slot = PlayerInventory.Instance.slots[selectedSlotIndex];
            if (!slot.IsEmpty)
                item = slot.item;
        }

        bool hasItem = item != null;

        if (selectedIcon != null)
        {
            selectedIcon.enabled = hasItem && item.icon != null;
            selectedIcon.sprite = hasItem ? item.icon : null;
        }

        if (selectedNameText != null)
            selectedNameText.text = hasItem ? item.itemName : "Select an item";

        if (selectedDescriptionText != null)
            selectedDescriptionText.text = hasItem ? item.description : "Click an item slot.";

        if (selectedStatsText != null)
            selectedStatsText.text = hasItem ? item.GetStatsText() : "";

        if (useOrEquipButton != null)
        {
            useOrEquipButton.interactable = hasItem;

            TextMeshProUGUI buttonText = useOrEquipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (!hasItem) buttonText.text = "Use";
                else if (item.itemType == RomanItemType.Weapon || item.itemType == RomanItemType.Armor) buttonText.text = "Equip";
                else if (item.itemType == RomanItemType.Consumable) buttonText.text = "Use";
                else buttonText.text = "Cannot Use";
            }
        }
    }

    private void UpdateEquippedText()
    {
        if (PlayerInventory.Instance == null)
            return;

        if (equippedWeaponText != null)
            equippedWeaponText.text = PlayerInventory.Instance.equippedWeapon != null
                ? $"Weapon: {PlayerInventory.Instance.equippedWeapon.itemName}"
                : "Weapon: Empty";

        if (equippedArmorText != null)
            equippedArmorText.text = PlayerInventory.Instance.equippedArmor != null
                ? $"Armor: {PlayerInventory.Instance.equippedArmor.itemName}"
                : "Armor: Empty";
    }

    private void UseSelectedSlot()
    {
        if (PlayerInventory.Instance == null)
            return;

        PlayerInventory.Instance.UseSlot(selectedSlotIndex);
        Refresh();
    }
}
