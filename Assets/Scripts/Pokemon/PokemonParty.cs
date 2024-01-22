using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons = new List<Pokemon>();
    [SerializeField] List<Pokemon> enemyPokemons;
    private List<Pokemon> storageSystem;
    private int maxSize = 6;

    public static PokemonParty Instance { get; private set; }
    public List<Pokemon> Pokemons => pokemons;
    public List<Pokemon> EnemyPokemons => enemyPokemons;
    public List<Pokemon> StorageSystem => storageSystem;
    public int MaxSize => maxSize;
    public bool IsWildBattle { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            storageSystem = new List<Pokemon>(); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }


    private void Start()
    {
        if (Instance == this) 
        {
                foreach (var pokemon in pokemons)
                {
                    pokemon.Init();
                }
            
                foreach (var pokemon in enemyPokemons)
                {
                    pokemon.Init();
                }
            
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        foreach (var pokemon in pokemons)
        {
            Debug.Log($"Checking {pokemon.Base.Name}, HP: {pokemon.HP}");
            if (pokemon.HP > 0)
            {
                Debug.Log($"Found healthy Pokemon: {pokemon.Base.Name}");
                return pokemon;
            }
        }
        Debug.Log("No healthy Pok�mon in player's party.");
        return null;
    }

    public Pokemon GetHealthyEnemyPokemon()
    {
        foreach (var pokemon in enemyPokemons)
        {
            if (pokemon.HP > 0)
            {
                return pokemon;
            }
        }
        return null;
    }

    
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
    public void AddPokemonV(Pokemon pokemon)
    {
        if (pokemons.Count < maxSize)
        {
            pokemons.Add(pokemon);
            pokemon.Init();
        }
        else
        {
            storageSystem.Add(pokemon);
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