using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base { get => _base;}
    public int Level { get => level;}
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public void Init()
    {
        Heal();
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
    public void Heal()
    {
        HP = MaxHP;
    }
    public void TakeDamage(Move move, float effectiveness, bool isCritical, Pokemon attacker)
    {
        float attack = move.Base.IsSpecial ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.IsSpecial ? SpDefense : Defense;

        float burn = 1f;
        float targets = 1f;
        float weather = 1f;
        float parentalBond = 1f;
        float zMove = 1f;
        float teraShield = 1f;
        float glaiveRush = 1f;
        float critical = isCritical ? 1.5f : 1f;
        float stab = move.Base.Type == Base.Type1 || move.Base.Type == Base.Type2 ? 1.5f : 1f;
        float types = effectiveness;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        float a = ((2 * attacker.Level) / 5) + 2;
        float d = a * move.Base.Power * ((float)attack / defense);
        d = (d / 50) + 2;
        d *= targets * parentalBond * weather * glaiveRush * critical * modifiers * stab * types * burn * zMove * teraShield;

        int damage = Mathf.FloorToInt(d);
        Debug.Log($"{attacker.Base.Name} used {move.Base.Name} on {Base.Name} for {damage} damage");
        HP = Mathf.Clamp(HP - damage, 0, HP);
    }

    public int MaxHP { get => Mathf.FloorToInt(0.01f * (2 * Base.MaxHP + 31 + Mathf.Floor(0.25f * 255) * Level) + Level + 10); }
    public int Attack { get => Mathf.FloorToInt(0.01f * (2 * Base.Attack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Defense { get => Mathf.FloorToInt(0.01f * (2 * Base.Defense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpAttack { get => Mathf.FloorToInt(0.01f * (2 * Base.SpAttack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int SpDefense { get => Mathf.FloorToInt(0.01f * (2 * Base.SpDefense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
    public int Speed { get => Mathf.FloorToInt(0.01f * (2 * Base.Speed + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1; }
}
