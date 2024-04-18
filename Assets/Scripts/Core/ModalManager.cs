using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ModalManager : MonoBehaviour
{
    public static ModalManager Instance { get; private set; }

    [SerializeField] public ModalPanel modalPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private bool _isModalActive = false;

    public bool IsModalActive
    {
        get { return _isModalActive; }
        set
        {
            if (_isModalActive != value) 
            {
                _isModalActive = value;
                Debug.Log($"Modal is now: {value}");
            }
        }
    }

}
