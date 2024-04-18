using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MoveBase;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject enemyHud;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject[] moveButtons;
    [SerializeField] private GameObject[] partyButtons;
    [SerializeField] private GameObject pokemonParty;
    [SerializeField] private GameObject pokemonPartyDialogBox;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameObject turnCounter;
    [SerializeField] private GameObject firstPokemonButton;
    [SerializeField] private GameObject battleBackground;
    [SerializeField] private Image playerStatus;
    [SerializeField] private Image enemyStatus;
    [SerializeField] private Fader fader;
    public int turn;
    private BattleState state;
    public AudioClip song;
    PokemonParty playerParty;
    PokemonParty enemyParty;
    Pokemon enemyPokemon;
    public bool isItTrue = false;
    public bool IsPlayerTurn;
    public bool IsFaintSwitching = false;
    public bool isSwitching = false;
    public bool isApplyingItem = false;
    public bool _shouldSkipAnimation = false;
    public bool shouldSkipAnimation
    {
        get { return _shouldSkipAnimation; }
        set
        {
            if (_shouldSkipAnimation != value)
            {
                _shouldSkipAnimation = value;
                Debug.Log($"shouldSkipAnimation is now: {value}");
            }
        }
    }
    public int Shakes = 99;
    public float catchRate;
    public List<Pokemon> NPCPokemons = new List<Pokemon>();
    public static BattleSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

            Debug.Log("hp: " + playerUnit.Pokemon.HP + " maxhp: " + playerUnit.Pokemon.MaxHP);
            playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon, true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("isplayerturn: " + IsPlayerTurn);
            foreach (var item in PokemonParty.Instance.Pokemons)
            {
                Debug.Log(item.Base.Name + " hp: " + item.HP + " maxhp: " + item.MaxHP);
            }
        }
    }
    private void Start()
    {
        CharacterValueManager.Instance.transition.SetActive(false);
        battleBackground.GetComponent<Image>().sprite = Resources.Load<Sprite>("BattleBackgrounds/" + UnityEngine.Random.Range(1, 4));
        StartBattle(PokemonParty.Instance.IsWildBattle);
    }
    public void StartBattle(bool isWildBattle)
    {
        Shakes = 99;
        turn = 1;
        var playerParty = PokemonParty.Instance;
        var enemyPokemon = isWildBattle ? MapArea.Instance.GetRandomWildPokemon() : EnemySpriteManager.Instance.GetHealthyPokemon();
        playerParty.GetHealthyPokemon();
        if (playerParty.GetHealthyPokemon() == null)
        {
            Debug.LogError("No healthy Pok�mon in player's party. Cannot start battle.");
            
            return;
        }

        if (enemyPokemon == null)
        {
            Debug.LogError("No wild Pok�mon found. Cannot start battle.");
            
            return;
        }

        SetupBattle(playerParty, enemyPokemon);
    }


    private void SetupBattle(PokemonParty playerParty, Pokemon enemyPokemon)
    {
        this.playerParty = playerParty;
        this.enemyPokemon = enemyPokemon;

        Debug.Log($"Setting up player unit with {playerParty.GetHealthyPokemon()?.Base?.Name} and ID {playerParty.GetHealthyPokemon()?.ID}");
        Debug.Log($"Setting up enemy unit with {enemyPokemon?.Base?.Name} and ID {enemyPokemon?.ID}");

        if (playerParty.GetHealthyPokemon() == null)
        {
            Debug.LogError("GetHealthyPokemon returned null.");
        }

        if (enemyPokemon == null)
        {
            Debug.LogError("GetRandomWildPokemon returned null or there is no healthy enemy Pokemon.");
        }
        Debug.Log($"Should skip animation: {shouldSkipAnimation}");
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(enemyPokemon);
        UpdateHudData();
        StartCoroutine(InitialDialog(shouldSkipAnimation));
    }

    float GetMoveEffectiveness(Move move, PokemonType opponentPrimaryType, PokemonType opponentSecondaryType)
    {
        float effectivenessPrimary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentPrimaryType);
        float effectivenessSecondary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentSecondaryType);

        return effectivenessPrimary * effectivenessSecondary;
    }
    private IEnumerator InitialDialog(bool skipAnimation = false)
    {
        skipAnimation = false;
        MainBattleButtons.Instance.HideMainButtons();
        PokedexManager.Instance.AddPokemonAsSeen(enemyPokemon.Base.PokedexNumber);
        enemyUnit.AddTrainerImage(EnemySpriteManager.Instance.GetSprite());
        if (skipAnimation)
        {
            dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Finishing up...");
            PlayerAction();
            yield break;
        }

        Debug.Log($"{enemyPokemon.Base.Name} was added as seen.");

        GameObject[] playerPokemonGUI = GameObject.FindGameObjectsWithTag("PlayerPokemonGUI");
        GameObject[] enemyPokemonGUI = GameObject.FindGameObjectsWithTag("FoePokemonGUI");
        foreach (var item in playerPokemonGUI)
        {
            item.GetComponent<Image>().DOFade(0, 0f);
            item.GetComponent<TextMeshProUGUI>().DOFade(0, 0f);
        }
        foreach (var item in enemyPokemonGUI)
        {
            item.GetComponent<Image>().DOFade(0, 0f);
            item.GetComponent<TextMeshProUGUI>().DOFade(0, 0f);
        }
        if (PokemonParty.Instance.IsWildBattle)
        {
            dialogBox.SetActive(false);
            yield return new WaitForSeconds(3f);
            dialogBox.SetActive(true);
            yield return TypeDialog("A wild " + enemyUnit.Pokemon.Base.Name + " appeared!");
            foreach (var item in enemyPokemonGUI)
            {
                item.GetComponent<Image>().DOFade(1, 0.5f);
                item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
            }
            yield return new WaitForSeconds(1f);
            yield return TypeDialog("Go,  " + playerUnit.Pokemon.Base.Name + "!");
        }
        else
        {
            yield return TypeDialog("You are challenged by " + EnemySpriteManager.Instance.GetName() + "!");
            yield return new WaitForSeconds(1.5f);
            yield return TypeDialog(EnemySpriteManager.Instance.GetName() + " sent out " + enemyUnit.Pokemon.Base.Name + "!");
            yield return new WaitForSeconds(1f);
            foreach (var item in enemyPokemonGUI)
            {
                item.GetComponent<Image>().DOFade(1, 0.5f);
                item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
            }
            yield return new WaitForSeconds(0.75f);
            yield return TypeDialog("Go,  " + playerUnit.Pokemon.Base.Name + "!");
        }
        PlayerAction();
        foreach (var item in playerPokemonGUI)
        {
            item.GetComponent<Image>().DOFade(1, 0.5f);
            item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
        }
        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
    }



    private IEnumerator TypeDialog(string message)
    {
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
        yield return StartCoroutine(WaitForDialogueBox());
    }
    private void UpdateHudData()
    {
        playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon, true);
        enemyHud.GetComponent<StatGUI>().SetData(enemyUnit.Pokemon, false);
    }
    private float CalculateCatchRate(Pokemon targetPokemon, Pokeball pokeball)
    {
        float statusBonus = 1;
        float hpPercentage = (float)targetPokemon.HP / targetPokemon.MaxHP;
        float catchRate = targetPokemon.Base.BaseCatchRate * statusBonus * ((float)pokeball.CatchRateModifier / hpPercentage);

        return Mathf.Clamp(catchRate, 0, 255);
    }
    private int CalculateShakes(float catchRate)
    {
        int shakes = 0;
        for (int i = 0; i < 4; i++)
        {
            if (UnityEngine.Random.Range(0, 255) < catchRate)
                shakes++;
            else
                break;
        }
        return shakes;
    }
    private float GetCurrentCatchRateInPercent(Pokemon targetPokemon, Pokeball pokeball)
    {
        float catchRate = CalculateCatchRate(targetPokemon, pokeball);
        float catchRatePercent = Mathf.Max(1, catchRate / 255 * 100);

        return catchRatePercent;
    }

    public void TryCatchPokemon(Pokeball pokeballType)
    {
        Debug.Log("TryCatchPokemon called");
        float catchRate = CalculateCatchRate(enemyUnit.Pokemon, pokeballType);
        int shakes = CalculateShakes(catchRate);
        this.catchRate = GetCurrentCatchRateInPercent(enemyUnit.Pokemon, pokeballType);
        UpdateCatchRate(shakes);
        Debug.Log("Catch rate: " + catchRate + ", shakes: " + shakes);
        if(DebugMenu.Instance.isCatchRate100) shakes = 4;
        StartCoroutine(CatchAttemptCoroutine(shakes, pokeballType));
    }

    private IEnumerator CatchAttemptCoroutine(int shakes, Pokeball pokeballType)
    {
        bool animationCompleted = false;
        Action onAnimationComplete = () => animationCompleted = true;

        enemyUnit.PlayPokeballThrowAnimation(shakes, onAnimationComplete);

        yield return new WaitUntil(() => animationCompleted);

        string message;
        switch (shakes)
        {
            case 0: message = "Oh no! The Pokemon broke free!"; break;
            case 1: message = "Aww! It appeared to be caught!"; break;
            case 2: message = "Aargh! Almost had it!"; break;
            case 3: message = "Gah! It was so close, too!"; break;
            case 4:
                message = $"Gotcha! {enemyUnit.Pokemon.Base.Name} was caught!";
                yield return TypeDialog(message);
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(AddPokemonToPokedex(enemyUnit.Pokemon));
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(AddPokemonToParty(enemyUnit.Pokemon));
                yield return EndBattle(true);
                break;
            default: throw new InvalidOperationException("Invalid number of shakes");
        }
        if (shakes < 4)
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
            yield return StartCoroutine(WaitForDialogueBox());


            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(EnemyMoveRoutine());

        }

    }

    private IEnumerator AddPokemonToParty(Pokemon caughtPokemon)
    {
        yield return TypeDialog(PokemonParty.Instance.AddPokemon(caughtPokemon));
    }

    private IEnumerator AddPokemonToPokedex(Pokemon caughtPokemon)
    {
        PokedexManager.Instance.AddPokemonAsCaught(caughtPokemon.Base.PokedexNumber);
        yield return TypeDialog($"{caughtPokemon.Base.Name}'s data was added to the PokeDex!");
    }

    private void SetMoveButtons(List<Move> moves)
    {
        foreach (var button in moveButtons)
        {
            button.SetActive(false);
        }

        List<GameObject> activeButtons = new List<GameObject>();

        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            GameObject moveButton = moveButtons[i];
            SetMoveButton(moveButton, move);
            activeButtons.Add(moveButton);
        }

        SetupNavigation(activeButtons);
    }
    public void SwitchPokemon(int index)
    {
        isItTrue = true;
        IsPlayerTurn = IsFaintSwitching;
        StartCoroutine(SwitchPokemonRoutine(index));
    }
    public void PartyPokemonAction(int index)
    {
        Debug.Log($"is Switching: {isSwitching}, is Applying Item: {isApplyingItem}");
        if (isSwitching)
        {
            isApplyingItem = false;
            isSwitching = false;
            isItTrue = true;
            IsPlayerTurn = IsFaintSwitching;
            StartCoroutine(SwitchPokemonRoutine(index));
        }
        else if (isApplyingItem)
        {
            isSwitching = false;
            isApplyingItem = false;
            isItTrue = true;
            StartCoroutine(UseItemRoutine(index, MainBattleButtons.Instance.currentItem));
        }
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
        Pokemon pokemon = playerParty.Pokemons[index];
        Debug.Log($"Using {item.Name} on {pokemon.Base.Name}.");
        Debug.Log($"HP before: {pokemon.HP}");
        if (pokemon.HP == pokemon.MaxHP)
        {
            StartCoroutine(MainBattleButtons.Instance.TypeDialog("It won't have any effect.", false));
            isApplyingItem = true;
            yield break;
        }
        if ((item.Name == "Revive" || item.Name == "Max Revive") && pokemon.HP > 0)
        {
            StartCoroutine(MainBattleButtons.Instance.TypeDialog("It won't have any effect.", false));
            isApplyingItem = true;
            yield break;
        }
        if (pokemon != playerUnit.Pokemon)
        {
            dialogBox.GetComponent<BattleDialogBox>().SetDialogText("Waiting for opponent...");
            item.Use(pokemon);
            Inventory.Instance.RemoveItem(item.Name);
            yield return MainBattleButtons.Instance.TypeDialog($"You used {item.Name} on {pokemon.Base.Name}.", true);

            var slider = partyButtons[index].GetComponent<PartyButtonManager>().GetSlider();
            var text = partyButtons[index].GetComponent<PartyButtonManager>().GetText();
            var image = partyButtons[index].GetComponent<PartyButtonManager>().GetImage();
            yield return StartCoroutine(playerHud.GetComponent<StatGUI>().UpdateHP(pokemon, slider, text, image));
            yield return new WaitForSeconds(2f);
            MainBattleButtons.Instance.HidePartyScreen();
            StartCoroutine(EnemyMoveRoutine());
            yield break;
        }
        MainBattleButtons.Instance.HidePartyScreen();
        item.Use(pokemon);
        Inventory.Instance.RemoveItem(item.Name);
        yield return TypeDialog($"You used {item.Name} on {pokemon.Base.Name}.");

        yield return StartCoroutine(playerHud.GetComponent<StatGUI>().UpdateHP(pokemon));

        StartCoroutine(EnemyMoveRoutine());
    }
    public IEnumerator SwitchPokemonRoutine(int index)
    {
        var selectedPokemon = playerParty.Pokemons[index];

        if (selectedPokemon.HP <= 0)
        {
            yield return TypeDialog("You can't send out a fainted Pokemon!");
            yield break;
        }

        if (selectedPokemon == playerUnit.Pokemon)
        {
            yield return TypeDialog("You can't switch to the same Pokemon!");
            yield break;
        }

        playerUnit.Setup(selectedPokemon);
        playerHud.GetComponent<StatGUI>().SetData(selectedPokemon, true);
        pokemonParty.SetActive(false);
        MainBattleButtons.Instance.isInMoves = false;
        yield return TypeDialog($"Go, {selectedPokemon.Base.Name}!");


        yield return new WaitForSeconds(2.0f);



        if (IsPlayerTurn)
        {

            PlayerAction();
        }
        else
        {

            StartCoroutine(EnemyMoveRoutine());
        }
        cancelButton.SetActive(true);
        IsFaintSwitching = false;
    }
    private IEnumerator HandlePokemonFaint()
    {
        MainBattleButtons.Instance.SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
        yield return new WaitForSeconds(1f);
        IsFaintSwitching = true;
        isSwitching = true;
        cancelButton.SetActive(false);
        OpenPokemonPartyScreen();
        MainBattleButtons.Instance.SetDialogParty("Choose a Pokemon.");
        EventSystem.current.SetSelectedGameObject(firstPokemonButton);
    }
    private void OpenPokemonPartyScreen()
    {
        pokemonParty.SetActive(true);
    }
    private void SetupNavigation(List<GameObject> activeButtons)
    {
        if (activeButtons.Count <= 1) return;

        for (int i = 0; i < activeButtons.Count; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnUp = activeButtons[i > 0 ? i - 1 : activeButtons.Count - 1].GetComponent<UnityEngine.UI.Button>();

            nav.selectOnDown = activeButtons[i < activeButtons.Count - 1 ? i + 1 : 0].GetComponent<UnityEngine.UI.Button>();

            activeButtons[i].GetComponent<UnityEngine.UI.Button>().navigation = nav;
        }
    }



    private void SetMoveButton(GameObject moveButton, Move move)
    {
        moveButton.transform.Find("Move Name").GetComponent<TextMeshProUGUI>().text = move.Base.Name;
        moveButton.transform.Find("Move PP").GetComponent<TextMeshProUGUI>().text = $"{move.PP}/{move.Base.PP}";
        moveButton.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)Resources.Load($"{move.Base.Type.ToString().ToLower()}", typeof(Sprite));
        moveButton.SetActive(true);
        float effectiveness = GetMoveEffectiveness(move, enemyUnit.Pokemon.Base.Type1, enemyUnit.Pokemon.Base.Type2);
        TextMeshProUGUI effectivenessText = moveButton.transform.Find("Move Effectiveness").GetComponent<TextMeshProUGUI>();

        if (move.Base.Category == MoveCategory.Status)
        {
            effectivenessText.text = "Status";
            effectivenessText.color = ColorHex("#4700FF");
            return;
        }

        if (effectiveness == 0f)
        {
            effectivenessText.text = "Immune";
            effectivenessText.color = ColorHex("#800080");
        }
        else if (effectiveness > 1.0f)
        {
            effectivenessText.text = "Super Effective";
            effectivenessText.color = ColorHex("#1ECF26");
        }
        else if (effectiveness < 1.0f)
        {
            effectivenessText.text = "Not Very Effective";
            effectivenessText.color = ColorHex("#B73D3D");
        }
        else
        {
            effectivenessText.text = "Effective";
            effectivenessText.color = ColorHex("#D1931F");
        }
    }

    public Color ColorHex(string v)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(v, out color);
        return color;
    }

    private void DisableMoveButtons()
    {
        foreach (var button in moveButtons)
        {
            button.SetActive(false);
        }
    }

    private void UpdateHud(BattleUnit target)
    {
        if (target == playerUnit)
        {
            StartCoroutine(playerHud.GetComponent<StatGUI>().UpdateHP(target.Pokemon));
        }
        else if (target == enemyUnit)
        {
            StartCoroutine(enemyHud.GetComponent<StatGUI>().UpdateHP(target.Pokemon));
        }
    }

    private IEnumerator EndBattle(bool playerWins)
    {
        yield return new WaitForSeconds(1f);
        if (!playerWins)
        {
            string playerName = "You";
            int coinsLost = CalculateCoinsLost();

            yield return TypeDialog($"{playerName} are out of usable Pokemon!");
            yield return new WaitForSeconds(1f);

            if (!PokemonParty.Instance.IsWildBattle)
            {
                yield return TypeDialog($"{playerName} gave {coinsLost} coins to the foe!");

                yield return new WaitForSeconds(1f);
            }


            yield return TypeDialog("...");
            yield return new WaitForSeconds(1f);
            yield return TypeDialog($"{playerName} blacked out!");
            PokemonParty.Instance.HealAllPokemon();
        }
        else
        {
            EnemySpriteManager.Instance.AddTrainerAsDefeated(EnemySpriteManager.Instance.GetName());
            yield return TypeDialog("You won!");
        }
        yield return new WaitForSeconds(2f);
        var playerParty = PokemonParty.Instance;
        StartCoroutine(playerParty.CheckForEvolutions(() =>
        {
            StartCoroutine(SwitchToGame());
        }));

    }
    private IEnumerator SwitchToGame()
    {
        DontDestroyOnLoad(gameObject);

        yield return fader.FadeIn(0.5f);

        PlaySong(song);

        var sceneName = PokemonParty.Instance.IsWildBattle ? "Game" : EnemySpriteManager.Instance.GetSceneName();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return fader.FadeOut(0.5f);

        Destroy(gameObject);
    }


    private int CalculateCoinsLost()
    {
        //placeholder
        return 100;
    }


    private IEnumerator WaitForDialogueBox()
    {
        while (dialogBox.GetComponent<BattleDialogBox>().isTyping)
        {
            yield return null;
        }
    }

    private void PlayerAction()
    {
        Debug.Log("[BattleSystem] PlayerAction called");
        IsPlayerTurn = true;
        state = BattleState.PlayerAction;
        UpdateTurnCounter();
    }

    private void UpdateTurnCounter()
    {
        StartCoroutine(RunStatusEffects());
    }

    public IEnumerator FlashAfterAnimation(Pokemon pokemon)
    {
        var unit = pokemon == playerUnit.Pokemon ? playerUnit : enemyUnit;
        yield return new WaitForSeconds(1f);
        StartCoroutine(FlashSpriteWithFade(unit.GetComponent<UnityEngine.UI.Image>(), 1, 0.5f, unit.Pokemon.Status.FlashColor));
    }

    private IEnumerator FlashSprite(UnityEngine.UI.Image targetImage, int numberOfFlashes, float delay, Color flashColor)
    {
        Color originalColor = targetImage.color;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            targetImage.color = flashColor;
            yield return new WaitForSeconds(delay);
            targetImage.color = originalColor;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator FlashSpriteWithFade(UnityEngine.UI.Image targetImage, int numberOfFlashes, float delay, Color flashColor)
    {
        Color originalColor = targetImage.color;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            targetImage.DOColor(flashColor, delay / 2);
            yield return new WaitForSeconds(delay);
            targetImage.DOColor(originalColor, delay / 2);
            yield return new WaitForSeconds(delay);
        }
    }
    public void FlashAfter(Pokemon pokemon)
    {
        StartCoroutine(FlashAfterAnimation(pokemon));
    }
    private IEnumerator RunStatusEffects()
    {
        playerUnit.Pokemon.OnAfterTurn();
        enemyUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(playerUnit.Pokemon);
        yield return ShowStatusChanges(enemyUnit.Pokemon);
        UpdateHud(playerUnit);
        UpdateHud(enemyUnit);
        turnCounter.GetComponent<TextMeshProUGUI>().text = "Turn " + turn;
        turn++;
        UpdateCatchRate();
        StartCoroutine(EnablePlayerActions());
    }

    public void UpdateCatchRate(int shakes = 5)
    {
        catchRate = GetCurrentCatchRateInPercent(enemyUnit.Pokemon, new StandardPokeball());
        if (!DebugMenu.Instance.isCatchRate100) DebugMenu.Instance.UpdateDebugInfo("CatchRate", $"{this.catchRate.ToString("0.00")}% ({(shakes == 5 ? "Throw a PokeBall to calculate shakes" : shakes.ToString() + " shakes")})");
        else DebugMenu.Instance.UpdateDebugInfo("CatchRate", $"100% (Shakes: 4x)");
    }

    private IEnumerator EnablePlayerActions()
    {
        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
        SetMoveButtons(playerUnit.Pokemon.Moves);
        MainBattleButtons.Instance.ReturnToMenu(true);
    }

    public void PerformMove(int moveButtonIndex)
    {
        DisableMoveButtons();
        Move move = playerUnit.Pokemon.Moves[moveButtonIndex - 1];

        if (move.PP <= 0)
        {
            StartCoroutine(EnablePlayerActions());
            return;
        }

        move.PP--;
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}!");
        state = BattleState.Busy;
        StartCoroutine(PlayerMoveRoutine(move));
    }
    private int GetMostEffectiveMove(List<Move> moves)
    {
        int mostEffectiveMoveIndex = -1;
        float highestEffectiveness = 0f;
        int highestPower = 0;

        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            float effectiveness = GetMoveEffectiveness(move, playerUnit.Pokemon.Base.Type1, playerUnit.Pokemon.Base.Type2);
            int power = move.Base.Power;
            int pp = move.PP;

            if ((effectiveness > highestEffectiveness ||
                (effectiveness == highestEffectiveness && power > highestPower)) && pp > 0)
            {
                highestEffectiveness = effectiveness;
                highestPower = power;
                mostEffectiveMoveIndex = i;
            }
        }

        if (mostEffectiveMoveIndex != -1)
        {
            Debug.Log("Enemy's most effective move is: " + moves[mostEffectiveMoveIndex].Base.Name +
                      " with effectiveness: " + highestEffectiveness +
                      " and power: " + highestPower +
                      " and PP: " + moves[mostEffectiveMoveIndex].PP);
        }
        else
        {
            Debug.Log("No move available");
        }

        return mostEffectiveMoveIndex;
    }




    private IEnumerator EnemyMoveRoutine()
    {
        yield return new WaitForSeconds(1f);
        bool canRunMove = enemyUnit.Pokemon.OnBeforeMove();
        yield return ShowStatusChanges(enemyUnit.Pokemon);
        if (!canRunMove) {
                PlayerAction();
            yield break;
        }
        yield return WaitForDialogueBox();
        yield return new WaitForSeconds(1f);
        int enemyMoveIndex = PokemonParty.Instance.IsWildBattle ? UnityEngine.Random.Range(0, enemyUnit.Pokemon.Moves.Count) : GetMostEffectiveMove(enemyUnit.Pokemon.Moves);
        Move move = enemyUnit.Pokemon.Moves[enemyMoveIndex];
        Debug.Log("2. Enemy move: " + move.Base.Name);
        yield return TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        float effectiveness = GetMoveEffectiveness(move, playerUnit.Pokemon.Base.Type1, playerUnit.Pokemon.Base.Type2);
        bool isCritical = false;
        if (effectiveness > 0)
        {
            isCritical = UnityEngine.Random.Range(0, 24) == 0;
            enemyUnit.PlayAttackAnimation();
            StartCoroutine(FlashSprite(playerUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f, Color.black));
            playerUnit.Pokemon.TakeDamage(move, effectiveness, isCritical, enemyUnit.Pokemon);
            UpdateHud(playerUnit);
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("3. Effectiveness: " + effectiveness);
        if(move.Base.Category != MoveCategory.Status) yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, playerUnit.Pokemon.Base.Name));
        if (isCritical && move.Base.Category != MoveCategory.Status)
        {
            yield return new WaitForSeconds(1f);
            yield return TypeDialog($"A critical hit!");
        }

        if (playerUnit.Pokemon.HP <= 0)
        {
            yield return new WaitForSeconds(1f);
            yield return TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            playerUnit.PlayFaintAnimation();
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                IsPlayerTurn = true;
                yield return StartCoroutine(HandlePokemonFaint());
            }
            else
            {
                yield return StartCoroutine(EndBattle(false));
            }
        }
        else
        {
            PlayerAction();
        }
    }
    IEnumerator RunMoveEffects(Move move, Pokemon source, Pokemon target)
    {
        var effects = move.Base.Effects;
        if (effects.Status != ConditionID.none)
        {
            if (target.Status != null)
            {
                yield return new WaitForSeconds(1f);
                yield return TypeDialog($"But it failed!");
                yield break;
            }
            target.SetStatus(effects.Status);
            var statusName = ConditionsDB.Conditions[effects.Status].Name;
            if (target == enemyUnit.Pokemon)
            {
                enemyStatus.sprite = Resources.Load<Sprite>("Statuses/" + statusName);
                enemyStatus.gameObject.SetActive(true);
            }
            else
            {
                playerStatus.sprite = Resources.Load<Sprite>("Statuses/" + statusName);
                playerStatus.gameObject.SetActive(true);
            }
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return TypeDialog(message);
        }
    }
    private Color GetFlashColor(BattleUnit unit)
    {
        if (unit.Pokemon.Status == null) return Color.black;
        return unit.Pokemon.Status.FlashColor;
    }
    private IEnumerator PlayerMoveRoutine(Move move)
    {
        IsPlayerTurn = false;
        yield return StartCoroutine(WaitForDialogueBox());

        float effectiveness = GetMoveEffectiveness(move, enemyUnit.Pokemon.Base.Type1, enemyUnit.Pokemon.Base.Type2);
        Debug.Log("Effectiveness: " + effectiveness);
        bool isCritical = false;
        if ((effectiveness > 0 && !(move.Base.Category == MoveCategory.Status)) || (move.Base.Category == MoveCategory.Status && enemyUnit.Pokemon.Status == null))
        {
            isCritical = UnityEngine.Random.Range(0, 24) == 0;
            playerUnit.PlayAttackAnimation();
            StartCoroutine(FlashSprite(enemyUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f, Color.black));
            enemyUnit.Pokemon.TakeDamage(move, effectiveness, isCritical, playerUnit.Pokemon);
            UpdateHud(enemyUnit);
        }
        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, playerUnit.Pokemon, enemyUnit.Pokemon);
        }


        yield return new WaitForSeconds(1f);

        if (move.Base.Category != MoveCategory.Status) yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, enemyUnit.Pokemon.Base.Name));


        if (isCritical)
        {
            yield return new WaitForSeconds(1f);
            yield return TypeDialog($"A critical hit!");
        }

        if (enemyUnit.Pokemon.HP <= 0)
        {
            yield return new WaitForSeconds(1f);
            yield return TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
            int expYield = playerUnit.Pokemon.Base.ExpYield;
            int enemyLevel = enemyUnit.Pokemon.Level;
            int playerLevel = playerUnit.Pokemon.Level;
            float expGainMultiplier = Mathf.FloorToInt((expYield * enemyLevel) / 7);
            playerUnit.Pokemon.Exp += Mathf.FloorToInt(expGainMultiplier);
            yield return TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGainMultiplier} exp!");
            yield return playerHud.GetComponent<StatGUI>().SetExpSmooth(true);

            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerHud.GetComponent<StatGUI>().SetLevel();
                yield return playerHud.GetComponent<StatGUI>().SetExpSmooth(true);
                yield return TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}!");
            }



            yield return StartCoroutine(HandleEnemyPokemonFaint());
        }
        else
        {
            StartCoroutine(EnemyMoveRoutine());
        }
    }
    private IEnumerator HandleEnemyPokemonFaint()
    {
        yield return TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
        enemyUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);
        Pokemon nextPokemon = TrySendNextEnemyPokemon();
        if (nextPokemon != null && !PokemonParty.Instance.IsWildBattle)
        {
            enemyUnit.Setup(nextPokemon);
            enemyHud.GetComponent<StatGUI>().SetData(nextPokemon, false);
            yield return TypeDialog($"{EnemySpriteManager.Instance.GetName()} sent out {nextPokemon.Base.Name}!");
            yield return new WaitForSeconds(1f);
            PlayerAction();
        }
        else
        {
            yield return StartCoroutine(EndBattle(true));
        }
    }

    private Pokemon TrySendNextEnemyPokemon()
    {
        var nextPokemon = EnemySpriteManager.Instance.GetHealthyPokemon();
        return nextPokemon;
    }
    private IEnumerator ShowEffectivenessMessage(float effectiveness, string targetName)
    {
        if (effectiveness == 0f)
        {
            yield return TypeDialog("It does not affect " + targetName + "...");
        }
        else if (effectiveness > 1.0f)
        {
            yield return TypeDialog("It's super effective!");
        }
        else if (effectiveness < 1.0f)
        {
            yield return TypeDialog("It's not very effective...");
        }

        yield return StartCoroutine(WaitForDialogueBox());
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
}
