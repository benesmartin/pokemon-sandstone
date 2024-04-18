using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonBaseData : MonoBehaviour
{
    [SerializeField] int pokedexNumber;
    [SerializeField] new string name;
    [SerializeField] string subName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int baseCatchRate = 100;

    [SerializeField] List<LearnableMove> learnableMoves;

    [SerializeField] List<Evolution> evolutions;
}
