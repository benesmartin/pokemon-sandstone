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
         if(!MainBattleButtons.Instance) 
            eventSystem.SetSelectedGameObject(PauseMenu.Instance.GetBagButton());
         else 
         {
            GameObject itemScreen = ItemBagManager.Instance.GetItemScreen();
            if (itemScreen != null)
            {
                itemScreen.SetActive(false);
            }
            else
            {
                Debug.LogError("ItemScreen is null.");
            }
            MainBattleButtons.Instance.isItemScreenInBattle = false;
            MainBattleButtons.Instance.OnCancelButton();
        }
    }
}
