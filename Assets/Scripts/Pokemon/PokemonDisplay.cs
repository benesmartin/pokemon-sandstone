using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

public class PokemonDisplay : MonoBehaviour
{
    [SerializeField] private Image spriteImage;
    [SerializeField] private Image caughtImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI subnameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image type1Image;
    [SerializeField] private Image type2Image;
    [SerializeField] private PokemonBase firstPokemon;

    private Dictionary<PokemonType, Sprite> typeSprites = new Dictionary<PokemonType, Sprite>();
    private void Start()
    {
        LoadTypeSprites();
        DisplayPokemon(firstPokemon, true, true);
    }

    private void LoadTypeSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Types");
        foreach (Sprite sprite in sprites)
        {
            PokemonType type;
            var name = char.ToUpper(sprite.name[0]) + sprite.name.Substring(1);
            if (Enum.TryParse(name, out type))
            {
                typeSprites[type] = sprite;
            }
            else
            {
                Debug.Log("Sprite name does not match any PokemonType enum value: " + sprite.name);
            }
        }
    }
    public void DisplayPokemon(PokemonBase pokemon, bool isCaught, bool isSeen)
    {
        if (isCaught)
        {
            spriteImage.sprite = pokemon.FrontSprite;

            caughtImage.gameObject.SetActive(true);

            nameText.text = $"{pokemon.PokedexNumber:D3} {pokemon.Name}";
            subnameText.text = pokemon.SubName;

            type1Image.sprite = typeSprites[pokemon.Type1];
            type1Image.gameObject.SetActive(pokemon.Type1 != PokemonType.None);

            if (pokemon.Type2 == PokemonType.None)
            {
                type2Image.gameObject.SetActive(false);
            }
            else
            {
                type2Image.sprite = typeSprites[pokemon.Type2];
                type2Image.gameObject.SetActive(true);
            }

            descriptionText.text = pokemon.Description;
        }
        else if (isSeen)
        {
            spriteImage.sprite = pokemon.FrontSprite;

            caughtImage.gameObject.SetActive(false);

            nameText.text = $"{pokemon.PokedexNumber:D3} {pokemon.Name}";
            subnameText.text = "??? Pokemon";

            type1Image.gameObject.SetActive(false);
            type2Image.gameObject.SetActive(false);

            descriptionText.text = "";
        }
        else
        {
            spriteImage.sprite = Resources.Load<Sprite>("mark");

            caughtImage.gameObject.SetActive(false);

            nameText.text = "";
            subnameText.text = "";

            type1Image.gameObject.SetActive(false);
            type2Image.gameObject.SetActive(false);

            descriptionText.text = "";
        }
    }

}
