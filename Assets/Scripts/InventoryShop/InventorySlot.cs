using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ShopItemData item;
    [Min(0)] public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;

    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    public InventorySlot(ShopItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}
