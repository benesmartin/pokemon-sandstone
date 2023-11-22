using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    PokemonBase _base;
    int level;
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
    }
    public int MaxHP { get => Mathf.FloorToInt(0.01f * (2 * _base.MaxHP + 31 + Mathf.Floor(0.25f * 255) * level) + level + 10); }
    public int Attack { get => Mathf.FloorToInt(0.01f * (2 * _base.Attack + 31 + Mathf.Floor(0.25f * 255) * level) + 5) * 1; }
    public int Defense { get => Mathf.FloorToInt(0.01f * (2 * _base.Defense + 31 + Mathf.Floor(0.25f * 255) * level) + 5) * 1; }
    public int SpAttack { get => Mathf.FloorToInt(0.01f * (2 * _base.SpAttack + 31 + Mathf.Floor(0.25f * 255) * level) + 5) * 1; }
    public int SpDefense { get => Mathf.FloorToInt(0.01f * (2 * _base.SpDefense + 31 + Mathf.Floor(0.25f * 255) * level) + 5) * 1; }
    public int Speed { get => Mathf.FloorToInt(0.01f * (2 * _base.Speed + 31 + Mathf.Floor(0.25f * 255) * level) + 5) * 1; }
}
