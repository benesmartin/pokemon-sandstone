using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpValue;
    [SerializeField] Image fillImage;
    public void SetData(Pokemon pokemon)
    {
        StopAllCoroutines();
        Debug.Log("[StatGUI SetData] Setting HP: " + pokemon.HP + " / " + pokemon.MaxHP);
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv. " + pokemon.Level;
        hpSlider.maxValue = pokemon.MaxHP;
        hpSlider.value = pokemon.HP;
        hpValue.text = pokemon.HP + "/" + pokemon.MaxHP;
        Canvas.ForceUpdateCanvases(); // Force the canvas to update
        UpdateFillColor(pokemon.HP, pokemon.MaxHP);
    }
    void Start()
    {
        fillImage = hpSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
    }

    public void UpdateHP(Pokemon pokemon)
    {
        StartCoroutine(UpdateHPCoroutine(pokemon));
    }

    IEnumerator UpdateHPCoroutine(Pokemon pokemon)
    {
        Debug.Log("[StatGUI UpdateHPCoroutine] Called for: " + pokemon.Base.Name + " with HP: " + pokemon.HP);
        while (hpSlider.value > pokemon.HP)
        {
            hpSlider.value--;
            hpValue.text = hpSlider.value + "/" + pokemon.MaxHP;

            UpdateFillColor((int)hpSlider.value, pokemon.MaxHP);

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

        //if (currentHP <= 0)
        //{
        //    fillImage.gameObject.SetActive(false);
        //}
        //else
        //{
        //    if (!fillImage.gameObject.activeSelf)
        //    {
        //        fillImage.gameObject.SetActive(true);
        //    }
        //}
    }
    private Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
