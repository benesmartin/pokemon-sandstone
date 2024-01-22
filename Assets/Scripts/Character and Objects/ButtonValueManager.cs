using UnityEngine;
using System.Collections.Generic;

public class ButtonValueManager : MonoBehaviour
{
    public static ButtonValueManager Instance { get; private set; }

    public Dictionary<string, bool> buttonHighlights = new Dictionary<string, bool>
    {
        { "Text Speed Mid", true },
        { "Battle Style Shift", true },
        { "Battle Scene On", true }
    };

    public string selectedTextSpeed = "Mid";
    public bool isBattleSceneOn = true;
    public string selectedBattleStyle = "Shift";
    public float volume = 1;

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
