using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class Pokeball : Item
{
    public double CatchRateModifier { get; private set; }

    public Pokeball(string name, string description, double catchRateModifier)
        : base(name, description, ItemCategory.Pokeball)
    {
        CatchRateModifier = catchRateModifier;
    }
    public string GetSpriteName()
    {
        return Regex.Replace(Name.ToLower(), @"\s+", "");
    }
}
public class StandardPokeball : Pokeball
{
    public StandardPokeball() : base("Poke Ball", "It has a simple red and white design, and it's the most known kind of Poke Ball.", 1.0) { }
}
public class GreatBall : Pokeball
{
    public GreatBall() : base("Great Ball", "It is slightly better than the regular Poke Ball.", 1.5) { }
}
public class UltraBall : Pokeball
{
    public UltraBall() : base("Ultra Ball", "It is twice as good as a regular Poke Ball.", 2.0) { }
}
public class MasterBall : Pokeball
{
    public MasterBall() : base("Master Ball", "A very rare Poke Ball that never fails in an attempt to catch a Pokemon.", double.PositiveInfinity) { }
}
public class LureBall : Pokeball
{
    public LureBall() : base("Lure Ball", "A ball that works better with Pokemon found by fishing.", 4.0) { }
}
public class DuskBall : Pokeball
{
    public DuskBall() : base("Dusk Ball", "A ball that works better in dark places or at night.", 3.0) { }
}
public class QuickBall : Pokeball
{
    public QuickBall() : base("Quick Ball", "A kind of Poke Ball that works better the sooner it is used in battle.", 5.0) { }
}
public class FriendBall : Pokeball
{
    public FriendBall() : base("Friend Ball", "It has the same chance to catch as a regular Poke Ball but makes the Pokemon more friendly to the trainer as soon as it is caught.", 1.0) { }
}
public class HealBall : Pokeball
{
    public HealBall() : base("Heal Ball", "A regular Poke Ball that heals a Pokemon as soon as it is caught.", 1.0) { }
}
public class HeavyBall : Pokeball
{
    public HeavyBall() : base("Heavy Ball", "A ball whose catch rate increases as the weight of the targeted Pokemon does.", 1.0) { }
}
public class LevelBall : Pokeball
{
    public LevelBall() : base("Level Ball", "Effectiveness increases with the level gap between your and the wild Pokémon.", 1.0) { }
}
public class LightBall : Pokeball
{
    public LightBall() : base("Light Ball", "A ball whose catch rate decreases as the weight of the targeted Pokemon does.", 1.0) { }
}
public class LoveBall : Pokeball
{
    public LoveBall() : base("Love Ball", "A ball that works better if the trainer's Pokemon and the wild Pokémon have opposite genders.", 8.0) { }
}
public class LuxuryBall : Pokeball
{
    public LuxuryBall() : base("Luxury Ball", "Doubles the rate that the caught Pokemon’s friendship level increases.", 1.0) { }
}
public class MoonBall : Pokeball
{
    public MoonBall() : base("Moon Ball", "The potential of this ball increases if the Pokemon attempted to catch can evolve by a Moon Stone.", 4.0) { }
}
public class NestBall : Pokeball
{
    public NestBall() : base("Nest Ball", "A kind of ball that becomes better if the level of the targeted Pokemon is lower.", 1.0) { }
}
public class NetBall : Pokeball
{
    public NetBall() : base("Net Ball", "A Poke Ball that works better with Bug and Water-type Pokemon.", 1.0) { }
}
public class PremierBall : Pokeball
{
    public PremierBall() : base("Premier Ball", "A somewhat rare Poke Ball that was made as a commemorative item used to celebrate an event of some sort.", 1.0) { }
}
public class RepeatBall : Pokeball
{
    public RepeatBall() : base("Repeat Ball", "A ball that works better if the trainer has the Pokemon already registered in their PokeDex.", 3.5) { }
}
public class TimerBall : Pokeball
{
    public TimerBall() : base("Timer Ball", "A type of ball that works better as more time passes since an encounter with a wild Pokemon.", 4.0) { }
}
public class DiveBall : Pokeball
{
    public DiveBall() : base("Dive Ball", "A type of ball that works better with Pokemon found underwater.", 3.5) { }
}
public class FastBall : Pokeball
{
    public FastBall() : base("Fast Ball", "A kind of Poke Ball that works better with Pokémon that like to flee from trainers.", 4.0) { }
}