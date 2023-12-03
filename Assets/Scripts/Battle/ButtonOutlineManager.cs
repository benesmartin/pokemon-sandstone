using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonOutlineManager : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Selectable selectable;
    private Outline outline;

    void Start()
    {
        selectable = GetComponent<Selectable>();

        outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
