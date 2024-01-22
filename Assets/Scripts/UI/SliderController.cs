using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject objectToActivate;

    public void OnSelect(BaseEventData eventData)
    {
        objectToActivate.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        objectToActivate.SetActive(false);
    }
    public void OnSliderChanged(float value)
    {
        Debug.Log("Slider value: " + value*10 + "%");
        AudioListener.volume = value/10;
        ButtonValueManager.Instance.volume = value/10;

        Debug.Log("BWM? value: " + ButtonValueManager.Instance.volume);
        objectToActivate.SetActive(true);
    }
}
