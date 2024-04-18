using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    public static TradingManager Instance { get; private set; }
    public GameObject tradingScreen;
    public GameObject tradingPlayer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public GameObject GetTransitionScreen()
    {
        return tradingScreen;
    }
    public GameObject GetTransitionPlayer()
    {
        return tradingPlayer;
    }


}
