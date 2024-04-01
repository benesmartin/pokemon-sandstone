using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour
{
    [SerializeField] public Image mapImage;
    [SerializeField] public Button toggleButton;
    bool cities = true;
    public void ToggleCities()
    {
        cities = !cities;
        mapImage.sprite = cities ? Resources.Load<Sprite>("Maps/ejiputo-map-full") : Resources.Load<Sprite>("Maps/ejiputo-map-no-cities");
        toggleButton.image.color = cities ? HexToColor("#77dd77") : HexToColor("#ff6961");
    }
    private Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
