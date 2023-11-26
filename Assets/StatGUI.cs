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
    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lv. " + pokemon.Level;
        hpSlider.maxValue = pokemon.MaxHP;
        hpSlider.value = pokemon.HP;
        hpValue.text = pokemon.HP + "/" + pokemon.MaxHP;
    }
}
