using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusManager : MonoBehaviour
{
    private GameObject lastSelectedObject;
    [SerializeField] public EventSystem eventSystem;
    public static UIFocusManager Instance { get; private set; }
    private bool shouldRestoreFocus = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }

        if (lastSelectedObject != null)
        {
            eventSystem.SetSelectedGameObject(lastSelectedObject);
            shouldRestoreFocus = false;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lastSelectedObject != null)
        {
            shouldRestoreFocus = true;
        }
    }
}
