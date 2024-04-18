using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorManager : MonoBehaviour
{
    public static ProfessorManager Instance { get; private set; }
    [SerializeField] public GameObject professorScreen;
    [SerializeField] public GameObject interactableDialog;
    [SerializeField] public Fader fader;
    [SerializeField] public NPCController professorNPC;

    public bool isProfessorScreenActive = false;

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
    public void StartProfessorScreen()
    {
        isProfessorScreenActive = true;
        professorScreen.SetActive(true);
        interactableDialog.SetActive(true);
    }
}
