using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    public GameObject transitionScreen;
    public GameObject transitionPlayer;
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
        return transitionScreen;
    }
    public GameObject GetTransitionPlayer()
    {
        return transitionPlayer;
    }


}
