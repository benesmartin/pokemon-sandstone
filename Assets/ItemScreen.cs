using System.Collections.Generic;
using UnityEngine;

public class ItemScreen : MonoBehaviour
{
    // Add methods specific to the item screen here
    public static ItemScreen Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    public void ToggleVisibility(bool show)
    {
        gameObject.SetActive(show);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    // Other item screen methods...
}
