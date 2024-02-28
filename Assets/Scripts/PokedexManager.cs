using System.Collections.Generic;
using UnityEngine;

public class PokedexManager : MonoBehaviour
{
    public static PokedexManager Instance { get; private set; }

    private Dictionary<int, bool> pokemonStatus = new Dictionary<int, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPokemonAsSeen(int pokemonId)
    {
        if (!pokemonStatus.ContainsKey(pokemonId))
        {
            pokemonStatus.Add(pokemonId, false);
        }
    }

    public void AddPokemonAsCaught(int pokemonId)
    {
        pokemonStatus[pokemonId] = true;
    }

    public bool IsPokemonCaught(int pokemonId)
    {
        if (pokemonStatus.TryGetValue(pokemonId, out bool isCaught))
        {
            return isCaught;
        }
        return false;
    }

    public bool IsPokemonSeen(int pokemonId)
    {
        return pokemonStatus.ContainsKey(pokemonId);
    }
}
