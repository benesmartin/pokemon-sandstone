using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBagManager : MonoBehaviour
{
    public static ItemBagManager Instance { get; private set; }

    [SerializeField] private GameObject itemScreen;

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




    public GameObject GetItemScreen()
    {
        return itemScreen;
    }
}
