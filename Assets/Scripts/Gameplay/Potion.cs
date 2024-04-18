using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Potion : Item
{
    public int HealingAmount { get; private set; }
    public List<ConditionID> CuresStatus { get; private set; }

    public Potion(string name, string description, int healingAmount, List<ConditionID> curesStatus = null)
        : base(name, description, ItemCategory.Potion)
    {
        HealingAmount = healingAmount;
        CuresStatus = curesStatus ?? new List<ConditionID>();
    }

    public virtual void Use(Pokemon pokemon)
    {
        if (HealingAmount > 0)
        {
            pokemon.Heal(HealingAmount);
        }

        if (CuresStatus != null && CuresStatus.Any())
        {
            var status = pokemon.Status;
            var statusID = pokemon.GetStatus(status);
            foreach (var condition in CuresStatus)
            {
                if (statusID == condition)
                {
                    pokemon.CureStatus();
                    Debug.Log($"{pokemon.Base.Name} was cured of {condition}!");
                    break;
                }
            }
        }
    }

    public string GetSpriteName()
    {
        return Regex.Replace(Name.ToLower(), @"\s+", "");
    }
}

public class StandardPotion : Potion
{
    public StandardPotion() : base("Potion", "A basic potion that restores 20 HP.", 20) { }
}

public class SuperPotion : Potion
{
    public SuperPotion() : base("Super Potion", "A potion that restores 60 HP.", 60) { }
}

public class HyperPotion : Potion
{
    public HyperPotion() : base("Hyper Potion", "A potion that restores 120 HP.", 120) { }
}

public class MaxPotion : Potion
{
    public MaxPotion() : base("Max Potion", "A potion that fully restores HP.", 0) { }

    public override void Use(Pokemon pokemon)
    {
        pokemon.Heal();
    }
}

public class FullRestore : Potion
{
    public FullRestore() : base("Full Restore", "A potion that fully restores HP and cures all status conditions.", 0, Enum.GetValues(typeof(ConditionID)).Cast<ConditionID>().ToList()) { }

    public override void Use(Pokemon pokemon)
    {
        pokemon.Heal();
        pokemon.CureStatus();
    }
}

public class FullHeal : Potion
{
    public FullHeal() : base("Full Heal", "A potion that cures all status conditions.", 0, Enum.GetValues(typeof(ConditionID)).Cast<ConditionID>().ToList()) { }
}

public class Antidote : Potion
{
    public Antidote() : base("Antidote", "A potion that cures poison.", 0, new List<ConditionID> { ConditionID.psn }) { }
}

public class Awakening : Potion
{
    public Awakening() : base("Awakening", "A potion that awakens a Pokémon from sleep.", 0, new List<ConditionID> { ConditionID.slp }) { }
}

public class BurnHeal : Potion
{
    public BurnHeal() : base("Burn Heal", "A potion that heals burns.", 0, new List<ConditionID> { ConditionID.brn }) { }
}

public class IceHeal : Potion
{
    public IceHeal() : base("Ice Heal", "A potion that thaws out frozen Pokémon.", 0, new List<ConditionID> { ConditionID.frz }) { }
}

public class Revive : Potion
{
    public Revive() : base("Revive", "A medicine that revives a fainted Pokémon.", 0) { }

    public override void Use(Pokemon pokemon)
    {
        if (pokemon.HP == 0)
        {
            pokemon.Heal(pokemon.MaxHP / 2);
        }
    }
}

public class MaxRevive : Potion
{
    public MaxRevive() : base("Max Revive", "A medicine that revives a fainted Pokémon with full health.", 0) { }

    public override void Use(Pokemon pokemon)
    {
        if (pokemon.HP == 0)
        {
            pokemon.Heal();
        }
    }
}

