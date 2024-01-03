using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Count { get; protected set; }
    public ItemCategory Category { get; protected set; }
    public Sprite Image { get; protected set; }


    public Item(string name, string description, ItemCategory category, int count = 1)
    {
        Name = name;
        Description = description;
        Category = category;
        Count = count;
    }

    public void AddCount(int amount)
    {
        Count += amount;
    }
    public void AddImage(Sprite image)
    {
        Image = image;
    }
}
public enum ItemCategory
{
    Item,
    Potion,
    Pokeball,
    TM,
    Berry,
    BattleItem,
    KeyItem,

}

