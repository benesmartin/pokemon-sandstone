using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollAdjuster : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float yDecrement = 155f;
    public float topBuffer = -5f;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.transform.IsChildOf(scrollRect.content.transform))
        {
            AdjustScrollPosition(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
        }
    }

    private void AdjustScrollPosition(RectTransform selectedRectTransform)
    {
        int buttonIndex = Mathf.RoundToInt((selectedRectTransform.anchoredPosition.y - topBuffer) / -yDecrement);

        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollTargetPosition = ((buttonIndex * yDecrement) + topBuffer) / (contentHeight - viewportHeight);

        scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(scrollTargetPosition);
    }
}
