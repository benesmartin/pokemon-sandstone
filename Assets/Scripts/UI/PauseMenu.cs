using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using static UnityEditor.Progress;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    private GameObject selectedPokemonButton = null;
    private Pokemon selectedPokemon = null;
    [SerializeField] GameObject[] pokemonPartyButtons;
    [SerializeField] GameObject[] pokemonPartyButtons2;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject cancelButton2;
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject firsttButton;
    [SerializeField] GameObject[] categoryButtons;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject pokedexButton;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject partyScreen;
    [SerializeField] GameObject bagButton;
    [SerializeField] GameObject partyButton;
    [SerializeField] ModalPanel modalPanel;
    [SerializeField] GameObject pokemonParty;
    [SerializeField] GameObject pokemonPartyDialogBox;
    [SerializeField] GameObject pokemonPartyButton;
    [SerializeField] GameObject mapScreen;
    [SerializeField] GameObject mapExitButton;
    private EventSystem eventSystem;   
    private float timeSinceLastKeyPress = 0.0f;
    private float debounceTime = 0.5f; 
    public bool isInBag = false;
    public List<Button> buttonsList = new();
    public bool isInSubmenu = false;
    public Item chosenItem;
    public Button currentItem;
    public static PauseMenu Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        eventSystem = UIFocusManager.Instance.eventSystem;
    }



    public void SetSubmenu(bool value)
    {
        isInSubmenu = value;
    }   
    void Update()
    {
        timeSinceLastKeyPress += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && timeSinceLastKeyPress >= debounceTime)
        {
            CreateAFewItemsFromEachCategory();
            PrintInventory();
            timeSinceLastKeyPress = 0.0f; 
        }
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu")) && !isInSubmenu)
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrintStorageSystem();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Pokemon randomPokemon = MapArea.Instance.GetRandomWildPokemon();
            randomPokemon.Init();
            randomPokemon.ID = Guid.NewGuid();
            PokemonParty.Instance.AddPokemon(randomPokemon);
        }

    }
    public void CreateAFewItemsFromEachCategory()
    {
        
        Inventory.Instance.CreateOrAddItem<StandardPokeball>(5);
        Inventory.Instance.CreateOrAddItem<MasterBall>(5);
        Inventory.Instance.CreateOrAddItem<GreatBall>(5);
        Inventory.Instance.CreateOrAddItem<UltraBall>(5);

        Inventory.Instance.CreateOrAddItem<StandardPotion>(5);
        Inventory.Instance.CreateOrAddItem<SuperPotion>(5);
        Inventory.Instance.CreateOrAddItem<HyperPotion>(5);
        Inventory.Instance.CreateOrAddItem<MaxPotion>(5);
        Inventory.Instance.CreateOrAddItem<FullRestore>(5);
        Inventory.Instance.CreateOrAddItem<FullHeal>(5);
        Inventory.Instance.CreateOrAddItem<Antidote>(5);
        Inventory.Instance.CreateOrAddItem<Awakening>(5);
        Inventory.Instance.CreateOrAddItem<BurnHeal>(5);
        Inventory.Instance.CreateOrAddItem<IceHeal>(5);
        Inventory.Instance.CreateOrAddItem<Revive>(5);
        Inventory.Instance.CreateOrAddItem<MaxRevive>(5);


        Inventory.Instance.CreateOrAddItem(new Item("Oran Berry", "A berry to be consumed by Pokemon. Restores 10 HP.", ItemCategory.Berry), 15);
        Inventory.Instance.CreateOrAddItem(new Item("Lum Berry", "A berry that heals any status conditions.", ItemCategory.Berry), 8);

        
        Inventory.Instance.CreateOrAddItem(new Item("TM01", "Teaches a move to a Pokemon.", ItemCategory.TM), 3);

        
        Inventory.Instance.CreateOrAddItem(new Item("X Attack", "Boosts the Attack stat of a Pokemon during a battle.", ItemCategory.BattleItem), 10);


        Inventory.Instance.CreateOrAddItem<Map>(1);

        
        Inventory.Instance.CreateOrAddItem(new Item("Escape Rope", "A long, durable rope. Use it to escape instantly from a cave or a dungeon.", ItemCategory.Item), 3);
    }

    public void PrintStorageSystem()
    {
        Debug.Log("Storage System\n---");
        foreach (var pokemon in PokemonParty.Instance.Pokemons)
        {
            Debug.Log("-> "+ pokemon.Base.Name + "(Lv. " + pokemon.Level + ")" + "["+ pokemon.HP + "/" + pokemon.Base.MaxHP + "]");
        }
    }
    public void PrintInventory()
    {
        Debug.Log("printing inventory!");
        Inventory.Instance.PrintInventory();
    }
    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);   
        GameIsPaused = false;
        PlayerMovement.Instance.isPaused = false;
    }
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        eventSystem.SetSelectedGameObject(pokedexButton);
        PlayerMovement.Instance.isPaused = true;
        
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        ResumeGame();
    }
    public void PlaySFX(AudioClip clip)
    {
        BGmusic.instance.audioSource.PlayOneShot(clip);
    }
    public void PlaySong(AudioClip song)
    {
        BGmusic.instance.audioSource.Stop();
        BGmusic.instance.audioSource.clip = song;
        BGmusic.instance.audioSource.Play();
    }
    public void ShowParty()
    {
        if (PokemonParty.Instance.Pokemons.Count > 0) 
        {
            SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
            partyScreen.SetActive(true);
            eventSystem.SetSelectedGameObject(pokemonPartyButtons[0]);
        }
        else
        {
            Debug.Log("Party is empty!");
            StartCoroutine(AlertParty(1f));
        }
    }
    public void UseItem(int index)
    {
        StartCoroutine(UseItemRoutine(index, chosenItem));
    }
    public IEnumerator UseItemRoutine(int index, Item item)
    {
        if (item is Pokeball)
        {
            yield break;
        }
        else if (item.Category == ItemCategory.Potion)
        {
            yield return UsePotion(index, item as Potion);
        }

    }
    private IEnumerator UsePotion(int index, Potion item)
    {
        Pokemon pokemon = PokemonParty.Instance.Pokemons[index];
        Debug.Log($"Using {item.Name} on {pokemon.Base.Name}.");
        Debug.Log($"HP before: {pokemon.HP}");
        if (pokemon.HP == pokemon.MaxHP)
        {
            yield return TypeDialog($"It won't have any effect.");
            yield return new WaitForSeconds(2f);
            yield return TypeDialog("On which Pokemon should the item be used?");
            yield break;
        }
        if ((item.Name == "Revive" || item.Name == "Max Revive") && pokemon.HP > 0)
        {
            yield return TypeDialog($"It won't have any effect.");
            yield return new WaitForSeconds(2f);
            yield return TypeDialog("On which Pokemon should the item be used?");
            yield break;
        }
        item.Use(pokemon);
        Inventory.Instance.RemoveItem(item.Name);
        yield return TypeDialog($"You used {item.Name} on {pokemon.Base.Name}.");

        var slider = pokemonPartyButtons2[index].GetComponent<PartyButtonManager>().GetSlider();
        var text = pokemonPartyButtons2[index].GetComponent<PartyButtonManager>().GetText();
        var image = pokemonPartyButtons2[index].GetComponent<PartyButtonManager>().GetImage();
        yield return StartCoroutine(UpdateHP(pokemon, slider, text, image));
        yield return new WaitForSeconds(2f);
        pokemonParty.SetActive(false);
        ItemBagManager.Instance.GetItemScreen().SetActive(true);
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(currentItem.gameObject);
        Transform countTransform = currentItem.gameObject.transform.Find("Background/Count");
        if (countTransform != null)
        {
            Debug.Log("Count GameObject found.");
            TextMeshProUGUI countText = countTransform.GetComponent<TextMeshProUGUI>();
            if (countText != null)
            {
                Debug.Log($"Current count text: {countText.text}");

                try
                {
                    string displayedCountStr = countText.text.Substring(1);
                    int displayedCount = int.Parse(displayedCountStr);
                    Debug.Log($"Displayed count parsed: {displayedCount}");

                    int actualCount = Inventory.Instance.GetItemCount(item.Name);
                    Debug.Log($"Actual count from inventory: {actualCount}");

                    if (displayedCount != actualCount)
                    {
                        Debug.Log("Displayed count is different from actual count. Updating...");
                        countText.text = $"x{actualCount}";
                    }
                    else
                    {
                        Debug.Log("Displayed count matches the actual count. No update needed.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing displayed count: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("Count TextMeshProUGUI component not found on 'Count' GameObject.");
            }
        }
        else
        {
            Debug.LogError("Count GameObject not found.");
        }


        yield break;
    }
    public IEnumerator UpdateHP(Pokemon pokemon, UnityEngine.UI.Slider hpSlider, TextMeshProUGUI hpValue, UnityEngine.UI.Image fillImage)
    {
        yield return StartCoroutine(UpdateHPCoroutine(pokemon, hpSlider, hpValue, fillImage));
    }

    IEnumerator UpdateHPCoroutine(Pokemon pokemon, UnityEngine.UI.Slider hpSlider, TextMeshProUGUI hpValue, UnityEngine.UI.Image fillImage)
    {
        Debug.Log("[StatGUI UpdateHPCoroutine] Called for: " + pokemon.Base.Name + " with HP: " + pokemon.HP);
        while (hpSlider.value != pokemon.HP)
        {
            if (hpSlider.value < pokemon.HP)
                hpSlider.value++;
            else
                hpSlider.value--;
            hpValue.text = hpSlider.value + "/" + pokemon.MaxHP;
            Canvas.ForceUpdateCanvases();
            UpdateFillColor((int)hpSlider.value, pokemon.MaxHP, fillImage);

            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator TypeDialog(string message)
    {
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
        yield return StartCoroutine(WaitForDialogueBox());
    }
    private IEnumerator WaitForDialogueBox()
    {
        while (pokemonPartyDialogBox.GetComponent<BattleDialogBox>().isTyping)
        {
            yield return null;
        }
    }
    public void ShowParty(bool isApplyingItem, Button item)
    {
        if (PokemonParty.Instance.Pokemons.Count > 0)
        {
            if (isApplyingItem)
            {
                currentItem = item;
                ItemBagManager.Instance.GetItemScreen().SetActive(false);
                pokemonParty.SetActive(true);
                SetPokemonPartyButtons2(PokemonParty.Instance.Pokemons);
                UIFocusManager.Instance.eventSystem.SetSelectedGameObject(pokemonPartyButton);
                pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("On which Pokemon should the item be used?");
                return;
            }
            SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
            partyScreen.SetActive(true);
            eventSystem.SetSelectedGameObject(pokemonPartyButtons[0]);
        }
        else
        {
            Debug.Log("Party is empty!");
            StartCoroutine(AlertParty(1f));
        }
    }
    public GameObject GetBagButton()
    {
        return bagButton;
    }
    public void SetPokemonPartyButtons2(List<Pokemon> pokemons)
    {
        foreach (var button in pokemonPartyButtons2)
        {
            button.SetActive(false);
        }

        List<GameObject> activeButtons = new List<GameObject>();
        for (int i = 0; i < pokemons.Count; i++)
        {
            Pokemon pokemon = pokemons[i];
            GameObject pokemonButton = pokemonPartyButtons2[i];
            SetPokemonButton2(pokemonButton, pokemon);
            activeButtons.Add(pokemonButton);
        }

        SetupNavigation2(activeButtons);
    }
    public void OnCancelButton()
    {
        pokemonParty.SetActive(false);
        ItemBagManager.Instance.GetItemScreen().SetActive(true);
        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(currentItem.gameObject);
    }
    private void SetupNavigation2(List<GameObject> activeButtons)
    {
        Debug.Log("Setting up navigation for " + activeButtons.Count + " buttons.");
        Debug.Log("Active buttons: " + string.Join(", ", activeButtons));
        if (activeButtons.Count == 0) return;

        Navigation cancelNav = new Navigation();
        cancelNav.mode = Navigation.Mode.Explicit;
        cancelNav.selectOnUp = activeButtons[activeButtons.Count - 1].GetComponent<Button>();
        cancelButton2.GetComponent<Button>().navigation = cancelNav;

        if (activeButtons.Count == 1)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnDown = cancelButton2.GetComponent<Button>();
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
                nav.selectOnDown = cancelButton2.GetComponent<Button>();
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

    private void SetPokemonButton2(GameObject pokemonButton, Pokemon pokemon)
    {


        if (pokemonButton == null) Debug.LogError("pokemonButton is null");
        pokemonButton.SetActive(true);
        FindDeepChild(pokemonButton, "Party Sprite").GetComponent<Image>().sprite = pokemon.Base.FrontSprite;
        FindDeepChild(pokemonButton, "Name").GetComponent<TextMeshProUGUI>().text = pokemon.Base.Name;
        FindDeepChild(pokemonButton, "Level").GetComponent<TextMeshProUGUI>().text = $"Lv. {pokemon.Level}";
        Debug.Log($"hp {pokemon.HP}");
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().maxValue = pokemon.MaxHP;
        FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>().value = pokemon.HP;
        Canvas.ForceUpdateCanvases();
        FindDeepChild(pokemonButton, "HP Value").GetComponent<TextMeshProUGUI>().text = $"{pokemon.HP}/{pokemon.MaxHP}";
        var fillImage = FindDeepChild(pokemonButton, "Fill").GetComponent<Image>();
        UpdateFillColor(pokemon.HP, pokemon.MaxHP, fillImage);

    }
    private IEnumerator AlertParty(float seconds)
    {
        Debug.Log("Coroutine started: Setting text to 'Party is empty!'");
        title.text = "Party is empty!";
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log("Coroutine resumed: Setting text to 'PAUSED'");
        title.text = "PAUSED";
    }

    private void SetPokemonPartyButtons(List<Pokemon> pokemons)
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

            
            pokemonButton.GetComponent<Button>().onClick.RemoveAllListeners();

            
            pokemonButton.GetComponent<Button>().onClick.AddListener(() => OnPokemonButtonClick(pokemon, pokemonButton));

            SetPokemonButton(pokemonButton, pokemon);
            activeButtons.Add(pokemonButton);
        }

        SetupNavigation(activeButtons);
    }

    private void OnPokemonButtonClick(Pokemon pokemon, GameObject pokemonButton)
    {
        if (selectedPokemonButton == null)
        {
            
            selectedPokemonButton = pokemonButton;
            selectedPokemon = pokemon;
            HighlightButton(pokemonButton, true); 
        }
        else
        {
            if (selectedPokemonButton != pokemonButton)
            {
                
                SwitchPokemons(selectedPokemon, pokemon);
            }

            
            HighlightButton(selectedPokemonButton, false);
            if (selectedPokemonButton != pokemonButton)
            {
                HighlightButton(pokemonButton, false);
            }

            selectedPokemonButton = null;
            selectedPokemon = null;
        }
    }

    private void SwitchPokemons(Pokemon firstPokemon, Pokemon secondPokemon)
    {
        
        if (firstPokemon != secondPokemon)
        {
            int firstIndex = PokemonParty.Instance.Pokemons.IndexOf(firstPokemon);
            int secondIndex = PokemonParty.Instance.Pokemons.IndexOf(secondPokemon);

            
            PokemonParty.Instance.Pokemons[firstIndex] = secondPokemon;
            PokemonParty.Instance.Pokemons[secondIndex] = firstPokemon;

            
            SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
            Canvas.ForceUpdateCanvases();
        }
    }


    private void HighlightButton(GameObject button, bool highlight)
    {
        
        if (highlight)
        {
            button.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            button.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void SetupNavigation(List<GameObject> activeButtons)
    {
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

            if (activeButtons.Count == 1 || i >= activeButtons.Count - 2)
                nav.selectOnDown = cancelButton.GetComponent<Button>();
            else if (i + 2 < activeButtons.Count)
                nav.selectOnDown = activeButtons[i + 2].GetComponent<Button>();

            if (i % 2 == 0)
                nav.selectOnRight = (i + 1 < activeButtons.Count) ? activeButtons[i + 1].GetComponent<Button>() : null;
            else
                nav.selectOnLeft = activeButtons[i - 1].GetComponent<Button>();

            activeButtons[i].GetComponent<Button>().navigation = nav;
        }
    }





    private void SetPokemonButton(GameObject pokemonButton, Pokemon pokemon)
    {
        if (pokemonButton == null)
        {
            Debug.LogError("pokemonButton is null");
            return;
        }

        
        pokemonButton.SetActive(true);

        
        FindDeepChild(pokemonButton, "Party Sprite").GetComponent<Image>().sprite = pokemon.Base.FrontSprite;
        FindDeepChild(pokemonButton, "Name").GetComponent<TextMeshProUGUI>().text = pokemon.Base.Name;
        FindDeepChild(pokemonButton, "Level").GetComponent<TextMeshProUGUI>().text = $"Lv. {pokemon.Level}";

        Slider hpSlider = FindDeepChild(pokemonButton, "Slider").GetComponent<Slider>();
        hpSlider.maxValue = pokemon.MaxHP;  
        hpSlider.value = pokemon.HP;        

        FindDeepChild(pokemonButton, "HP Value").GetComponent<TextMeshProUGUI>().text = $"{pokemon.HP}/{pokemon.MaxHP}";

        var fillImage = FindDeepChild(pokemonButton, "Fill").GetComponent<Image>();
        UpdateFillColor(pokemon.HP, pokemon.MaxHP, fillImage);

        
        Canvas.ForceUpdateCanvases();
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
    private void ClearButtonsExceptFirst()
    {
        Transform parentTransform = firsttButton.transform.parent;
        firsttButton.GetComponent<Outline>().enabled = false;

        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);
            if (child.gameObject != firsttButton.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }
    public void OnCancelBagButton()
    {
        ExecuteEvents.Execute<ISelectHandler>(bagButton, new BaseEventData(eventSystem), ExecuteEvents.selectHandler);
    }
    public void OpenInventory()
    {

        UIFocusManager.Instance.eventSystem.SetSelectedGameObject(categoryButtons[0]);
        OnBagButton("Item");
    }
    public void OnBagButton(string categoryName)
    {
        ClearButtonsExceptFirst();

        if (Enum.TryParse(categoryName, out ItemCategory category))
        {
            int categoryIndex = Array.IndexOf(Enum.GetValues(typeof(ItemCategory)), category);
            Debug.Log("Category index: " + categoryIndex);
            PrintInfoOfCategory(categoryName);
            SetItemButtons(Inventory.Instance.GetItemsByCategory(category), categoryIndex);
        }
        else
        {
            Debug.LogError("Invalid category name: " + categoryName);
        }
    }
    private void PrintInfoOfCategory(string categoryName)
    {
        switch (categoryName)
        {
            case "Item":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Item Pocket\nFor items that will be useful during your adventure.");
                break;
            case "Potion":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Medicine Pocket\nFor items to regain your Pokemon's HP and PP.");
                break;
            case "Pokeball":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Poke Ball Pocket\nFor all different types of Poke Balls.");
                break;
            case "TM":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("TMs & HMs Pocket\nFor all different types of TMs and HMs.");
                break;
            case "Berry":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Berry Pocket\nFor sorting and storing different Berries.");
                break;
            case "BattleItem":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Battle Items Pocket\nFor items that are useful during battle.");
                break;
            case "KeyItem":
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Key Items Pocket\nFor items that are indispensable for your adventure.");
                break;
            default:
                dialogBox.GetComponent<BattleDialogBox>().SetDialogText("This category does not exist.");
                break;
        }
    }


    private void SetItemButtons(List<Item> items, int categoryIndex)
    {
        Button firstButton = this.firsttButton.GetComponent<Button>();
        float yPosition = 75f;
        float yDecrement = 155f;

        List<Button> buttons = new List<Button>();
        Debug.Log("Items count: " + items.Count);

        if (items.Count == 0)
        {
            firstButton.gameObject.SetActive(false); 
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            Item currentItem = items[i];
            Button newButton;

            if (i == 0)
            {
                newButton = firstButton;
            }
            else
            {
                newButton = Instantiate(firstButton, firstButton.transform.parent);

                Vector3 newPosition = newButton.transform.localPosition;
                newPosition.y = yPosition;
                newButton.transform.localPosition = newPosition;

                yPosition -= yDecrement;
            }
            newButton.onClick.RemoveAllListeners();
            SetButtonDesign(newButton, items[i]);
            newButton.gameObject.tag = "ItemButton";
            newButton.GetComponent<ItemHolder>().Item = currentItem;

            Item localItemCopy = currentItem;
            newButton.onClick.AddListener(() => OnItemButtonClick(localItemCopy, newButton));

            buttons.Add(newButton);
        }
        buttonsList = buttons;

        for (int i = 0; i < buttons.Count; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            if (i == 0)
            {
                nav.selectOnUp = categoryButtons[categoryIndex].GetComponent<Button>();
                nav.selectOnDown = (buttons.Count > 1) ? buttons[i + 1] : null;
                firstButton.GetComponent<Button>().navigation = nav;


                Debug.Log("First button: Up -> " + categoryButtons[categoryIndex].name + ", Down -> " + ((buttons.Count > 1) ? buttons[i + 1].name : "null"));
            }
            else
            {
                nav.selectOnUp = buttons[i - 1];
                nav.selectOnDown = (i < buttons.Count - 1) ? buttons[i + 1] : null;


                Debug.Log("Button " + (i + 1) + ": Up -> " + buttons[i - 1].name + ", Down -> " + ((i < buttons.Count - 1) ? buttons[i + 1].name : "null"));
            }

            buttons[i].navigation = nav;
        }

    }
    public void OnItemSelect(Item item)
    {
        if (item.Description != null) dialogBox.GetComponent<BattleDialogBox>().SetDialogText(item.Description);
        else dialogBox.GetComponent<BattleDialogBox>().SetDialogText("No description provided.");
    }
    private void OnItemButtonClick(Item item, Button button)
    {
        Debug.Log("Item clicked: " + item?.Name ?? "Null item");
        chosenItem = item;
        Scene currentScene = SceneManager.GetActiveScene();
        if (item.Category == ItemCategory.Pokeball && currentScene.name == "BattleScene")
        {
            modalPanel.Show(eventSystem, $"Throw a {item.Name}?",
                () =>
                {
                    ItemBagManager.Instance.GetItemScreen().SetActive(false);
                    MainBattleButtons.Instance.HideMainButtons();
                    eventSystem.SetSelectedGameObject(null);
                    button.GetComponent<Outline>().enabled = false;

                    StartCoroutine(MainBattleButtons.Instance.OnPokeballRoutine(item as Pokeball));
                },
                () =>
                {
                    eventSystem.SetSelectedGameObject(button.gameObject);
                });

        }
        else if (item.Category == ItemCategory.Potion)
        {
            if (MainBattleButtons.Instance) MainBattleButtons.Instance.OnItemApply(item);
            else 
            {
                ItemBagManager.Instance.GetItemScreen().SetActive(false);

                Debug.Log($"Applying {item.Name}...");
                chosenItem = item;
                ShowParty(true, button);
            }
        }
        else if (item.Category == ItemCategory.KeyItem && currentScene.name != "BattleScene")
        {
            KeyItem keyItem = item as KeyItem;
            keyItem.Use();
        }
        else
        {
            var message = currentScene.name == "BattleScene" ? "This item can't be used in battle." : "This item can't be used outside of battle.";
            modalPanel.ShowOkay(eventSystem, message,
                () =>
                {
                    eventSystem.SetSelectedGameObject(button.gameObject);
                });
        }
    }
    public void OpenMap()
    {
        mapScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(mapExitButton);
    }
    private void SetButtonDesign(Button button, Item item)
    {

        GameObject buttonz = button.gameObject;
        if (buttonz == null)
        {
            Debug.LogError("button is null");
            return;
        }


        buttonz.SetActive(true);


        FindDeepChild(buttonz, "Item Sprite").GetComponent<Image>().sprite = item.Image;
        FindDeepChild(buttonz, "Name").GetComponent<TextMeshProUGUI>().text = item.Name;
        if (item.Category != ItemCategory.KeyItem)
            FindDeepChild(buttonz, "Count").GetComponent<TextMeshProUGUI>().text = $"x{item.Count}";
        else
            FindDeepChild(buttonz, "Count").GetComponent<TextMeshProUGUI>().text = "";

        Canvas.ForceUpdateCanvases();
    }

}
