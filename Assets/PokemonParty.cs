using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;
    private List<Pokemon> storageSystem;
    private int maxSize = 6;

    public static PokemonParty Instance { get; private set; }
    public List<Pokemon> Pokemons => pokemons;
    public List<Pokemon> StorageSystem => storageSystem;
    public int MaxSize => maxSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            storageSystem = new List<Pokemon>(); // Initialize the storage system list
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }


    private void Start()
    {
        if (Instance == this) // Ensure initialization only occurs for the persistent instance
        {
            foreach (var pokemon in pokemons)
            {
                pokemon.Init();
            }
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        foreach (var pokemon in pokemons)
        {
            if (pokemon.HP > 0)
            {
                return pokemon;
            }
        }
        return null;
    }

    // Add methods to manage the party (add/remove Pokémon, etc.) as needed
    public string AddPokemon(Pokemon pokemon) 
    {         
        if (pokemons.Count < maxSize)
        {
            pokemons.Add(pokemon);
            return pokemon.Base.Name + " was sent to your party!";
        }
        else
        {
            storageSystem.Add(pokemon);
            return pokemon.Base.Name + " was sent to someone's PC!";
        }
    }
    public void HealAllPokemon()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Heal();
        }
    }
}
