using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;


public class MainMenu : MonoBehaviour
{
    public static Color color = hexToColor("#706dda");
    public static Color darkerHighlightedColor = hexToColor("#3732c6");
    public static Color defaultDarkerColor = hexToColor("#666666");

    void Start()
    {
        Debug.Log("ButtonValueManger value: " + ButtonValueManager.Instance.volume);
        Button[] allButtons = GameObject.FindObjectsOfType<Button>();

        if (ButtonValueManager.Instance.buttonHighlights != null)
        {
            foreach (var button in allButtons)
            {
                if (ButtonValueManager.Instance.buttonHighlights.TryGetValue(button.name, out bool isHighlighted) && isHighlighted)
                {
                    Debug.Log("Button: " + button.name + " was highlighted, setting colors");
                    var clickedButtonColors = button.colors;
                    clickedButtonColors.normalColor = color;
                    clickedButtonColors.selectedColor = darkerHighlightedColor;
                    clickedButtonColors.pressedColor = darkerHighlightedColor;
                    button.colors = clickedButtonColors;
                }
            }
        }

        var slider = GameObject.FindObjectOfType<Slider>();

        if (slider != null)
        {
            slider.gameObject.SetActive(true);

            slider.value = ButtonValueManager.Instance.volume * 10;
            Debug.Log("BWM? value: " + ButtonValueManager.Instance.volume);
            Debug.Log("Slider name: " + slider.name);
            Debug.Log("Slider value: " + slider.value);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
    public void SelectOptions()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Text Speed Slow"));
    }
    public void SelectMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Play Button"));
    }
    public void HighlightOption(Selectable clickedButton)
    {
        string category = "";

        if (clickedButton.name.Contains("Text Speed"))
        {
            category = "Text Speed";
        }
        else if (clickedButton.name.Contains("Battle Style"))
        {
            category = "Battle Style";
        }
        else if (clickedButton.name.Contains("Battle Scene"))
        {
            category = "Battle Scene";
        }

        if (!string.IsNullOrEmpty(category))
        {
            ClearHighlights(category);
            ButtonValueManager.Instance.buttonHighlights[clickedButton.name] = true;
            Button[] categoryButtons = GameObject.FindObjectsOfType<Button>()
                .Where(button => button.name.Contains(category))
                .ToArray();

            foreach (var button in categoryButtons)
            {
                var colors = button.colors;
                colors.normalColor = Color.white;
                colors.selectedColor = defaultDarkerColor;
                colors.pressedColor = defaultDarkerColor;
                button.colors = colors;
            }

            var clickedButtonColors = clickedButton.colors;
            clickedButtonColors.normalColor = color;
            clickedButtonColors.selectedColor = darkerHighlightedColor;
            clickedButtonColors.pressedColor = darkerHighlightedColor;
            clickedButton.colors = clickedButtonColors;

            if (category == "Text Speed")
            {
                if (clickedButton.name.Contains("Slow"))
                {
                    ButtonValueManager.Instance.selectedTextSpeed = "Slow";
                }
                else if (clickedButton.name.Contains("Mid"))
                {
                    ButtonValueManager.Instance.selectedTextSpeed = "Mid";
                }
                else if (clickedButton.name.Contains("Fast"))
                {
                    ButtonValueManager.Instance.selectedTextSpeed = "Fast";
                }
            }
            else if (clickedButton.name.Contains("Battle Style"))
            {
                if (clickedButton.name.Contains("Shift"))
                {
                    ButtonValueManager.Instance.selectedBattleStyle = "Shift";
                }
                else if (clickedButton.name.Contains("Set"))
                {
                    ButtonValueManager.Instance.selectedBattleStyle = "Set";
                }
            }
            else if (clickedButton.name.Contains("Battle Scene"))
            {
                if (clickedButton.name.Contains("On"))
                {
                    ButtonValueManager.Instance.isBattleSceneOn = true;
                }
                else if (clickedButton.name.Contains("Off"))
                {
                    ButtonValueManager.Instance.isBattleSceneOn = false;
                }
            }
            Debug.Log("Selected " + category + ": " + clickedButton.name + " - Singleton: " + ButtonValueManager.Instance.selectedTextSpeed + ", " + ButtonValueManager.Instance.selectedBattleStyle + ", " + ButtonValueManager.Instance.isBattleSceneOn.ToString());
            Debug.Log("Colors: " + clickedButton.colors.normalColor + ", " + clickedButton.colors.selectedColor + ", " + clickedButton.colors.pressedColor);
        }
    }
    private void ClearHighlights(string category)
    {
        Button[] categoryButtons = GameObject.FindObjectsOfType<Button>()
            .Where(button => button.name.Contains(category))
            .ToArray();

        foreach (var button in categoryButtons)
        {
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.selectedColor = defaultDarkerColor;
            colors.pressedColor = defaultDarkerColor;
            button.colors = colors;

            ButtonValueManager.Instance.buttonHighlights.Remove(button.name);
        }
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
    public void PlaySFX(AudioClip clip)
    {
        BGmusic.instance.audioSource.PlayOneShot(clip);
    }
    public void PlaySong(AudioClip song)
    {
        BGmusic.instance.audioSource.Stop();
        BGmusic.instance.audioSource.clip = song;
        BGmusic.instance.audioSource.Play();
    }

}
