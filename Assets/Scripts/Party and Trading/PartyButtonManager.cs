using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyButtonManager : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpValue;
    [SerializeField] private Image fillImage;
    public Slider GetSlider()
    {
        return hpSlider;
    }
    public TextMeshProUGUI GetText()
    {
        return hpValue;
    }
    public Image GetImage()
    {
        return fillImage;
    }
}
