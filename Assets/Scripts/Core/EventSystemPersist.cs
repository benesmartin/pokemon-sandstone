using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemPersist : MonoBehaviour
{
    public static EventSystemPersist Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        CheckAndDestroyExtraEventSystems();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndDestroyExtraEventSystems();
    }

    private void CheckAndDestroyExtraEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        foreach (var es in eventSystems)
        {
            if (es.gameObject != gameObject)
            {
                Destroy(es.gameObject);
            }
        }
    }

}
