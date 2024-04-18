using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Import this namespace

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }
    [SerializeField] public GameObject loadScreen;
    [SerializeField] public GameObject firstLoadButton;
    [SerializeField] public ModalPanelSave modalPanelLoad;

    private string lastSceneName = "Menu"; // Track the last scene name

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the last scene name with the current scene at start
        lastSceneName = SceneManager.GetActiveScene().name;
    }

    // New method to handle scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        Debug.Log("Last scene: " + lastSceneName);
        // Check if the last scene was the menu scene
        if (lastSceneName == "Menu") // Replace "MenuSceneName" with the actual name of your menu scene
        {

            CharacterValueManager.Instance.posX = 100;
            CharacterValueManager.Instance.posY = 3;
            PlayerMovement.Instance.LoadCharacterPosition();
            modalPanelLoad.Init();
            UIFocusManager.Instance.eventSystem.SetSelectedGameObject(firstLoadButton);
            firstLoadButton.GetComponent<Outline>().enabled = true;
            loadScreen.SetActive(true);
        }

        // Update the last scene name
        lastSceneName = scene.name;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
    }

    public void HideScreen()
    {
        loadScreen.SetActive(false);
    }
}
