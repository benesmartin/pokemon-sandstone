using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;
    public static MapArea Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    public Pokemon GetRandomWildPokemon()
    {
        var random = Random.Range(0, wildPokemons.Count);
        var wildPokemon = wildPokemons[random];
        wildPokemon.Init();
        return wildPokemon;
    }
}
