using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("Main UI")]
    public GameObject rootPanel;
    public Transform itemListParent;
    public ShopItemRowUI rowPrefab;

    [Header("Selected Item Info")]
    public Image selectedIcon;
    public TextMeshProUGUI selectedNameText;
    public TextMeshProUGUI selectedDescriptionText;
    public TextMeshProUGUI selectedStatsText;
    public TextMeshProUGUI selectedPriceText;
    public TextMeshProUGUI selectedClassText;
    public TextMeshProUGUI playerMoneyText;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Buttons")]
    public Button buyButton;
    public Button closeButton;

    public bool IsOpen => rootPanel != null && rootPanel.activeSelf;

    private ShopInteractable currentShop;
    private ShopItemData selectedItem;
    private ShopItemRowUI selectedRow;

    private void Awake()
    {
        Instance = this;

        if (rootPanel != null)
            rootPanel.SetActive(false);

        GameUIState.SetShopOpen(false);

        if (buyButton != null)
            buyButton.onClick.AddListener(BuySelectedItem);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
    }

    private void OnDisable()
    {
        GameUIState.SetShopOpen(false);
    }

    public void OpenShop(ShopInteractable shop)
    {
        if (rootPanel == null)
        {
            Debug.LogWarning("ShopUI is missing Root Panel.");
            return;
        }

        // Do not stack inventory and shop on top of each other.
        if (InventoryUI.Instance != null && InventoryUI.Instance.IsOpen)
            InventoryUI.Instance.CloseInventory();

        currentShop = shop;
        selectedItem = null;
        selectedRow = null;

        rootPanel.SetActive(true);
        GameUIState.SetShopOpen(true);

        BuildShopList();
        UpdateSelectedInfo();
        UpdateMoneyText();
        ClearFeedback();
    }

    public void CloseShop()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        GameUIState.SetShopOpen(false);
        currentShop = null;
        selectedItem = null;

        if (selectedRow != null)
        {
            selectedRow.SetSelected(false);
            selectedRow = null;
        }

        UpdateSelectedInfo();
        ClearFeedback();
    }

    public void ToggleShop(ShopInteractable shop)
    {
        if (IsOpen)
            CloseShop();
        else
            OpenShop(shop);
    }

    private void BuildShopList()
    {
        if (itemListParent == null || rowPrefab == null || currentShop == null)
            return;

        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (ShopItemData item in currentShop.itemsForSale)
        {
            if (item == null || !item.canBeSoldInShop)
                continue;

            ShopItemRowUI row = Instantiate(rowPrefab, itemListParent);
            row.Setup(this, item);
        }
    }

    public void SelectItem(ShopItemRowUI row, ShopItemData item)
    {
        if (selectedRow != null)
        {
            selectedRow.SetSelected(false);
        }

        selectedRow = row;
        selectedItem = item;

        if (selectedRow != null)
        {
            selectedRow.SetSelected(true);
        }

        ClearFeedback();
        UpdateSelectedInfo();
    }

    public void SelectItem(ShopItemData item)
    {
        SelectItem(null, item);
    }

    private void UpdateSelectedInfo()
    {
        bool hasItem = selectedItem != null;

        if (selectedIcon != null)
        {
            selectedIcon.enabled = hasItem && selectedItem.icon != null;
            selectedIcon.sprite = hasItem ? selectedItem.icon : null;
            selectedIcon.preserveAspect = true;
        }

        if (selectedNameText != null)
            selectedNameText.text = hasItem ? selectedItem.itemName : "Select an item";

        if (selectedDescriptionText != null)
            selectedDescriptionText.text = hasItem ? selectedItem.description : "Choose an item from the shop list.";

        if (selectedStatsText != null)
            selectedStatsText.text = hasItem ? selectedItem.GetStatsText() : "";

        if (selectedPriceText != null)
            selectedPriceText.text = hasItem ? selectedItem.GetPriceText() : "";

        if (selectedClassText != null)
            selectedClassText.text = hasItem ? selectedItem.GetClassText() : "";

        if (buyButton != null)
            buyButton.interactable = hasItem;
    }

    private void UpdateMoneyText()
    {
        if (playerMoneyText == null || PlayerStats.Instance == null)
            return;

        int money = PlayerStats.Instance.denarii;
        playerMoneyText.text = $"Your money: {RomanCurrency.FormatDenarii(money)}";
    }

    private void BuySelectedItem()
    {
        if (selectedItem == null)
            return;

        if (PlayerStats.Instance == null || PlayerInventory.Instance == null)
        {
            Debug.LogWarning("PlayerStats or PlayerInventory missing from Player.");
            return;
        }

        CharacterClassData currentClass = CharacterClassData.SelectedClass;
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();

        if (player != null && player.classData != null)
            currentClass = player.classData;

        if (!selectedItem.CanUseWithClass(currentClass))
        {
            ShowFeedback($"{selectedItem.itemName} cannot be bought by your current class.", success: false);
            return;
        }

        if (!PlayerInventory.Instance.HasFreeSpaceFor(selectedItem))
        {
            ShowFeedback("Inventory is full.", success: false);
            return;
        }

        if (!PlayerStats.Instance.TrySpendDenarii(selectedItem.priceDenarii))
        {
            ShowFeedback("Not enough denarii.", success: false);
            return;
        }

        PlayerInventory.Instance.AddItem(selectedItem, 1);
        UpdateMoneyText();

        ShowFeedback("Success!", success: true);
    }

    private void ShowFeedback(string message, bool success)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = message;
        feedbackText.color = success ? new Color(0.18f, 0.80f, 0.44f) : new Color(0.90f, 0.22f, 0.22f);
    }

    private void ClearFeedback()
    {
        if (feedbackText == null)
            return;

        feedbackText.text = string.Empty;
    }
}
