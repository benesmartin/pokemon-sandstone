using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public List<PokemonData> playerPokemons;
    public List<PokemonData> storageSystemPokemons;
    public List<ItemData> inventory;
    public float playerPosX, playerPosY;
    public bool spokeToProfessor;
    public string location;
    public string pokedex;
    public string time;
    public string player;
    public string badges;
    public string scene;
}

[System.Serializable]
public class PokemonData
{
    public int pokedexNumber;
    public int level;

    public PokemonData(int pokedexNumber, int level)
    {
        this.pokedexNumber = pokedexNumber;
        this.level = level;
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int amount;
    public string description;
    public ItemCategory category;
    public string itemType;

    public ItemData(string itemName, int amount, string description, ItemCategory category, string itemType)
    {
        this.itemName = itemName;
        this.amount = amount;
        this.description = description;
        this.category = category;
        this.itemType = itemType;
    }
}

public static class SaveSystem
{
    public static void SaveGame(int slot)
    {
        string path = Application.persistentDataPath + "/save" + slot + ".json";
        PlayerMovement.Instance.SaveCharacterPosition();
        SaveDataManager.Instance.UpdateStats();
        SaveData data = new SaveData
        {
            playerPokemons = PokemonParty.Instance.Pokemons.Select(p => new PokemonData(p.PokedexNumber, p.Level)).ToList(),
            storageSystemPokemons = PokemonParty.Instance.StorageSystem.Select(p => new PokemonData(p.PokedexNumber, p.Level)).ToList(),
            inventory = Inventory.Instance.Items.Select(i => new ItemData(i.Name, i.Count, i.Description, i.Category, i.GetType().Name)).ToList(),
            playerPosX = CharacterValueManager.Instance.posX,
            playerPosY = CharacterValueManager.Instance.posY,
            spokeToProfessor = CharacterValueManager.Instance.SpokeToProfessor,
            location = SaveDataManager.Instance.Location,
            pokedex = SaveDataManager.Instance.Pokedex,
            time = SaveDataManager.Instance.Time,
            player = SaveDataManager.Instance.Player,
            badges = SaveDataManager.Instance.Badges,
            scene = SaveDataManager.Instance.Scene
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Game saved to " + path);
    }

    public static SaveData LoadGame(int slot)
    {
        string path = Application.persistentDataPath + "/save" + slot + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static bool SaveExists(int slot)
    {
        string path = Application.persistentDataPath + "/save" + slot + ".json";
        return File.Exists(path);
    }

}