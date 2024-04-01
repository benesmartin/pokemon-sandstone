using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<string, Item> items;
    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            items = new Dictionary<string, Item>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public void AddItem(Item item)
    {
        if (items.ContainsKey(item.Name))
        {
            items[item.Name].AddCount(item.Count);
        }
        else
        {
            items.Add(item.Name, item);
        }
    }

    public Item GetItem(string itemName)
    {
        items.TryGetValue(itemName, out Item item);
        return item;
    }

    public int GetItemCount(string itemName)
    {
        if (items.TryGetValue(itemName, out Item item))
        {
            return item.Count;
        }
        return 0;
    }

    public List<Item> GetItemsByCategory(ItemCategory category)
    {
        var itemsInCategory = new List<Item>();
        foreach (var item in items.Values)
        {
            if (item.Category == category)
            {
                itemsInCategory.Add(item);
            }
        }
        return itemsInCategory;
    }

    public void CreateOrAddItem<T>(int count) where T : Item, new()
    {
        T newItem = new T();
        string filename = Regex.Replace(newItem.Name.ToLower(), @"\s+", "");
        newItem.AddImage((Sprite)Resources.Load($"{filename}", typeof(Sprite)) ? (Sprite)Resources.Load($"{filename}", typeof(Sprite)) : (Sprite)Resources.Load($"mark", typeof(Sprite)));
        if (items.ContainsKey(newItem.Name))
        {
            items[newItem.Name].AddCount(count);
            Debug.Log($"Updated {newItem.Name} count to {items[newItem.Name].Count}");
        }
        else
        {
            newItem.AddCount(count - 1);
            items.Add(newItem.Name, newItem);
            Debug.Log($"Added new item: {newItem.Name} with count {newItem.Count}");
        }
    }
    public void CreateOrAddItem(Item newItem, int count)
    {
        string filename = Regex.Replace(newItem.Name.ToLower(), @"\s+", "");
        newItem.AddImage((Sprite)Resources.Load($"{filename}", typeof(Sprite)) ?? (Sprite)Resources.Load("mark", typeof(Sprite)));

        if (items.ContainsKey(newItem.Name))
        {
            items[newItem.Name].AddCount(count);
            Debug.Log($"Updated {newItem.Name} count to {items[newItem.Name].Count}");
        }
        else
        {
            newItem.AddCount(count);
            items.Add(newItem.Name, newItem);
            Debug.Log($"Added new item: {newItem.Name} with count {newItem.Count}");
        }
    }


    public void PrintInventory()
    {
        Debug.Log("Inventory\n---");
        foreach (var kvp in items)
        {
            Debug.Log($"\n-> {kvp.Key} x{kvp.Value.Count}");
        }
    }
    public bool RemoveItem(string itemName, int count = 1)
    {
        if (items.TryGetValue(itemName, out Item item))
        {
            if (item.Count > count)
            {
                item.AddCount(-count);
                Debug.Log($"Decreased {itemName} count to {item.Count}");
            }
            else
            {
                items.Remove(itemName);
                Debug.Log($"Removed {itemName} from inventory");
            }
            return true;
        }
        else
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
            return false;
        }
    }

}
