using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] string sceneToLoad;
    [SerializeField] int x;
    [SerializeField] int y;
    private Fader fader;
    public void OnPlayerTriggered(PlayerMovement player)
    {
        Debug.Log("Player triggered portal");
        CharacterValueManager.Instance.posX = x;
        CharacterValueManager.Instance.posY = y;

        StartCoroutine(SwitchScene());
    }
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        yield return fader.FadeOut(0.5f);
        Destroy(gameObject);
    }
}
