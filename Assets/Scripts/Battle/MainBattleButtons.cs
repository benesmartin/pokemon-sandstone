using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainBattleButtons : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject mainButtons;

    public bool isInMoves = false;
    public int escapeAttempts = 0;

    public static MainBattleButtons Instance { get; private set; }
    public AudioClip song;

    private void Awake()
    {
        ManageSingleton();
    }

    private void Update()
    {
        HandleEscapeKey();
    }

    private void ManageSingleton()
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

    private void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        if (isInMoves)
        {
            StartCoroutine(ReturnToMenuRoutine());
        }
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        GameObject.Find("Move Buttons").SetActive(false);
        mainButtons.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Battle Button"));
        isInMoves = false;

        yield return StartCoroutine(WaitForDialogueBox());
        SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
    }

    public void OnFightButton()
    {
        SetDialog("Choose a move!");
        isInMoves = true;
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Move 1 Button"));
    }

    public void OnPokemonButton()
    {
        StartCoroutine(PokemonButtonRoutine());
    }

    private IEnumerator PokemonButtonRoutine()
    {
        SetDialog("Pokemon is not implemented yet!");

        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
        SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
    }

    public void OnBagButton()
    {
        StartCoroutine(BagButtonRoutine());
    }

    private IEnumerator BagButtonRoutine()
    {
        SetDialog("Bag is not implemented yet!");

        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
        SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
    }

    public void OnRunButton()
    {
        StartCoroutine(RunButtonRoutine());
    }

    private IEnumerator RunButtonRoutine()
    {
        int F = (((playerUnit.Pokemon.Speed * 128) / enemyUnit.Pokemon.Speed) + 30 * escapeAttempts) % 256;
        if (Random.Range(0, 256) < F)
        {
            SetDialog("Got away safely!");
            yield return StartCoroutine(WaitForDialogBox());
            yield return new WaitForSeconds(1f);
            PlaySong(song);
            SceneManager.LoadScene("Game");
        }
        else
        {
            SetDialog("Can't escape!");
            yield return StartCoroutine(WaitForDialogBox());
            yield return new WaitForSeconds(1f);
            SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");

            escapeAttempts++;
        }
    }

    private IEnumerator WaitForDialogBox()
    {
        yield return StartCoroutine(WaitForDialogueBox());
    }

    private IEnumerator WaitForDialogueBox()
    {
        while (dialogBox.GetComponent<BattleDialogBox>().isTyping)
        {
            yield return null;
        }
    }

    private void SetDialog(string message)
    {
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
    }
    public void PlaySong(AudioClip song)
    {
        BGmusic.instance.audioSource.Stop();
        BGmusic.instance.audioSource.clip = song;
        BGmusic.instance.audioSource.Play();
    }
}

