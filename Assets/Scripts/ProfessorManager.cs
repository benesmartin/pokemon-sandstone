using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorManager : MonoBehaviour
{
    public static ProfessorManager Instance { get; private set; }
    [SerializeField] public GameObject professorScreen;
    [SerializeField] public GameObject interactableDialog;

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
    private void Start()
    {
        if(CharacterValueManager.Instance.SpokeToProfessor) 
        {
            professorScreen.SetActive(false); 
            interactableDialog.SetActive(false);
        }
        else 
        {
            professorScreen.SetActive(true); 
            interactableDialog.SetActive(true);
        }
    }
}
