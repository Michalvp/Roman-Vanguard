using System.Collections.Generic;
using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    [Header("Shop Inventory")]
    public List<ShopItemData> itemsForSale = new List<ShopItemData>();

    [Header("UI")]
    public ShopUI shopUI;

    [Header("Optional Visual Feedback")]
    public GameObject highlightObject;
    public GameObject pressSPromptObject;

    public void Interact()
    {
        if (shopUI == null)
            shopUI = Object.FindFirstObjectByType<ShopUI>();

        if (shopUI == null)
        {
            Debug.LogWarning("No ShopUI found in the scene.");
            return;
        }

        // Same key behavior:
        // Press S near closed shop -> open it.
        // Press S while shop is open -> close it.
        if (shopUI.IsOpen)
            shopUI.CloseShop();
        else
            shopUI.OpenShop(this);
    }

    public void SetHighlight(bool isActive)
    {
        if (highlightObject != null)
            highlightObject.SetActive(isActive);

        if (pressSPromptObject != null)
            pressSPromptObject.SetActive(isActive);
    }
}
