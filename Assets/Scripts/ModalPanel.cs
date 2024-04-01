using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModalPanel : MonoBehaviour
{
    public GameObject panel;
    public Button yesButton;
    public Button okayButton;
    public Button noButton;
    public TextMeshProUGUI messageText;

    public void Show(EventSystem eventSystem, string message, Action onYes, Action onNo)
    {
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        okayButton.gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(yesButton.gameObject);
        yesButton.GetComponent<Outline>().enabled = true;   
        messageText.text = message;
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { onYes?.Invoke(); Hide(); });
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => { onNo?.Invoke(); Hide(); });

        panel.SetActive(true);
    }

    public void ShowOkay(EventSystem eventSystem, string message, Action onOkay)
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        okayButton.gameObject.SetActive(true);
        okayButton.GetComponent<Outline>().enabled = true;
        eventSystem.SetSelectedGameObject(okayButton.gameObject);
        messageText.text = message;
        okayButton.onClick.RemoveAllListeners();
        okayButton.onClick.AddListener(() => { onOkay?.Invoke(); Hide(); });

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
