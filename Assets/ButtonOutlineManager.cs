using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonOutlineManager : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Selectable selectable;
    private Outline outline;

    void Start()
    {
        // Get the Selectable component attached to the button
        selectable = GetComponent<Selectable>();

        // Add an Outline component to the button if not already present
        outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false; // Initially, disable the outline
        }
    }

    // Called when the button is selected
    public void OnSelect(BaseEventData eventData)
    {
        // Enable the outline when the button is selected
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    // Called when the button is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        // Disable the outline when the button is deselected
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
