using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    private GameObject selectedPokemonButton = null;
    private Pokemon selectedPokemon = null;
    [SerializeField] GameObject[] pokemonPartyButtons;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject firstButton;
    [SerializeField] GameObject[] categoryButtons;
    [SerializeField] GameObject dialogBox;
    private float timeSinceLastKeyPress = 0.0f;
    private float debounceTime = 0.5f; 
    public bool isInBag = false;
    public List<Button> buttonsList = new();

    void Update()
    {
        timeSinceLastKeyPress += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && timeSinceLastKeyPress >= debounceTime)
        {
            CreateAFewItemsFromEachCategory();
            PrintInventory();
            timeSinceLastKeyPress = 0.0f; 
        }
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }
    public void CreateAFewItemsFromEachCategory()
    {
        
        Inventory.Instance.CreateOrAddItem<StandardPokeball>(5);
        Inventory.Instance.CreateOrAddItem<MasterBall>(5);
        Inventory.Instance.CreateOrAddItem<GreatBall>(5);
        Inventory.Instance.CreateOrAddItem<UltraBall>(5);

        
        Inventory.Instance.CreateOrAddItem(new Item("Basic Potion", "Restores 20 HP to one Pokemon.", ItemCategory.Potion), 10);
        Inventory.Instance.CreateOrAddItem(new Item("Super Potion", "Restores 50 HP to one Pokemon.", ItemCategory.Potion), 7);
        Inventory.Instance.CreateOrAddItem(new Item("Hyper Potion", "Restores 200 HP to one Pok�mon.", ItemCategory.Potion), 5);

        
        Inventory.Instance.CreateOrAddItem(new Item("Oran Berry", "A berry to be consumed by Pokemon. Restores 10 HP.", ItemCategory.Berry), 15);
        Inventory.Instance.CreateOrAddItem(new Item("Lum Berry", "A berry that heals any status conditions.", ItemCategory.Berry), 8);

        
        Inventory.Instance.CreateOrAddItem(new Item("TM01", "Teaches a move to a Pokemon.", ItemCategory.TM), 3);

        
        Inventory.Instance.CreateOrAddItem(new Item("X Attack", "Boosts the Attack stat of a Pok�mon during a battle.", ItemCategory.BattleItem), 10);

        
        Inventory.Instance.CreateOrAddItem(new Item("Mystery Key", "A mysterious key. It's purpose is unknown.", ItemCategory.KeyItem), 1);

        
        Inventory.Instance.CreateOrAddItem(new Item("Escape Rope", "A long, durable rope. Use it to escape instantly from a cave or a dungeon.", ItemCategory.Item), 3);
    }

    public void PrintStorageSystem()
    {
        Debug.Log("Storage System\n---");
        foreach (var pokemon in PokemonParty.Instance.StorageSystem)
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
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Pokédex"));
        Time.timeScale = 0f;
        GameIsPaused = true;
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
        SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
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
        Transform parentTransform = firstButton.transform.parent;

        
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);
            if (child.gameObject != firstButton.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
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
        Button firstButton = this.firstButton.GetComponent<Button>();
        float yPosition = 75f;
        float yDecrement = 155f;

        List<Button> buttons = new List<Button> { };
        Debug.Log("Items count: " + items.Count);
        if (items.Count == 0)
        {
            firstButton.gameObject.SetActive(false);
            firstButton.onClick.AddListener(() => OnItemButtonClick(new Item(null, null, ItemCategory.Item), firstButton));
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
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

            SetButtonDesign(newButton, items[i]);
            newButton.gameObject.tag = "ItemButton";
            Item currentItem = items[i];
            newButton.onClick.AddListener(() => OnItemButtonClick(currentItem, newButton));
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
    private void OnItemButtonClick(Item item, Button button)
    {
        if(item.Description != null) dialogBox.GetComponent<BattleDialogBox>().SetDialogText(item.Description);
        else dialogBox.GetComponent<BattleDialogBox>().SetDialogText("no description provided");
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
        if(item.Category != ItemCategory.KeyItem)
            FindDeepChild(buttonz, "Count").GetComponent<TextMeshProUGUI>().text = $"x{item.Count}";
        else
            FindDeepChild(buttonz, "Count").GetComponent<TextMeshProUGUI>().text = "";
        
        Canvas.ForceUpdateCanvases();
    }
}
