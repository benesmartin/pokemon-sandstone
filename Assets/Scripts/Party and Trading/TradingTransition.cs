using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingTransition : MonoBehaviour
{
    public static TradingTransition Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
