using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteManager : MonoBehaviour
{
    public static EnemySpriteManager Instance;
    public Sprite enemySprite;
    public string enemyName, sceneName;
    public List<Pokemon> pokemons;
    public string battleOutcome = "None";
    public Dictionary<string, bool> defeatedTrainers = new Dictionary<string, bool>();
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
    public Sprite GetSprite()
    {
        return enemySprite;
    }
    public void SetSprite(Sprite sprite)
    {
        enemySprite = sprite;
    }
    public string GetName()
    {
        return enemyName;
    }
    public void SetName(string name)
    {
        enemyName = name;
    }
    public List<Pokemon> GetPokemons()
    {
        return pokemons;
    }
    public void SetPokemons(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
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
    public void SetSceneName(string name)
    {
        sceneName = name;
    }
    public string GetSceneName()
    {
        return sceneName;
    }
    public string GetBattleOutcome()
    {
        return battleOutcome;
    }
    public void SetBattleOutcome(string outcome)
    {
        battleOutcome = outcome;
    }
    public void ClearAll()
    {
        enemySprite = null;
        enemyName = null;
        pokemons = null;
        sceneName = null;
        battleOutcome = "None";
    }
    public void AddTrainerAsDefeated(string trainerName)
    {
        defeatedTrainers[trainerName] = true;
    }
    public bool IsTrainerDefeated(string trainerName)
    {
        return defeatedTrainers.ContainsKey(trainerName);
    }
}
