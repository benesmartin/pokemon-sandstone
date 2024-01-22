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



public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject enemyHud;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject[] moveButtons;
    [SerializeField] private GameObject pokemonParty;
    [SerializeField] private GameObject pokemonPartyDialogBox;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameObject turnCounter;
    [SerializeField] private GameObject firstPokemonButton;
    public int turn;
    private BattleState state;
    public AudioClip song;
    PokemonParty playerParty;
    Pokemon enemyPokemon;
    public bool isItTrue = false;
    public bool IsPlayerTurn;
    public bool IsFaintSwitching = false;
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
            playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon);
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
        StartBattle(PokemonParty.Instance.IsWildBattle);
    }
    public void StartBattle(bool isWildBattle)
    {
        Shakes = 99;
        turn = 1;
        var playerParty = PokemonParty.Instance;
        var enemyPokemon = isWildBattle ? MapArea.Instance.GetRandomWildPokemon() : playerParty.GetHealthyEnemyPokemon();
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

        Debug.Log($"Setting up player unit with {playerParty.GetHealthyPokemon()?.Base?.Name}");
        Debug.Log($"Setting up enemy unit with {enemyPokemon?.Base?.Name}");

        if (playerParty.GetHealthyPokemon() == null)
        {
            Debug.LogError("GetHealthyPokemon returned null.");
        }

        if (enemyPokemon == null)
        {
            Debug.LogError("GetRandomWildPokemon returned null or there is no healthy enemy Pokemon.");
        }

        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(enemyPokemon);
        UpdateHudData();
        StartCoroutine(InitialDialog());
    }

    float GetMoveEffectiveness(Move move, PokemonType opponentPrimaryType, PokemonType opponentSecondaryType)
    {
        float effectivenessPrimary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentPrimaryType);
        float effectivenessSecondary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentSecondaryType);

        return effectivenessPrimary * effectivenessSecondary;
    }
    private IEnumerator InitialDialog()
    {
        MainBattleButtons.Instance.HideMainButtons();
        
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
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("A wild " + enemyUnit.Pokemon.Base.Name + " appeared!");

            yield return StartCoroutine(WaitForDialogueBox());
            foreach (var item in enemyPokemonGUI)
            {
                item.GetComponent<Image>().DOFade(1, 0.5f);
                item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
            }
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Go,  " + playerUnit.Pokemon.Base.Name + "!");
        } 
        else
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("You are challenged by Trainer Girl" + "!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1.5f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Trainer Girl sent out " + enemyUnit.Pokemon.Base.Name +  "!");
            
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);
            foreach (var item in enemyPokemonGUI)
            {
                item.GetComponent<Image>().DOFade(1, 0.5f);
                item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
            }
            yield return new WaitForSeconds(0.75f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Go,  " + playerUnit.Pokemon.Base.Name + "!");
        }
        yield return new WaitForSeconds(2f);
        PlayerAction();
        foreach (var item in playerPokemonGUI)
        {
            item.GetComponent<Image>().DOFade(1, 0.5f);
            item.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
        }
        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
    }
    private void UpdateHudData()
    {
        playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon);
        enemyHud.GetComponent<StatGUI>().SetData(enemyUnit.Pokemon);
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
                dialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
                yield return StartCoroutine(WaitForDialogueBox());
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
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog(PokemonParty.Instance.AddPokemon(caughtPokemon));
        yield return WaitForDialogueBox();
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
    public IEnumerator SwitchPokemonRoutine(int index)
    {
        var selectedPokemon = playerParty.Pokemons[index];

        if (selectedPokemon.HP <= 0)
        {
            pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("You can't send out a fainted Pokemon!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield break;
        }

        if (selectedPokemon == playerUnit.Pokemon)
        {
            pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("You can't switch to the same Pokemon!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield break;
        }

        playerUnit.Setup(selectedPokemon);
        playerHud.GetComponent<StatGUI>().SetData(selectedPokemon);
        pokemonParty.SetActive(false);
        MainBattleButtons.Instance.isInMoves = false;
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"Go, {selectedPokemon.Base.Name}!");


        yield return StartCoroutine(WaitForDialogueBox());


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
        cancelButton.SetActive(false);
        OpenPokemonPartyScreen();
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("Choose a Pokemon.");
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

    private Color ColorHex(string v)
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
            playerHud.GetComponent<StatGUI>().UpdateHP(target.Pokemon);
        }
        else if (target == enemyUnit)
        {
            enemyHud.GetComponent<StatGUI>().UpdateHP(target.Pokemon);
        }
    }

    private IEnumerator EndBattle(bool playerWins)
    {
        yield return new WaitForSeconds(1f);
        if (!playerWins)
        {

            string playerName = "You";
            int coinsLost = CalculateCoinsLost();

            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerName} are out of usable Pokemon!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);

            if (!PokemonParty.Instance.IsWildBattle)
            {
                dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerName} gave {coinsLost} coins to the foe!");
                yield return StartCoroutine(WaitForDialogueBox());

                yield return new WaitForSeconds(1f);
            }
            

            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("...");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerName} blacked out!");
            yield return StartCoroutine(WaitForDialogueBox());
            PokemonParty.Instance.HealAllPokemon();
        }
        else
        {

            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("You won!");
            yield return StartCoroutine(WaitForDialogueBox());
        }

        yield return new WaitForSeconds(1f);
        PlaySong(song);
        SceneManager.LoadScene("Game");
    }

    private int CalculateCoinsLost()
    {

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
        StartCoroutine(EnablePlayerActions());
    }

    private void UpdateTurnCounter()
    {
        turnCounter.GetComponent<TextMeshProUGUI>().text = "Turn " + turn;
        turn++;
        UpdateCatchRate();
    }

    public void UpdateCatchRate(int shakes = 5)
    {
        this.catchRate = GetCurrentCatchRateInPercent(enemyUnit.Pokemon, new StandardPokeball());
        if (!DebugMenu.Instance.isCatchRate100) DebugMenu.Instance.UpdateDebugInfo("CatchRate", $"{this.catchRate.ToString("0.00")}% ({(shakes == 5 ? "Throw a Pok�Ball to calculate shakes" : shakes.ToString() + " shakes")})");
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
        int enemyMoveIndex = PokemonParty.Instance.IsWildBattle ? UnityEngine.Random.Range(0, enemyUnit.Pokemon.Moves.Count) : GetMostEffectiveMove(enemyUnit.Pokemon.Moves);
        Move move = enemyUnit.Pokemon.Moves[enemyMoveIndex];

        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        yield return StartCoroutine(WaitForDialogueBox());

        float effectiveness = GetMoveEffectiveness(move, playerUnit.Pokemon.Base.Type1, playerUnit.Pokemon.Base.Type2);
        bool isCritical = false;
        if (effectiveness > 0)
        {
            isCritical = UnityEngine.Random.Range(0, 24) == 0;
            enemyUnit.PlayAttackAnimation();
            StartCoroutine(FlashSprite(playerUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f));
            playerUnit.Pokemon.TakeDamage(move, effectiveness, isCritical, enemyUnit.Pokemon);
            UpdateHud(playerUnit);
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, playerUnit.Pokemon.Base.Name));
        if (isCritical)
        {
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"A critical hit!");

            yield return StartCoroutine(WaitForDialogueBox());
        }

        if (playerUnit.Pokemon.HP <= 0)
        {
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            yield return StartCoroutine(WaitForDialogueBox());
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

    private IEnumerator PlayerMoveRoutine(Move move)
    {
        IsPlayerTurn = false;
        yield return StartCoroutine(WaitForDialogueBox());

        float effectiveness = GetMoveEffectiveness(move, enemyUnit.Pokemon.Base.Type1, enemyUnit.Pokemon.Base.Type2);
        bool isCritical = false;
        if (effectiveness > 0)
        {
            isCritical = UnityEngine.Random.Range(0, 24) == 0;
            playerUnit.PlayAttackAnimation();
            StartCoroutine(FlashSprite(enemyUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f));
            enemyUnit.Pokemon.TakeDamage(move, effectiveness, isCritical, playerUnit.Pokemon);
            UpdateHud(enemyUnit);
        }


        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, playerUnit.Pokemon.Base.Name));


        if (isCritical)
        {
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"A critical hit!");

            yield return StartCoroutine(WaitForDialogueBox());
        }

        if (enemyUnit.Pokemon.HP <= 0)
        {
            yield return new WaitForSeconds(1f);
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
            yield return StartCoroutine(WaitForDialogueBox());
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(EndBattle(true));
        }
        else
        {
            StartCoroutine(EnemyMoveRoutine());
        }
    }
    private IEnumerator ShowEffectivenessMessage(float effectiveness, string targetName)
    {
        if (effectiveness == 0f)
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("It does not affect " + targetName + "...");
        }
        else if (effectiveness > 1.0f)
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("It's super effective!");
        }
        else if (effectiveness < 1.0f)
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("It's not very effective...");
        }

        yield return StartCoroutine(WaitForDialogueBox());
    }
    private IEnumerator FlashSprite(UnityEngine.UI.Image targetImage, int numberOfFlashes, float delay)
    {
        Color originalColor = targetImage.color;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            targetImage.color = Color.black;
            yield return new WaitForSeconds(delay);
            targetImage.color = originalColor;
            yield return new WaitForSeconds(delay);
        }
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
