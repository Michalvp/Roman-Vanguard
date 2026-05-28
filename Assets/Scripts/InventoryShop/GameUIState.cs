using UnityEngine;

/// <summary>
/// One tiny place that tells gameplay scripts whether a menu is open.
/// PlayerController reads this so the player cannot move/attack while
/// inventory or shop is open.
/// </summary>
public static class GameUIState
{
    public static bool IsInventoryOpen { get; private set; }
    public static bool IsShopOpen { get; private set; }

    public static bool IsAnyBlockingMenuOpen => IsInventoryOpen || IsShopOpen;

    public static void SetInventoryOpen(bool isOpen)
    {
        IsInventoryOpen = isOpen;
    }

    public static void SetShopOpen(bool isOpen)
    {
        IsShopOpen = isOpen;
    }

    public static void ClearAll()
    {
        IsInventoryOpen = false;
        IsShopOpen = false;
    }
}
