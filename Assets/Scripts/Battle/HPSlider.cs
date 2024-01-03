using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPSlider : MonoBehaviour
{
    public TextMeshProUGUI text;
    int progress = 0;
    public Slider slider;
    public void OnSliderChanged(float value)
    {
        text.text = value.ToString() + "/" + slider.maxValue;
    }
    public void UpdateProgress()
    {
        progress++;
        slider.value = progress;
    }
}
