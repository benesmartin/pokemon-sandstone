using System.Collections.Generic;
using UnityEngine;

public class PokemonBaseDatabase
{
    private static PokemonBaseDatabase _instance;
    public static PokemonBaseDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PokemonBaseDatabase();
            return _instance;
        }
    }

    private Dictionary<int, PokemonBase> database = new Dictionary<int, PokemonBase>();

    private PokemonBaseDatabase()
    {
        LoadPokemonBases();
    }

    private void LoadPokemonBases()
    {
        PokemonBase[] bases = Resources.LoadAll<PokemonBase>("Bases");
        foreach (var pokemonBase in bases)
        {
            if (!database.ContainsKey(pokemonBase.PokedexNumber))
                database.Add(pokemonBase.PokedexNumber, pokemonBase);
        }
    }

    public PokemonBase FindByPokedexNumber(int pokedexNumber)
    {
        if (database.TryGetValue(pokedexNumber, out PokemonBase pokemonBase))
        {
            return pokemonBase;
        }
        else
        {
            Debug.LogWarning($"PokemonBase not found for Pokedex number: {pokedexNumber}");
            return null;
        }
    }
}
