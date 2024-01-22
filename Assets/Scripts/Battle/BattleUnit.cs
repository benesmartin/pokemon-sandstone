using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] GameObject pokeball;
    [SerializeField] Image trainer;
    [SerializeField] Sprite trainer2;
    [SerializeField] Sprite trainer3;
    public Pokemon Pokemon { get; private set; }
    private RectTransform rectTransform;
    Image image;
    Vector3 originalPos;
    private bool initialAnimationPlayed = false;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        rectTransform = GetComponent<RectTransform>();
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayEnterAnimation();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayAttackAnimation();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayFaintAnimation();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (isPlayerUnit) Pokemon.HP = 5000;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            BattleSystem.Instance.Shakes = 0;
            Debug.Log("Shakes: " + BattleSystem.Instance.Shakes);


        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            BattleSystem.Instance.Shakes = 1;
            Debug.Log("Shakes: " + BattleSystem.Instance.Shakes);

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BattleSystem.Instance.Shakes = 2;
            Debug.Log("Shakes: " + BattleSystem.Instance.Shakes);

        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            BattleSystem.Instance.Shakes = 3;
            Debug.Log("Shakes: " + BattleSystem.Instance.Shakes);

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            BattleSystem.Instance.Shakes = 4;
            Debug.Log("Shakes: " + BattleSystem.Instance.Shakes);

        }
    }
    public void Setup(Pokemon pokemon)
    {
        Debug.Log("[PlayerUnit] Setup called for" + pokemon.Base.Name + ", Pokémon HP: " + pokemon.HP);
        Pokemon = pokemon;
        image.sprite = Pokemon.Base.BackSprite;
        if (PokemonParty.Instance.IsWildBattle)
        {
            if (!initialAnimationPlayed) StartCoroutine(PlayWildBattleEnterAnimation());
            else PlayEnterAnimation();
        }
        else
        {
            if (!initialAnimationPlayed) StartCoroutine(StartAnimation());
            else PlayEnterAnimation();
        }
    }
    private IEnumerator PlayWildBattleEnterAnimation()
    {
        TweenerCore<Vector2, Vector2, VectorOptions> wildPokemonMove = null;
        if (!isPlayerUnit)
        { 
            
            var wildPokemonImage = image; 
            image.DOFade(1f, 0f);
            
            wildPokemonImage.rectTransform.anchoredPosition = new Vector2(-2400, -574); 
            wildPokemonMove = wildPokemonImage.rectTransform.DOAnchorPos(new Vector2(-1058, -574), 3f).SetEase(Ease.OutSine);
        }

        Coroutine trainerMove = null;
        
        if (isPlayerUnit)
        {
            var trainerImage = trainer; 
            trainerImage.rectTransform.anchoredPosition = new Vector2(1269, -294); 

            
            trainerMove = StartCoroutine(MoveTrainerToBattlePosition(trainerImage));
        }

        
        yield return wildPokemonMove?.WaitForCompletion();

        
        if (trainerMove != null)
        {
            yield return trainerMove;
        }

        
        yield return new WaitForSeconds(2f); 

        
        if (isPlayerUnit)
        {
            yield return StartCoroutine(MoveTrainerOffScreen(trainer));
        }

        initialAnimationPlayed = true;
    }

    private IEnumerator MoveTrainerToBattlePosition(Image trainerImage)
    {
        
        yield return trainerImage.rectTransform.DOAnchorPos(new Vector2(-500, -294), 3f).SetEase(Ease.OutSine).WaitForCompletion();
    }



    private IEnumerator StartAnimation()
    {
        if (!isPlayerUnit) this.image.DOFade(0f, 0f);
        var image = trainer; 
                         
        if (isPlayerUnit)
        {
            
            image.rectTransform.anchoredPosition = new Vector2(1269, -294); 
                                                                            
            yield return image.rectTransform.DOAnchorPos(new Vector2(-500, -294), 3f).SetEase(Ease.OutSine).WaitForCompletion();

            yield return new WaitForSeconds(3f);
            
            yield return StartCoroutine(MoveTrainerOffScreen(image));
        }
        else
        {
            
            image.rectTransform.anchoredPosition = new Vector2(-2400, -468); 
                                                                             
            yield return image.rectTransform.DOAnchorPos(new Vector2(-1058, -468), 3f).SetEase(Ease.OutSine).WaitForCompletion();
            yield return new WaitForSeconds(1f);
            MoveFoeOffScreen(image);
        }
        initialAnimationPlayed = true;
    }

    private IEnumerator MoveTrainerOffScreen(Image image)
    {
        
        image.sprite = trainer2;
        PlayEnterAnimation();
        
        float halfwayDuration = 1.5f; 
        float endX = -1250f;

        
        Tween moveTween = image.rectTransform.DOAnchorPos(new Vector2(endX, image.rectTransform.anchoredPosition.y), 3f).SetEase(Ease.InOutSine);

        
        yield return new WaitForSeconds(halfwayDuration);
        image.sprite = trainer3;

        
        yield return new WaitForSeconds(halfwayDuration);
    }

    private void MoveFoeOffScreen(Image image)
    {
        if (isPlayerUnit) return;
        this.image.DOFade(1f, 0.5f);
        
        float endX = -150f;

        
        Tween moveTween = image.rectTransform.DOAnchorPos(new Vector2(endX, image.rectTransform.anchoredPosition.y), 2f).SetEase(Ease.InOutSine);
    }



    public void PlayEnterAnimation()
    {
        if (!isPlayerUnit) return;
        image.sprite = Pokemon.Base.BackSprite;
        image.DOFade(1f, 0);
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-1100, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(-110, originalPos.y);
        }
        float duration = initialAnimationPlayed ? 1.5f : 3f;
        float rotation = 0.4f;
        float rotationSpeed = isPlayerUnit ? -360f : 360f;


        var rotationTween = rectTransform.DORotate(new Vector3(0f, 0f, rotationSpeed), rotation, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);


        var moveTween = rectTransform.DOAnchorPos3D(new Vector2(-300, originalPos.y), duration).OnComplete(() =>
        {

            rotationTween.Kill();


            rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);


            image.sprite = Pokemon.Base.FrontSprite;
        });


        moveTween.Play();

    }
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(-300 + 50f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        sequence.Append(image.transform.DOLocalMoveX(isPlayerUnit ? -300 : originalPos.x, 0.25f));
    }
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
    private IEnumerator MoveAndRotateImage(bool isPlayerUnit)
    {
        Vector3 startPosition = rectTransform.anchoredPosition3D;
        Vector3 endPosition = isPlayerUnit
            ? new Vector3(-292f, -159f, 2.86f)
            : new Vector3(-1050f, -574f, 2.86f);
        float duration = 2f;
        float elapsed = 0f;
        float rotationAmount = isPlayerUnit ? -5f : 5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;

            rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, endPosition, normalizedTime);

            rectTransform.Rotate(0f, 0f, rotationAmount * Time.deltaTime / duration * 360f);

            yield return null;
        }

        rectTransform.anchoredPosition3D = endPosition;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        image.sprite = Pokemon.Base.FrontSprite;
    }
    public void PlayPokeballThrowAnimation(int numShakes, Action onAnimationComplete)
    {

        pokeball.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        image.color = new Color(1f, 1f, 1f, 1f);
        Vector3 pokeballStartPosLocal = new Vector3(-1148f, -121f, 0);
        Vector3 midAirPosLocal = new Vector3(188f, 333f, 0);
        Vector3 endPosLocal = new Vector3(188f, 20f, 0);

        pokeball.transform.localPosition = pokeballStartPosLocal;

        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOLocalMove(midAirPosLocal, 1f))
                .AppendCallback(() => StartCoroutine(FadeOutPokemon()))
                .Append(pokeball.transform.DOLocalMove(endPosLocal, 0.5f));

        if (numShakes == 0)
        {
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => StartCoroutine(BreakOutPokemon()));
        }

        int actualShakes = (numShakes == 4) ? 3 : numShakes;
        for (int i = 1; i <= actualShakes; i++)
        {

            sequence.Append(pokeball.transform.DORotate(new Vector3(0, 0, -45f), 0.2f));

            sequence.Append(pokeball.transform.DORotate(new Vector3(0, 0, 45f), 0.2f));

            sequence.Append(pokeball.transform.DORotate(Vector3.zero, 0.2f))
                    .AppendInterval(1f);

            if (i == numShakes)
            {
                sequence.AppendCallback(() => StartCoroutine(BreakOutPokemon()));
                break;
            }
        }

        if (numShakes == 4)
        {

            sequence.Append(pokeball.GetComponent<Image>().DOColor(new Color(0.5f, 0.5f, 0.5f, 1f), 0.5f));
        }
        sequence.OnComplete(() =>
        {
            if (onAnimationComplete != null)
                onAnimationComplete();
        });

        sequence.Play();
    }



    private IEnumerator FadeOutPokemon()
    {
        if (isPlayerUnit) yield break;
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            image.color = new Color(1f, 1f, 1f, 1f - normalizedTime);
            yield return null;
        }
    }

    private IEnumerator FadeInPokemon()
    {
        if (isPlayerUnit) yield break;
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            image.color = new Color(1f, 1f, 1f, normalizedTime);
            yield return null;
        }
    }
    private IEnumerator FadeOutPokeball()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Image pokeballSprite = pokeball.GetComponent<Image>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            pokeballSprite.color = new Color(1f, 1f, 1f, 1f - normalizedTime);
            yield return null;
        }

        pokeballSprite.color = new Color(1f, 1f, 1f, 0f);
    }
    private IEnumerator BreakOutPokemon()
    {

        StartCoroutine(FadeOutPokeball());
        StartCoroutine(FadeInPokemon());
        yield break;
    }

}