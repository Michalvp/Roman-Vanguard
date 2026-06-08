using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

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
    public Button sellButton;

    [Header("Equipped Text")]
    public TextMeshProUGUI equippedWeaponText;
    public TextMeshProUGUI equippedArmorText;

    public bool IsOpen => rootPanel != null && rootPanel.activeSelf;

    private int selectedSlotIndex = -1;
    private bool isSellingMode = false;

    private void Awake()
    {
        Instance = this;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        GameUIState.SetInventoryOpen(false);

        if (useOrEquipButton != null)
            useOrEquipButton.onClick.AddListener(UseSelectedSlot);

        if (sellButton != null)
            sellButton.onClick.AddListener(SellSelectedSlot);
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

        GameUIState.SetInventoryOpen(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(inventoryKey))
            return;

        // Do not open inventory on top of the shop.
        // Close the shop first, then press E again if you want inventory.
        if (!IsOpen && ShopUI.Instance != null && ShopUI.Instance.IsOpen)
            return;

        ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (IsOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory(bool sellingMode = false)
    {
        isSellingMode = sellingMode;
        if (rootPanel == null)
        {
            Debug.LogWarning("InventoryUI is missing Root Panel.");
            return;
        }

        // Safety: inventory and shop should not cover each other.
        if (ShopUI.Instance != null && ShopUI.Instance.IsOpen)
            ShopUI.Instance.CloseShop();

        rootPanel.SetActive(true);
        GameUIState.SetInventoryOpen(true);

        selectedSlotIndex = -1;
        Refresh();
    }

    public void CloseInventory()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        isSellingMode = false;

        GameUIState.SetInventoryOpen(false);
        selectedSlotIndex = -1;
        UpdateSelectedInfo();
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
            selectedIcon.preserveAspect = true;
        }

        if (selectedNameText != null)
            selectedNameText.text = hasItem ? item.itemName : "Select an item";

        if (selectedDescriptionText != null)
            selectedDescriptionText.text = hasItem ? item.description : "Click an item slot.";

        if (selectedStatsText != null)
            selectedStatsText.text = hasItem ? item.GetStatsText() : "";

        // First, set both buttons to inactive
        if (useOrEquipButton != null)
            useOrEquipButton.gameObject.SetActive(false);
            
        if (sellButton != null)
            sellButton.gameObject.SetActive(false);

        // Then activate the correct one if we have an item
        if (hasItem)
        {
            if (isSellingMode)
            {
                if (sellButton != null)
                    sellButton.gameObject.SetActive(true);
            }
            else
            {
                if (useOrEquipButton != null)
                {
                    useOrEquipButton.gameObject.SetActive(true);
                    
                    bool canUseButton =
                        (item.itemType == RomanItemType.Weapon ||
                         item.itemType == RomanItemType.Armor ||
                         item.itemType == RomanItemType.Consumable);

                    useOrEquipButton.interactable = canUseButton;

                    TextMeshProUGUI buttonText = useOrEquipButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (buttonText != null)
                    {
                        if (item.itemType == RomanItemType.Weapon || item.itemType == RomanItemType.Armor)
                            buttonText.text = "Equip";
                        else if (item.itemType == RomanItemType.Consumable)
                            buttonText.text = "Use";
                        else
                            buttonText.text = "Cannot Use";
                    }
                }
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

    private void SellSelectedSlot()
    {
        if (PlayerInventory.Instance == null || !isSellingMode)
            return;

        if (selectedSlotIndex < 0 || selectedSlotIndex >= PlayerInventory.Instance.slots.Count)
            return;

        InventorySlot slot = PlayerInventory.Instance.slots[selectedSlotIndex];
        if (slot == null || slot.IsEmpty)
            return;

        int sellPrice = Mathf.Max(1, slot.item.priceDenarii / 2); // Sell for half price

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AddDenarii(sellPrice);

        PlayerInventory.Instance.RemoveOne(selectedSlotIndex);
        // Refresh() is called automatically because of OnInventoryChanged event
    }
}
