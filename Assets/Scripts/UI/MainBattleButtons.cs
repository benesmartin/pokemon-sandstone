using DG.Tweening.Core.Easing;
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
    [SerializeField] GameObject itemScreen;
    [SerializeField] GameObject itemScreenDialogBox;
    [SerializeField] GameObject itemButton;

    public Item currentItem;

    public bool isItemScreenInBattle = false;

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
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(GameObject.Find("Battle Button"));
        isInMoves = false;

        yield return StartCoroutine(WaitForDialogueBox());
        SetDialog("What will " + playerUnit.Pokemon.Base.Name + " do?");
    }

    public void OnFightButton()
    {
        SetDialog("Choose a move!");
        isInMoves = true;
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(GameObject.Find("Move 1 Button"));
    }
    public void HidePartyScreen()
    {
        pokemonParty.SetActive(false);
    }
    public IEnumerator TypeDialog(string message, bool success)
    {
        Debug.Log($"Typing message: {message}");
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
        if (!success)
        {
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(2f);
            pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("On which Pokemon should the item be used?");
        }

    }
    public void OnPokemonButton()
    {

        pokemonParty.SetActive(true);
        SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(firstPokemonButton);
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("Choose a Pokemon.");
        BattleSystem.Instance.isSwitching = true;
    }

    public void OnItemApply(Item item)
    {
        currentItem = item; 
        ItemBagManager.Instance.GetItemScreen().SetActive(false);
        pokemonParty.SetActive(true);
        SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(firstPokemonButton);
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("On which Pokemon should the item be used?");
        BattleSystem.Instance.isApplyingItem = true;
    }

    public void OnCancelButton()
    {
        if (BattleSystem.Instance.isApplyingItem)
        {
            Debug.Log("Canceling item application.");
            BattleSystem.Instance.isApplyingItem = false;
            pokemonParty.SetActive(false);
            ItemBagManager.Instance.GetItemScreen().SetActive(true);
            UIFocusManager.Instance.eventSystem.SetSelectedGameObject(PauseMenu.Instance.currentItem.gameObject);
            return;
        }
        if (BattleSystem.Instance.isSwitching)
        {
            Debug.Log("Canceling pokemon switch.");
            BattleSystem.Instance.isSwitching = false;
            pokemonParty.SetActive(false);
            UIFocusManager.Instance.eventSystem.SetSelectedGameObject(GameObject.Find("Battle Button"));
            return;
        }
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(GameObject.Find("Battle Button"));
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
        Debug.Log("Setting up navigation for " + activeButtons.Count + " buttons.");
        Debug.Log("Active buttons: " + string.Join(", ", activeButtons));
        if (activeButtons.Count == 0) return;

        Navigation cancelNav = new Navigation();
        cancelNav.mode = Navigation.Mode.Explicit;
        cancelNav.selectOnUp = activeButtons[activeButtons.Count - 1].GetComponent<Button>();
        cancelButton.GetComponent<Button>().navigation = cancelNav;

        if (activeButtons.Count == 1)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnDown = cancelButton.GetComponent<Button>();
            activeButtons[0].GetComponent<Button>().navigation = nav;
            activeButtons[0].GetComponent<Outline>().enabled = true;
            return;
        }

        for (int i = 0; i < activeButtons.Count; i++)
        {
            if (i == 0) activeButtons[0].GetComponent<Outline>().enabled = true;
            else activeButtons[i].GetComponent<Outline>().enabled = false;
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
    }


    public void SetDialogParty(string message)
    {
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
    }   

    private void SetPokemonButton(GameObject pokemonButton, Pokemon pokemon)
    {


        if (pokemonButton == null) Debug.LogError("pokemonButton is null");
        pokemonButton.SetActive(true);
        FindDeepChild(pokemonButton, "Party Sprite").GetComponent<Image>().sprite = pokemon.Base.FrontSprite;
        FindDeepChild(pokemonButton, "Name").GetComponent<TextMeshProUGUI>().text = pokemon.Base.Name;
        FindDeepChild(pokemonButton, "Level").GetComponent<TextMeshProUGUI>().text = $"Lv. {pokemon.Level}";
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().maxValue = pokemon.MaxHP;
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().value = pokemon.HP;
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
        GameObject itemScreen = ItemBagManager.Instance.GetItemScreen();
        if (itemScreen != null)
        {
            itemScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("ItemScreen is null.");
        }

        PauseMenu.Instance.OpenInventory();
        isItemScreenInBattle = true;
    }

    private IEnumerator OnBagRoutine()
    {
        yield return StartCoroutine(BagButtonRoutine());
    }

    public IEnumerator OnPokeballRoutine(Pokeball ball)
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
            UIFocusManager.Instance.eventSystem.SetSelectedGameObject(GameObject.Find("Battle Button"));
            yield break;
        }
        if (BattleSystem.Instance != null)
        {
            Inventory.Instance.RemoveItem(ball.Name);
            string filename = ball.GetSpriteName();
            pokeball.GetComponent<Image>().sprite = Resources.Load<Sprite>(filename);
            SetDialog("You threw a " + ball.Name + "!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);
            BattleSystem.Instance.TryCatchPokemon(ball);
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

