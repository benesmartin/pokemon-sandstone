using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionScreenManager : MonoBehaviour
{
    public static EvolutionScreenManager Instance { get; private set; }
    [SerializeField] public GameObject evolutionScreen;
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
