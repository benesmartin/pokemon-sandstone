using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemScreen : MonoBehaviour
{
    [SerializeField] private GameObject itemButton;
    private EventSystem eventSystem;
    void Awake()
    {
        eventSystem = UIFocusManager.Instance.eventSystem;
    }
    public void OnCancelPartyButton()
    {
        eventSystem.SetSelectedGameObject(itemButton);
    }
}
