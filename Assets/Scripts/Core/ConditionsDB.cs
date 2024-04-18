using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {ConditionID.psn, new Condition()
        {
            Name = "Poison",
            Description = "The Pokémon is poisoned.",
            StartMessage = " was poisoned!",
            FlashColor = BattleSystem.Instance.ColorHex("#7b00c6"),
            OnAfterTurn = (Pokemon pokemon) =>
            {
                pokemon.UpdateHP(pokemon.MaxHP / 8);
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt by poison!");
            }
        }},
        {ConditionID.brn, new Condition()
        {
            Name = "Burn",
            Description = "The Pokémon is burned.",
            StartMessage = " was burned!",
            FlashColor = BattleSystem.Instance.ColorHex("#ff7400"),
            OnAfterTurn = (Pokemon pokemon) =>
            {
                pokemon.UpdateHP(pokemon.MaxHP / 16);
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt by burn!");
            }
        }},
        {ConditionID.slp, new Condition()
        {
            Name = "Sleep",
            Description = "The Pokémon is asleep.",
            StartMessage = " fell asleep!",
            FlashColor = BattleSystem.Instance.ColorHex("#000097"),
            OnStart = (Pokemon pokemon) =>
            {
                pokemon.StatusTime = Random.Range(1, 4);
                Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
            },
            OnBeforeMove = (Pokemon pokemon) =>
            {
                if (pokemon.StatusTime <= 0)
                {
                    pokemon.CureStatus();
                    Debug.Log("1. Woke up!");
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                    return true;
                }

                pokemon.StatusTime--;
                pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is fast asleep.");
                return false;
            }
        }},
        {ConditionID.frz, new Condition()
        {
            Name = "Freeze",
            Description = "The Pokémon is frozen.",
            StartMessage = " was frozen solid!",
            FlashColor = BattleSystem.Instance.ColorHex("#00c7d3"),
            OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thawed out!");
                        return true;
                    }

                    return false;
                }
        }},
        {ConditionID.par, new Condition()
        {
            Name = "Paralyze",
            Description = "The Pokémon is paralyzed.",
            StartMessage = " was paralyzed!",
            FlashColor = BattleSystem.Instance.ColorHex("#d1e300"),
            OnBeforeMove = (Pokemon pokemon) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed! It can't move!");
                        return false;
                    }

                    return true;
                }
        }},
    };
}

public enum ConditionID
{
    none,
    psn,
    brn,
    slp,
    frz,
    par,
}
