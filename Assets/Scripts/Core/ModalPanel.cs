using System;
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

    private void InitializeButtons()
    {
        InitializeButton(yesButton, false);
        InitializeButton(okayButton, false);
        InitializeButton(noButton, false);
    }

    private void InitializeButton(Button button, bool isActive)
    {
        button.gameObject.SetActive(isActive);
        button.GetComponent<Outline>().enabled = false;
        button.onClick.RemoveAllListeners();
    }

    public void ShowOkay(EventSystem eventSystem, string message, Action onOkay)
    {
        InitializeButtons();
        ConfigureModal(eventSystem, ModalType.Okay, message);
        AddListener(okayButton, () => { onOkay?.Invoke(); Hide(); });
    }

    public void ShowTextOnly(string message)
    {
        InitializeButtons();
        ConfigureModal(null, ModalType.TextOnly, message);
    }
    public void Show(EventSystem eventSystem, string message, Action onYes, Action onNo)
    {
        InitializeButtons();
        ConfigureModal(eventSystem, ModalType.YesNo, message);
        AddListener(yesButton, () => { onYes?.Invoke(); Hide(); });
        AddListener(noButton, () => { onNo?.Invoke(); Hide(); });
    }

    private void ConfigureModal(EventSystem eventSystem, ModalType modalType, string message)
    {
        yesButton.gameObject.SetActive(modalType == ModalType.YesNo);
        noButton.gameObject.SetActive(modalType == ModalType.YesNo);
        okayButton.gameObject.SetActive(modalType == ModalType.Okay);

        Button selectedButton = modalType == ModalType.YesNo ? yesButton : okayButton;
        if (eventSystem != null && selectedButton.gameObject.activeSelf)
        {
            eventSystem.SetSelectedGameObject(selectedButton.gameObject);
            selectedButton.GetComponent<Outline>().enabled = true;
        }

        messageText.text = message;
        panel.SetActive(true);
    }

    private void AddListener(Button button, Action action)
    {
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(action));
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private enum ModalType
    {
        YesNo,
        Okay,
        TextOnly
    }
}
