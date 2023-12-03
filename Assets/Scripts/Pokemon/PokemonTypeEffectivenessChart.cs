using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonTypeEffectivenessChart : MonoBehaviour
{
    public static PokemonTypeEffectivenessChart Instance { get; private set; }

    private Dictionary<(PokemonType, PokemonType), float> typeEffectivenessChart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeChart();
        }
    }

    private void InitializeChart()
    {
        typeEffectivenessChart = new Dictionary<(PokemonType, PokemonType), float>
        {
            // Super Effective Fire
            { (PokemonType.Fire, PokemonType.Grass), 2.0f },
            { (PokemonType.Fire, PokemonType.Ice), 2.0f },
            { (PokemonType.Fire, PokemonType.Bug), 2.0f },
            { (PokemonType.Fire, PokemonType.Steel), 2.0f },
            // Super Effective Water
            { (PokemonType.Water, PokemonType.Fire), 2.0f },
            { (PokemonType.Water, PokemonType.Ground), 2.0f },
            { (PokemonType.Water, PokemonType.Rock), 2.0f },
            // Super Effective Grass
            { (PokemonType.Grass, PokemonType.Water), 2.0f },
            { (PokemonType.Grass, PokemonType.Ground), 2.0f },
            { (PokemonType.Grass, PokemonType.Rock), 2.0f },
            // Super Effective Electric
            { (PokemonType.Electric, PokemonType.Water), 2.0f },
            { (PokemonType.Electric, PokemonType.Flying), 2.0f },
            // Super Effective Ice
            { (PokemonType.Ice, PokemonType.Grass), 2.0f },
            { (PokemonType.Ice, PokemonType.Ground), 2.0f },
            { (PokemonType.Ice, PokemonType.Flying), 2.0f },
            { (PokemonType.Ice, PokemonType.Dragon), 2.0f },
            // Super Effective Fighting
            { (PokemonType.Fighting, PokemonType.Normal), 2.0f },
            { (PokemonType.Fighting, PokemonType.Ice), 2.0f },
            { (PokemonType.Fighting, PokemonType.Rock), 2.0f },
            { (PokemonType.Fighting, PokemonType.Dark), 2.0f },
            { (PokemonType.Fighting, PokemonType.Steel), 2.0f },
            // Super Effective Poison
            { (PokemonType.Poison, PokemonType.Grass), 2.0f },
            { (PokemonType.Poison, PokemonType.Fairy), 2.0f },
            // Super Effective Ground
            { (PokemonType.Ground, PokemonType.Fire), 2.0f },
            { (PokemonType.Ground, PokemonType.Electric), 2.0f },
            { (PokemonType.Ground, PokemonType.Poison), 2.0f },
            { (PokemonType.Ground, PokemonType.Rock), 2.0f },
            { (PokemonType.Ground, PokemonType.Steel), 2.0f },
            // Super Effective Flying
            { (PokemonType.Flying, PokemonType.Grass), 2.0f },
            { (PokemonType.Flying, PokemonType.Fighting), 2.0f },
            { (PokemonType.Flying, PokemonType.Bug), 2.0f },
            // Super Effective Psychic
            { (PokemonType.Psychic, PokemonType.Fighting), 2.0f },
            { (PokemonType.Psychic, PokemonType.Poison), 2.0f },
            // Super Effective Bug
            { (PokemonType.Bug, PokemonType.Grass), 2.0f },
            { (PokemonType.Bug, PokemonType.Psychic), 2.0f },
            { (PokemonType.Bug, PokemonType.Dark), 2.0f },
            // Super Effective Rock
            { (PokemonType.Rock, PokemonType.Fire), 2.0f },
            { (PokemonType.Rock, PokemonType.Ice), 2.0f },
            { (PokemonType.Rock, PokemonType.Flying), 2.0f },
            { (PokemonType.Rock, PokemonType.Bug), 2.0f },
            // Super Effective Ghost
            { (PokemonType.Ghost, PokemonType.Psychic), 2.0f },
            { (PokemonType.Ghost, PokemonType.Ghost), 2.0f },
            // Super Effective Dragon
            { (PokemonType.Dragon, PokemonType.Dragon), 2.0f },
            // Super Effective Dark
            { (PokemonType.Dark, PokemonType.Psychic), 2.0f },
            { (PokemonType.Dark, PokemonType.Ghost), 2.0f },
            // Super Effective Steel
            { (PokemonType.Steel, PokemonType.Ice), 2.0f },
            { (PokemonType.Steel, PokemonType.Rock), 2.0f },
            { (PokemonType.Steel, PokemonType.Fairy), 2.0f },
            // Super Effective Fairy
            { (PokemonType.Fairy, PokemonType.Fighting), 2.0f },
            { (PokemonType.Fairy, PokemonType.Dragon), 2.0f },
            { (PokemonType.Fairy, PokemonType.Dark), 2.0f },

            // Not Very Effective Normal
            { (PokemonType.Normal, PokemonType.Rock), 0.5f },
            { (PokemonType.Normal, PokemonType.Steel), 0.5f },
            // Not Very Effective Fire
            { (PokemonType.Fire, PokemonType.Fire), 0.5f },
            { (PokemonType.Fire, PokemonType.Water), 0.5f },
            { (PokemonType.Fire, PokemonType.Rock), 0.5f },
            { (PokemonType.Fire, PokemonType.Dragon), 0.5f },
            // Not Very Effective Water
            { (PokemonType.Water, PokemonType.Water), 0.5f },
            { (PokemonType.Water, PokemonType.Grass), 0.5f },
            { (PokemonType.Water, PokemonType.Dragon), 0.5f },
            // Not Very Effective Electric
            { (PokemonType.Electric, PokemonType.Electric), 0.5f },
            { (PokemonType.Electric, PokemonType.Grass), 0.5f },
            { (PokemonType.Electric, PokemonType.Dragon), 0.5f },
            // Not Very Effective Grass
            { (PokemonType.Grass, PokemonType.Fire), 0.5f },
            { (PokemonType.Grass, PokemonType.Grass), 0.5f },
            { (PokemonType.Grass, PokemonType.Poison), 0.5f },
            { (PokemonType.Grass, PokemonType.Flying), 0.5f },
            { (PokemonType.Grass, PokemonType.Bug), 0.5f },
            { (PokemonType.Grass, PokemonType.Dragon), 0.5f },
            { (PokemonType.Grass, PokemonType.Steel), 0.5f },
            // Not Very Effective Ice
            { (PokemonType.Ice, PokemonType.Fire), 0.5f },
            { (PokemonType.Ice, PokemonType.Water), 0.5f },
            { (PokemonType.Ice, PokemonType.Ice), 0.5f },
            { (PokemonType.Ice, PokemonType.Steel), 0.5f },
            // Not Very Effective Fighting
            { (PokemonType.Fighting, PokemonType.Poison), 0.5f },
            { (PokemonType.Fighting, PokemonType.Flying), 0.5f },
            { (PokemonType.Fighting, PokemonType.Psychic), 0.5f },
            { (PokemonType.Fighting, PokemonType.Bug), 0.5f },
            { (PokemonType.Fighting, PokemonType.Fairy), 0.5f },
            // Not Very Effective Poison
            { (PokemonType.Poison, PokemonType.Poison), 0.5f },
            { (PokemonType.Poison, PokemonType.Ground), 0.5f },
            { (PokemonType.Poison, PokemonType.Rock), 0.5f },
            { (PokemonType.Poison, PokemonType.Ghost), 0.5f },
            // Not Very Effective Ground
            { (PokemonType.Ground, PokemonType.Grass), 0.5f },
            { (PokemonType.Ground, PokemonType.Bug), 0.5f },
            // Not Very Effective Flying
            { (PokemonType.Flying, PokemonType.Electric), 0.5f },
            { (PokemonType.Flying, PokemonType.Rock), 0.5f },
            { (PokemonType.Flying, PokemonType.Steel), 0.5f },
            // Not Very Effective Psychic
            { (PokemonType.Psychic, PokemonType.Psychic), 0.5f },
            { (PokemonType.Psychic, PokemonType.Steel), 0.5f },
            // Not Very Effective Bug
            { (PokemonType.Bug, PokemonType.Fire), 0.5f },
            { (PokemonType.Bug, PokemonType.Fighting), 0.5f },
            { (PokemonType.Bug, PokemonType.Poison), 0.5f },
            { (PokemonType.Bug, PokemonType.Flying), 0.5f },
            { (PokemonType.Bug, PokemonType.Ghost), 0.5f },
            { (PokemonType.Bug, PokemonType.Steel), 0.5f },
            { (PokemonType.Bug, PokemonType.Fairy), 0.5f },
            // Not Very Effective Rock
            { (PokemonType.Rock, PokemonType.Fighting), 0.5f },
            { (PokemonType.Rock, PokemonType.Ground), 0.5f },
            { (PokemonType.Rock, PokemonType.Steel), 0.5f },
            // Not Very Effective Ghost
            { (PokemonType.Ghost, PokemonType.Dark), 0.5f },
            // Not Very Effective Dragon
            { (PokemonType.Dragon, PokemonType.Steel), 0.5f },
            // Not Very Effective Dark
            { (PokemonType.Dark, PokemonType.Fighting), 0.5f },
            { (PokemonType.Dark, PokemonType.Dark), 0.5f },
            { (PokemonType.Dark, PokemonType.Fairy), 0.5f },
            // Not Very Effective Steel
            { (PokemonType.Steel, PokemonType.Fire), 0.5f },
            { (PokemonType.Steel, PokemonType.Water), 0.5f },
            { (PokemonType.Steel, PokemonType.Electric), 0.5f },
            { (PokemonType.Steel, PokemonType.Steel), 0.5f },
            // Not Very Effective Fairy
            { (PokemonType.Fairy, PokemonType.Fire), 0.5f },
            { (PokemonType.Fairy, PokemonType.Poison), 0.5f },
            { (PokemonType.Fairy, PokemonType.Steel), 0.5f },

            // Immune Normal
            { (PokemonType.Normal, PokemonType.Ghost), 0.0f },
            // Immune Electric
            { (PokemonType.Electric, PokemonType.Ground), 0.0f },
            // Immune Fighting
            { (PokemonType.Fighting, PokemonType.Ghost), 0.0f },
            // Immune Poison
            { (PokemonType.Poison, PokemonType.Steel), 0.0f },
            // Immune Ground
            { (PokemonType.Ground, PokemonType.Flying), 0.0f },
            // Immune Psychic
            { (PokemonType.Psychic, PokemonType.Dark), 0.0f },
            // Immune Ghost
            { (PokemonType.Ghost, PokemonType.Normal), 0.0f },
            // Immune Dragon
            { (PokemonType.Dragon, PokemonType.Fairy), 0.0f },
        };
    }

    public float GetEffectiveness(PokemonType attackerType, PokemonType defenderType)
    {
        if (typeEffectivenessChart.TryGetValue((attackerType, defenderType), out float effectiveness))
        {
            return effectiveness;
        }
        return 1.0f; // Default effectiveness
    }
}
