using System;
using UnityEngine;

public class KeyItem : Item
{
    private Action _onUseCallback;

    public KeyItem(string name, string description, Action onUseCallback)
        : base(name, description, ItemCategory.KeyItem)
    {
        _onUseCallback = onUseCallback;
    }

    public void Use()
    {
        _onUseCallback?.Invoke();
    }
}

public class Map : KeyItem
{
    public Map() : base("Map", "A detailed map of the Ejiputo region, useful for adventurers.", OpenMap)
    {
    }
    private static void OpenMap()
    {
        Debug.Log("Opening the Explorer Map...");
        PauseMenu.Instance.OpenMap();
    }
}
