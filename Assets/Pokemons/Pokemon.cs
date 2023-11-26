using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public PokemonBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHP;
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }
    public int MaxHP { get => Mathf.FloorToInt(0.01f * (2 * Base.MaxHP + 31 + Mathf.Floor(0.25f * 255) * Level) + Level + 10); }
    public int Attack { get => Mathf.FloorToInt(0.01f * (2 * Base.Attack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Defense { get => Mathf.FloorToInt(0.01f * (2 * Base.Defense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpAttack { get => Mathf.FloorToInt(0.01f * (2 * Base.SpAttack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpDefense { get => Mathf.FloorToInt(0.01f * (2 * Base.SpDefense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Speed { get => Mathf.FloorToInt(0.01f * (2 * Base.Speed + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
}
