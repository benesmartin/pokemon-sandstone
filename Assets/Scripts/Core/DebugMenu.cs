using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu Instance { get; private set; }
    public bool isCatchRate100 = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void UpdateDebugInfo(string box, string info)
    {
        GameObject gameObject = getGameObjectByName(box);
        gameObject.GetComponent<TextMeshProUGUI>().text = info;
    }
    public GameObject getGameObjectByName(string name)
    {
        switch (name) { 
            case "CatchRate":
                return catchRate;
            default:
                return null;
        }
    }


    [SerializeField] public GameObject debugMenuUI; 
    [SerializeField] public GameObject catchRate; 
    [SerializeField] public GameObject toggleCatchRate; 

    void Update()
    {
        if (Input.GetKey(KeyCode.F1)) 
        {
            debugMenuUI.SetActive(true);
        }
        else
        {
            debugMenuUI.SetActive(false);
        }
        if (Input.GetKey(KeyCode.F2))
        {
            isCatchRate100 = !isCatchRate100;
            toggleCatchRate.GetComponent<UnityEngine.UI.Toggle>().isOn = isCatchRate100;
        }
    }
    public void UpdateCR()
    {
        isCatchRate100 = toggleCatchRate.GetComponent<UnityEngine.UI.Toggle>().isOn;
        BattleSystem.Instance.UpdateCatchRate();
    }
}

