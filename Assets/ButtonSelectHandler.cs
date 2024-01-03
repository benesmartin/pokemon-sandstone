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
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
    }
}
