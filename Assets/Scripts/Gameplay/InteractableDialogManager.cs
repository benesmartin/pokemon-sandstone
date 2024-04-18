using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDialogManager : MonoBehaviour
{
    public static InteractableDialogManager Instance { get; private set; }
    public GameObject dialogBox;

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
}
