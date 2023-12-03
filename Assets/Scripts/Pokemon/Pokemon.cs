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
        if (Base.LearnableMoves == null)
        {
            Debug.Log("LearnableMoves is null");
            return;
        }
        foreach (LearnableMove move in Base.LearnableMoves)
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
    public void TakeDamage(Move move, float effectiveness)
    {
        float burn = 1f;
        float targets = 1f;
        float weather = 1f;
        float parentalBond = 1f;
        float zMove = 1f;
        float teraShield = 1f;
        float glaiveRush = 1f;

        float critical = /*Random.value <= 0.0625f ? 1.5f : 1f*/ 1;
        float stab = move.Base.Type == Base.Type1 || move.Base.Type == Base.Type2 ? 1.5f : 1f;
        float types = effectiveness;

        float modifiers = Random.Range(0.85f, 1f);
        float a = ((2 * Level) / 5) + 2;
        float d = a * move.Base.Power * ((float)Attack / Defense);
        d = (d / 50) + 2;
        d *= targets * parentalBond * weather * glaiveRush * critical * modifiers * stab * types * burn * zMove * teraShield;

        int damage = Mathf.FloorToInt(d);
        Debug.Log(damage);

        HP -= damage;
    }

    public int MaxHP { get => Mathf.FloorToInt(0.01f * (2 * Base.MaxHP + 31 + Mathf.Floor(0.25f * 255) * Level) + Level + 10); }
    public int Attack { get => Mathf.FloorToInt(0.01f * (2 * Base.Attack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Defense { get => Mathf.FloorToInt(0.01f * (2 * Base.Defense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpAttack { get => Mathf.FloorToInt(0.01f * (2 * Base.SpAttack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpDefense { get => Mathf.FloorToInt(0.01f * (2 * Base.SpDefense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Speed { get => Mathf.FloorToInt(0.01f * (2 * Base.Speed + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
}
