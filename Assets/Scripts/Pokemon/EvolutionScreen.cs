using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionScreen : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Sprite previousSprite;
    [SerializeField] Sprite nextSprite;
    [SerializeField] Image spriteRenderer;
    [SerializeField] Pokemon debugPokemon;
    [SerializeField] PokemonBase debugEvolvedPokemon;
    public static EvolutionScreen Instance { get; private set; }
    private Pokemon pokemon;
    private PokemonBase evolvedPokemon;

    public bool IsFinished { get => isFinished; set => isFinished = value; }
    private bool isFinished = false;
    private void Awake()
    {
        Instance = this;
    }
    public void Show(Pokemon pokemon, PokemonBase evolvedPokemon)
    {
        IsFinished = false;
        this.pokemon = pokemon;
        this.evolvedPokemon = evolvedPokemon;
        previousSprite = pokemon.Base.FrontSprite;
        nextSprite = evolvedPokemon.FrontSprite;
        StartCoroutine(EvolveAnimation());
    }
    public IEnumerator EvolveAnimation()
    {
        var pokemonName = pokemon.Base.Name;
        var evolvedPokemonName = evolvedPokemon.Name;
        spriteRenderer.sprite = previousSprite;
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog("What? " + pokemonName + " is evolving!");
        yield return WaitForDialogueBox();
        yield return new WaitForSeconds(1f);
        spriteRenderer.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.black;
        spriteRenderer.DOFade(1, 0.5f);

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            sequence.Append(spriteRenderer.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.5f));
            sequence.AppendCallback(() =>
            {
                spriteRenderer.color = Color.black;
                spriteRenderer.sprite = nextSprite;
            });
            sequence.Append(spriteRenderer.transform.DOScale(Vector3.one, 0.5f));
            sequence.AppendCallback(() =>
            {
                spriteRenderer.color = Color.black;
                spriteRenderer.sprite = previousSprite;
            });
        }
        sequence.Append(spriteRenderer.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.5f));
        sequence.AppendCallback(() =>
        {
            spriteRenderer.color = Color.black;
            spriteRenderer.sprite = nextSprite;
        });
        sequence.Append(spriteRenderer.transform.DOScale(Vector3.one, 0.5f));
        sequence.Play();

        yield return sequence.WaitForCompletion();

        spriteRenderer.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
        spriteRenderer.DOFade(1, 0.5f);

        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"Congratulations! Your {pokemonName} evolved into {evolvedPokemonName}!");
        yield return WaitForDialogueBox();
        yield return new WaitForSeconds(1f);

        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{evolvedPokemonName}'s data was added to the Pokedex!");
        yield return WaitForDialogueBox();
        yield return new WaitForSeconds(2f);
        IsFinished = true;
    }

    private IEnumerator WaitForDialogueBox()
    {
        while (dialogBox.GetComponent<BattleDialogBox>().isTyping)
        {
            yield return null;
        }
    }
}
