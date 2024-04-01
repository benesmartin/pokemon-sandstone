using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base { get => _base; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Guid ID { get => id; set { id = value; } }
    private Guid id = Guid.Empty;
    public int Level { get => level; }
    public int Exp { get; set; }
    public int HP { get; set; }
    public Condition Status { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public int StatusTime { get; set; }

    public List<Move> Moves { get; set; }
    public void Init()
    {
        ID = ID != Guid.Empty ? ID : Guid.NewGuid();
        Debug.Log($"Initializing new Pokémon: {Base.Name} with ID: {ID}");
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
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Exp = Base.GetExpForLevel(Level);
        CalculateStats();
        Heal();
    }
    public void CureStatus()
    {
        Status = null;
    }
    public Pokemon Clone()
    {
        Pokemon clone = new Pokemon
        {
            _base = this._base,
            level = this.level,
            Exp = this.Exp,
            HP = this.HP,
            Moves = this.Moves != null ? new List<Move>(this.Moves.Select(move => move.Clone())) : null
        };
        return clone;
    }

    public void Heal()
    {
        HP = MaxHP;
    }

    public void Heal(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHP);
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(Level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
    }

    public ConditionID GetStatus(Condition condition)
    {
        return ConditionsDB.Conditions.FirstOrDefault(x => x.Value == condition).Key;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.Level <= Level);
    }

    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvedForm;
        CalculateStats();
    }
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        return statVal;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(0.01f * (2 * Base.Attack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1);
        Stats.Add(Stat.Defense, Mathf.FloorToInt(0.01f * (2 * Base.Defense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt(0.01f * (2 * Base.SpAttack + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt(0.01f * (2 * Base.SpDefense + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1);
        Stats.Add(Stat.Speed, Mathf.FloorToInt(0.01f * (2 * Base.Speed + 31 + Mathf.Floor(0.25f * 255) * Level) + 5) * 1);

        MaxHP = Mathf.FloorToInt(0.01f * (2 * Base.MaxHP + 31 + Mathf.Floor(0.25f * 255) * Level) + Level + 10);
    }

    public void TakeDamage(Move move, float effectiveness, bool isCritical, Pokemon attacker)
    {
        if (move.Base.Power == 0)
        {
            Debug.Log($"{attacker.Base.Name} used {move.Base.Name} on {Base.Name}, but it had no effect!");
            return;
        }
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
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
    }
    public void OnAfterTurn()
    {
        if (Status?.OnAfterTurn != null)
        {
            BattleSystem.Instance.FlashAfter(this);
            Status.OnAfterTurn.Invoke(this);
        }
    }
    public bool OnBeforeMove()
    {
        if (Status?.OnBeforeMove != null)
        {
            return Status.OnBeforeMove(this);
        }

        return true;
    }
    public int MaxHP { get; set; }
    public int Attack { get => GetStat(Stat.Attack); }
    public int Defense { get => GetStat(Stat.Defense); }
    public int SpAttack { get => GetStat(Stat.SpAttack); }
    public int SpDefense { get => GetStat(Stat.SpDefense); }
    public int Speed { get => GetStat(Stat.Speed); }
}
