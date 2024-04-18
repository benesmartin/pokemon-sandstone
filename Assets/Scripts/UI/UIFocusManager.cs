using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIFocusManager : MonoBehaviour
{
    private GameObject lastSelectedObject;
    [SerializeField] public EventSystem eventSystem;
    public Scene _lastScene;
    public Scene lastScene
    {
        get { return _lastScene; }
        set 
        { 
            _lastScene = value; 
            Debug.Log("Last scene: " + _lastScene.name);
        }
    }
    public static UIFocusManager Instance { get; private set; }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }
        else ResetFocus();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if(scene.name != "BattleScene") lastScene = scene;
        ResetFocusToDefault(scene);
    }

    private void ResetFocusToDefault(Scene scene)
    {
        if (scene.name == "Menu")
        {
            GameObject defaultButton = GameObject.Find("DefaultButton") ?? GameObject.FindGameObjectWithTag("DefaultButton");
            if (defaultButton)
            {
                eventSystem.SetSelectedGameObject(defaultButton);
            }
            else
            {
                Debug.LogError("No default button found for UI focus!");
            }
        }
    }
    private void ResetFocus()
    {
        eventSystem.SetSelectedGameObject(lastSelectedObject);
    }
}
