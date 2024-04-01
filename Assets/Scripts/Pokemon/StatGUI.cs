using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class StatGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpValue;
    [SerializeField] Image fillImage;
    [SerializeField] Slider levelSlider;
    [SerializeField] TextMeshProUGUI levelValue;
    [SerializeField] Image fillImageLevel;
    [SerializeField] GameObject expBar;
    public bool IsExpAnimationDone = false;
    Pokemon _pokemon;
    public void SetData(Pokemon pokemon, bool isPlayerPokemon)
    {
        Debug.Log("SetData called in StatGUI");
        Debug.Log("levelSlider assigned in SetData: " + (levelSlider != null));
        Debug.Log("levelValue assigned in SetData: " + (levelValue != null));
        _pokemon = pokemon;
        StopAllCoroutines();
        Debug.Log("[StatGUI SetData] Setting HP: " + pokemon.HP + " / " + pokemon.MaxHP);
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv. " + pokemon.Level;
        hpSlider.maxValue = pokemon.MaxHP;
        hpSlider.value = pokemon.HP;
        hpValue.text = pokemon.HP + "/" + pokemon.MaxHP;
        SetExp();
        Canvas.ForceUpdateCanvases();
        UpdateFillColor(pokemon.HP, pokemon.MaxHP);
    }
    void Awake()
    {
        Debug.Log("Start called in StatGUI");
        fillImage = hpSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
        Debug.Log("levelSlider assigned: " + (levelSlider != null));
        Debug.Log("levelValue assigned: " + (levelValue != null));
    }


    public void SetLevel()
    {
        levelText.text = "Lv. " + _pokemon.Level;
        StartCoroutine(FlashLevel(5));
    }
    public IEnumerator FlashLevel(int count)
    {
        for (int i = 0; i < count; i++)
        {
            levelText.color = new Color(i % 2, i % 2, i % 2, 1);
            yield return new WaitForSeconds(0.1f);
            levelText.color = new Color(76, 211, 255, 255);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetExp()
    {
        if (expBar == null)
        {
            Debug.Log("expBar is null in SetExp");
            return;
        }
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null)
        {
            Debug.Log("expBar is null in SetExp");
            yield break;
        }
        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currentLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);
        float normalizedExp = (float)(_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        Debug.Log("Current Level: " + _pokemon.Level + " Current Level Exp: " + currentLevelExp + " Next Level Exp: " + nextLevelExp + " Pokemon Exp: " + _pokemon.Exp + " Normalized Exp: " + Mathf.Clamp01(normalizedExp) + " Normalized Exp2 " + normalizedExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator UpdateHP(Pokemon pokemon)
    {
        yield return StartCoroutine(UpdateHPCoroutine(pokemon));
    }

    public IEnumerator UpdateHP(Pokemon pokemon, Slider hpSlider, TextMeshProUGUI hpValue, Image fillImage)
    {
        yield return StartCoroutine(UpdateHPCoroutine(pokemon, hpSlider, hpValue, fillImage));
    }

    IEnumerator UpdateHPCoroutine(Pokemon pokemon)
    {
        Debug.Log("[StatGUI UpdateHPCoroutine] Called for: " + pokemon.Base.Name + " with HP: " + pokemon.HP);
        while (hpSlider.value != pokemon.HP)
        {
            if (hpSlider.value < pokemon.HP)
                hpSlider.value++;
            else
                hpSlider.value--;
            hpValue.text = hpSlider.value + "/" + pokemon.MaxHP;
            Canvas.ForceUpdateCanvases();
            UpdateFillColor((int)hpSlider.value, pokemon.MaxHP);

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator UpdateHPCoroutine(Pokemon pokemon, Slider hpSlider, TextMeshProUGUI hpValue, Image fillImage)
    {
        Debug.Log("[StatGUI UpdateHPCoroutine] Called for: " + pokemon.Base.Name + " with HP: " + pokemon.HP);
        while (hpSlider.value != pokemon.HP)
        {
            if (hpSlider.value < pokemon.HP)
                hpSlider.value++;
            else
                hpSlider.value--;
            hpValue.text = hpSlider.value + "/" + pokemon.MaxHP;
            Canvas.ForceUpdateCanvases();
            UpdateFillColor((int)hpSlider.value, pokemon.MaxHP, fillImage);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateFillColor(int currentHP, int maxHP)
    {
        float hpPercentage = (float)currentHP / maxHP;

        Color highHPColor = HexToColor("#00C304");
        Color mediumHPColor = Color.yellow;
        Color lowHPColor = Color.red;

        if (hpPercentage > 0.5f)
        {
            fillImage.color = Color.Lerp(mediumHPColor, highHPColor, (hpPercentage - 0.5f) * 2);
        }
        else
        {
            fillImage.color = Color.Lerp(lowHPColor, mediumHPColor, hpPercentage * 2);
        }
    }

    private void UpdateFillColor(int currentHP, int maxHP, Image fillImage)
    {
        float hpPercentage = (float)currentHP / maxHP;

        Color highHPColor = HexToColor("#00C304");
        Color mediumHPColor = Color.yellow;
        Color lowHPColor = Color.red;

        if (hpPercentage > 0.5f)
        {
            fillImage.color = Color.Lerp(mediumHPColor, highHPColor, (hpPercentage - 0.5f) * 2);
        }
        else
        {
            fillImage.color = Color.Lerp(lowHPColor, mediumHPColor, hpPercentage * 2);
        }
    }
    private Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
