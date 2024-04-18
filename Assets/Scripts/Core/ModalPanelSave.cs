using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalPanelSave : MonoBehaviour
{
    [SerializeField] public GameObject saveButton1;
    [SerializeField] GameObject saveButton2;
    [SerializeField] GameObject saveButton3;
    public void Init()
    {
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(saveButton1);
        foreach (var button in new[] { saveButton1, saveButton2, saveButton3 })
        {
            SetButton(button);
        }
    }
    public void SetButton(GameObject button)
    {
        // we need to set the text of children: Location, Pokedex, Time, Player, Badges:
        //rewrite button name from "Save 1" to "1";
        int slot = int.Parse(button.name.Substring(button.name.Length - 1));
        if (SaveSystem.SaveExists(slot))
        {
            SaveData data = SaveSystem.LoadGame(slot);
            button.transform.Find("Location").GetComponent<TextMeshProUGUI>().text = "Location: " + data.location.SetColor("#3d6ea2").SetBold();
            button.transform.Find("Pokedex").GetComponent<TextMeshProUGUI>().text = "Pokedex: " + data.pokedex.SetColor("#3d6ea2").SetBold();
            button.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = "Time: " + data.time.SetColor("#3d6ea2").SetBold();
            button.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Player: " + data.player.SetColor("#3d6ea2").SetBold();
            button.transform.Find("Badges").GetComponent<TextMeshProUGUI>().text = "Badges: " + data.badges.SetColor("#3d6ea2").SetBold();
        }
        else
        {// pastel red hex: #ff6666
            button.transform.Find("Location").GetComponent<TextMeshProUGUI>().text = "Location: " + "---".SetColor("#ff6666");
            button.transform.Find("Pokedex").GetComponent<TextMeshProUGUI>().text = "Pokedex: " + "---".SetColor("#ff6666");
            button.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = "Time: " + "---".SetColor("#ff6666");
            button.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "Player: " + "---".SetColor("#ff6666");
            button.transform.Find("Badges").GetComponent<TextMeshProUGUI>().text = "Badges: " + "---".SetColor("#ff6666");
        }
    }
}
