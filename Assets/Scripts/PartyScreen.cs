using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private GameObject partyButton;
    private EventSystem eventSystem;
    void Awake()
    {
        eventSystem = UIFocusManager.Instance.eventSystem;
    }
    public void OnCancelPartyButton()
    {
        eventSystem.SetSelectedGameObject(partyButton);
    }
}
