using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PokedexScreen : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Button cancelButton, prevPageButton, nextPageButton, pokedexButton, debugButton;
    [SerializeField] Transform buttonContainer;
    private EventSystem eventSystem;
    [SerializeField] private int maxRowLength = 3;
    [SerializeField] private int maxRowsPerPage = 4;
    [SerializeField] private Vector2 startPosition = new Vector2(-50, 0);
    [SerializeField] private Vector2 buttonSpacing = new Vector2(300, 250);

    private List<PokemonBase> allPokemon;
    private List<GameObject> currentPageButtons = new List<GameObject>();
    private int currentPage = 0;
    private bool debugAllCaught = false;
    private Dictionary<int, bool> originalCaughtStatus = new Dictionary<int, bool>();
    private int totalNumberOfPages => Mathf.CeilToInt((float)allPokemon.Count / (maxRowLength * maxRowsPerPage));
    void Awake()
    {
        eventSystem = UIFocusManager.Instance.eventSystem;
    }   
    void Start()
    {
        
    }
    public void OpenPokedex()
    {
        allPokemon = GetAllPokemon();
        CreatePokedexButtons();
        SetupButtonNavigation();
        StartCoroutine(SetInitialFocus());
    }
    private IEnumerator SetInitialFocus()
    {
        yield return new WaitForEndOfFrame();
        FocusOnFirstButtonOfPage();
    }


    private List<PokemonBase> GetAllPokemon()
    {
        var list = new List<PokemonBase>(Resources.LoadAll<PokemonBase>("Bases"));
        list.Sort((x, y) => x.PokedexNumber.CompareTo(y.PokedexNumber));
        return list;
    }

    public void OnCancelButton()
    {
        eventSystem.SetSelectedGameObject(pokedexButton.gameObject);
    }

    private void CreatePokedexButtons()
    {
        foreach (var button in currentPageButtons)
        {
            Destroy(button);
        }
        currentPageButtons.Clear();

        Vector2 nextPosition = startPosition;
        int startIndex = currentPage * maxRowLength * maxRowsPerPage;
        int endIndex = Mathf.Min(startIndex + maxRowLength * maxRowsPerPage, allPokemon.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            buttonObj.GetComponent<RectTransform>().anchoredPosition = nextPosition;
            buttonObj.name = "Button_" + allPokemon[i].Name;
            PokedexButton pokedexButton = buttonObj.GetComponent<PokedexButton>();
            pokedexButton.SetupButton(allPokemon[i]);
            currentPageButtons.Add(buttonObj);

            if ((i - startIndex + 1) % maxRowLength == 0)
            {
                nextPosition.x = startPosition.x;
                nextPosition.y -= buttonSpacing.y;
            }
            else
            {
                nextPosition.x += buttonSpacing.x;
            }
        }

        SetupButtonNavigation();
        UpdatePageNavigationButtons();
    }
    public void ToggleDebugAllCaught()
    {
        debugAllCaught = !debugAllCaught;
        if (debugAllCaught)
        {
            // Save current caught status and mark all as caught
            originalCaughtStatus.Clear();
            foreach (PokemonBase pokemon in allPokemon)
            {
                int pokedexNumber = pokemon.PokedexNumber;
                bool isCaught = PokedexManager.Instance.IsPokemonCaught(pokedexNumber);
                originalCaughtStatus[pokedexNumber] = isCaught;
                if (!isCaught)
                {
                    PokedexManager.Instance.AddPokemonAsCaught(pokedexNumber);
                }
            }
        }
        else
        {
            // Restore original caught status
            foreach (var entry in originalCaughtStatus)
            {
                if (!entry.Value)
                {
                    PokedexManager.Instance.RemovePokemon(entry.Key); // You will need to implement this method in PokedexManager
                }
            }
        }
        CreatePokedexButtons(); // Refresh UI to reflect debug state
    }
    private void UpdatePageNavigationButtons()
    {
        prevPageButton.gameObject.SetActive(currentPage > 0);
        nextPageButton.gameObject.SetActive(currentPage < totalNumberOfPages - 1);
    }

    public void OnNextPage()
    {
        if (currentPage < totalNumberOfPages - 1)
        {
            currentPage++;
            CreatePokedexButtons();
            FocusOnFirstButtonOfPage();
        }
    }

    public void OnPrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            CreatePokedexButtons();
            FocusOnFirstButtonOfPage();
        }
    }

    private void FocusOnFirstButtonOfPage()
    {
        if (currentPageButtons.Count > 0)
        {
            GameObject firstButton = currentPageButtons[0];
            if (firstButton != null && firstButton.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(firstButton);

                Outline prevPageOutline = prevPageButton.GetComponent<Outline>();
                if (prevPageOutline != null) prevPageOutline.enabled = false;

                Outline nextPageOutline = nextPageButton.GetComponent<Outline>();
                if (nextPageOutline != null) nextPageOutline.enabled = false;

                Outline firstButtonOutline = firstButton.GetComponent<Outline>();
                if (firstButtonOutline != null) firstButtonOutline.enabled = true;

                ExecuteEvents.Execute<ISelectHandler>(firstButton, new BaseEventData(eventSystem), ExecuteEvents.selectHandler);
            }
        }
    }



    private void SetupButtonNavigation()
    {
        int numberOfButtons = currentPageButtons.Count;

        for (int i = 0; i < numberOfButtons; i++)
        {
            Button currentButton = currentPageButtons[i].GetComponent<Button>();
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;

            if (i >= maxRowLength)
            {
                customNav.selectOnUp = currentPageButtons[i - maxRowLength].GetComponent<Button>();
            }

            if (i < numberOfButtons - maxRowLength)
            {
                customNav.selectOnDown = currentPageButtons[i + maxRowLength].GetComponent<Button>();
            }
            else
            {
                if (currentPage < totalNumberOfPages - 1)
                {
                    customNav.selectOnDown = nextPageButton;
                }
                else if (currentPage > 0)
                {
                    customNav.selectOnDown = prevPageButton;
                }
            }

            if (i % maxRowLength == 0)
            {
                customNav.selectOnLeft = cancelButton;
            }
            else
            {
                customNav.selectOnLeft = currentPageButtons[i - 1].GetComponent<Button>();
            }

            if ((i + 1) % maxRowLength != 0 && i < numberOfButtons - 1) 
            {                 
                customNav.selectOnRight = currentPageButtons[i + 1].GetComponent<Button>();
            }

            currentButton.navigation = customNav;
        }

        Navigation prevNav = new Navigation { mode = Navigation.Mode.Explicit };
        Navigation nextNav = new Navigation { mode = Navigation.Mode.Explicit };

        if (currentPageButtons.Count > 0)
        {
            prevNav.selectOnUp = currentPageButtons[numberOfButtons - 1].GetComponent<Button>();
            nextNav.selectOnUp = currentPageButtons[numberOfButtons - 1].GetComponent<Button>();
        }

        prevNav.selectOnRight = nextPageButton;
        nextNav.selectOnLeft = prevPageButton;

        prevPageButton.navigation = prevNav;
        nextPageButton.navigation = nextNav;

        Navigation cancelNav = new Navigation();
        cancelNav.mode = Navigation.Mode.Explicit;
        if (currentPageButtons.Count > 0)
        {
            cancelNav.selectOnRight = currentPageButtons[0].GetComponent<Button>();
        }
        cancelNav.selectOnDown = debugButton;
        cancelButton.navigation = cancelNav;
    }
}
