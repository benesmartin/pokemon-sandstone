using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainBattleButtons : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject mainButtons;
    [SerializeField] GameObject firstPokemonButton;
    [SerializeField] GameObject pokemonParty;
    [SerializeField] GameObject[] pokemonPartyButtons;
    [SerializeField] GameObject pokemonPartyDialogBox;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject pokeball;

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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            ReturnToMenu();
        }
    }
    public void HideMainButtons()
    {
        mainButtons.SetActive(false);
    }

    public void ReturnToMenu(bool isItTrue = false)
    {
        if (isInMoves || isItTrue)
        {
            StartCoroutine(ReturnToMenuRoutine());
        }
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        if (GameObject.Find("Move Buttons")) GameObject.Find("Move Buttons").SetActive(false);
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

        pokemonParty.SetActive(true);
        SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
        EventSystem.current.SetSelectedGameObject(firstPokemonButton);
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("Choose a Pokemon.");
    }
    public void OnCancelButton()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Battle Button"));
    }
    public void SetPokemonPartyButtons(List<Pokemon> pokemons)
    {
        foreach (var button in pokemonPartyButtons)
        {
            button.SetActive(false);
        }

        List<GameObject> activeButtons = new List<GameObject>();
        for (int i = 0; i < pokemons.Count; i++)
        {
            Pokemon pokemon = pokemons[i];
            GameObject pokemonButton = pokemonPartyButtons[i];
            SetPokemonButton(pokemonButton, pokemon);
            activeButtons.Add(pokemonButton);
        }

        SetupNavigation(activeButtons);
    }

    private void SetupNavigation(List<GameObject> activeButtons)
    {
        if (activeButtons.Count <= 1) return;

        for (int i = 0; i < activeButtons.Count; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;


            nav.selectOnUp = i >= 2 ? activeButtons[i - 2].GetComponent<Button>() : null;


            if (i >= activeButtons.Count - 2)
                nav.selectOnDown = cancelButton.GetComponent<Button>();
            else if (i + 2 < activeButtons.Count)
                nav.selectOnDown = activeButtons[i + 2].GetComponent<Button>();


            if (activeButtons.Count % 2 != 0 && i == activeButtons.Count - 2)
                nav.selectOnDown = activeButtons[i + 1].GetComponent<Button>();


            if (i % 2 == 0)
                nav.selectOnRight = (i + 1 < activeButtons.Count) ? activeButtons[i + 1].GetComponent<Button>() : null;
            else
                nav.selectOnLeft = activeButtons[i - 1].GetComponent<Button>();

            activeButtons[i].GetComponent<Button>().navigation = nav;
        }


        Navigation cancelNav = new Navigation();
        cancelNav.mode = Navigation.Mode.Explicit;
        cancelNav.selectOnUp = activeButtons[activeButtons.Count - 1].GetComponent<Button>();
        cancelButton.GetComponent<Button>().navigation = cancelNav;
    }




    private void SetPokemonButton(GameObject pokemonButton, Pokemon pokemon)
    {


        if (pokemonButton == null) Debug.LogError("pokemonButton is null");
        pokemonButton.SetActive(true);
        FindDeepChild(pokemonButton, "Party Sprite").GetComponent<Image>().sprite = pokemon.Base.FrontSprite;
        FindDeepChild(pokemonButton, "Name").GetComponent<TextMeshProUGUI>().text = pokemon.Base.Name;
        FindDeepChild(pokemonButton, "Level").GetComponent<TextMeshProUGUI>().text = $"Lv. {pokemon.Level}";
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().value = pokemon.HP;
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().maxValue = pokemon.MaxHP;
        FindDeepChild(pokemonButton, "HP Value").GetComponent<TextMeshProUGUI>().text = $"{pokemon.HP}/{pokemon.MaxHP}";
        var fillImage = FindDeepChild(pokemonButton, "Fill").GetComponent<Image>();
        Canvas.ForceUpdateCanvases();
        UpdateFillColor(pokemon.HP, pokemon.MaxHP, fillImage);

    }
    GameObject FindDeepChild(GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
                return child.gameObject;

            GameObject found = FindDeepChild(child.gameObject, childName);
            if (found != null)
                return found;
        }

        return null;
    }


    private void UpdateFillColor(int currentHP, int maxHP, Image fillImage)
    {
        float hpPercentage = (float)currentHP / maxHP;

        Color highHPColor = HexToColor("#00C304");
        Color mediumHPColor = Color.yellow;
        Color lowHPColor = Color.red;

        if (hpPercentage > 0.5f)
        {
            fillImage.color = Color.Lerp(mediumHPColor, highHPColor, (hpPercentage - 0.5f) * 2);
        }
        else
        {
            fillImage.color = Color.Lerp(lowHPColor, mediumHPColor, hpPercentage * 2);
        }
    }
    private Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
    private IEnumerator PokemonButtonRoutine()
    {
        SetDialog("Pokemon is not implemented yet!");

        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
        SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
        Debug.Log("HP: " + playerUnit.Pokemon.HP + "/" + playerUnit.Pokemon.MaxHP);
    }

    public void OnBagButton()
    {
        StartCoroutine(OnBagRoutine());
    }

    private IEnumerator OnBagRoutine()
    {
        if (!PokemonParty.Instance.IsWildBattle)
        {
            SetDialog("The Trainer blocked the Ball!");
            yield return StartCoroutine(WaitForDialogBox());
            yield return new WaitForSeconds(1f);
            SetDialog("Don't be a thief!");
            yield return StartCoroutine(WaitForDialogBox());
            yield return new WaitForSeconds(1f);
            mainButtons.SetActive(true);
            SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
            yield break;
        }
        if (BattleSystem.Instance != null)
        {
            StandardPokeball standardPokeball = new();
            string filename = Regex.Replace(standardPokeball.Name.ToLower(), @"\s+", "");
            pokeball.GetComponent<Image>().sprite = (Sprite)Resources.Load($"{filename}", typeof(Sprite));
            SetDialog("You threw a " + standardPokeball.Name + "!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);
            BattleSystem.Instance.TryCatchPokemon(standardPokeball);
        }
        else
        {
            Debug.LogError("BattleSystem instance not found.");
        }
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
        if (!PokemonParty.Instance.IsWildBattle)
        {
            SetDialog("No, there's no running away from a trainer battle!");
            yield return StartCoroutine(WaitForDialogBox());
            yield return new WaitForSeconds(1f);
            SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
            yield break;
        }
        int F = (((playerUnit.Pokemon.Speed * 128) / enemyUnit.Pokemon.Speed) + 30 * escapeAttempts) % 256;
        if (UnityEngine.Random.Range(0, 256) < F)
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

