using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PokedexButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private Image caughtImage;
    [SerializeField] private Image spriteImage;
    [SerializeField] private PokemonDisplay pokemonDisplay;

    private PokemonBase pokemon;
    private bool isCaught;
    private bool isSeen;
    public void SetupButton(PokemonBase pokemonBase)
    {
        this.pokemon = pokemonBase;
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        if (pokemon != null)
        {
            isSeen = CheckIfSeen(pokemon);
            isCaught = CheckIfCaught(pokemon);
            idText.text = pokemon.PokedexNumber.ToString("D3");
            caughtImage.enabled = isCaught;
            spriteImage.sprite = pokemon.FrontSprite;
            spriteImage.enabled = isSeen;
        }
        else
        {
            Debug.LogError("Pokemon data is not set for the button.");
        }
    }

    private bool CheckIfCaught(PokemonBase pokemon)
    {
        return PokedexManager.Instance.IsPokemonCaught(pokemon.PokedexNumber);
    }
    private bool CheckIfSeen(PokemonBase pokemon)
    {
        return PokedexManager.Instance.IsPokemonSeen(pokemon.PokedexNumber);
    }
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Button selected");
        OnButtonSelected();
    }
    public void OnButtonSelected()
    {
        pokemonDisplay.DisplayPokemon(pokemon, isCaught, isSeen);
    }
}
