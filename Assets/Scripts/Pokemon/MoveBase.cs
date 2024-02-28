using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] bool isSpecial;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;


    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int PP
    {
        get { return pp; }
    }

    public bool IsSpecial
    {
        get { return isSpecial; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    [System.Serializable]
    public class MoveEffects
    {
        [SerializeField] List<StatBoost> statBoosts;
        [SerializeField] ConditionID status;
        public List<StatBoost> StatBoosts
        {
            get { return statBoosts; }
        }
        public ConditionID Status
        {
            get { return status; }
        }
    }
    [System.Serializable]
    public class StatBoost
    {
        public Stat stat;
        public int boost;
    }
    public enum MoveCategory
    {
        Physical, Special, Status
    }
}
