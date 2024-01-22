using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemPersist : MonoBehaviour
{
    public static EventSystemPersist Instance { get; private set; }
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
