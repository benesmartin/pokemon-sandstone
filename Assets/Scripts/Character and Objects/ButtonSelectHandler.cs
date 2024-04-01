using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectHandler : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        if (gameObject.tag == "CategoryButton")
        {
            ShowCategoryInfo();
        }
        else if (gameObject.tag == "ItemButton")
        {
            ShowItemDescriptionPlaceholder();
        }
    }

    private void ShowCategoryInfo()
    {
        Debug.Log("Category Selected: " + gameObject.name);
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
    }

    private void ShowItemDescriptionPlaceholder()
    {
        Debug.Log("Item Selected: " + gameObject.name);
        PauseMenu.Instance.currentItem = gameObject.GetComponent<UnityEngine.UI.Button>();
        ItemHolder itemHolder = gameObject.GetComponent<ItemHolder>();
        if (itemHolder != null && itemHolder.Item != null)
        {
            PauseMenu.Instance.OnItemSelect(itemHolder.Item);
        }
        else
        {
            Debug.LogError("ItemHolder component or Item is missing!");
        }
    }
}
