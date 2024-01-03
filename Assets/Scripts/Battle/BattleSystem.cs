using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;



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
    public int turn;
    private BattleState state;
    public AudioClip song;
    PokemonParty playerParty;
    Pokemon enemyPokemon;
    public bool isItTrue = false;
    public bool IsPlayerTurn;
    public int Shakes = 99;
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
    }
    private void Start()
    {
        StartBattle();
    }
    public void StartBattle()
    {
        Shakes = 99;
        turn = 1;
        var playerParty = PokemonParty.Instance;
        Debug.Log(playerParty);
        var enemyPokemon = MapArea.Instance.GetRandomWildPokemon();
        SetupBattle(playerParty, enemyPokemon);
    }
    float GetMoveEffectiveness(Move move, PokemonType opponentPrimaryType, PokemonType opponentSecondaryType)
    {
        float effectivenessPrimary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentPrimaryType);
        float effectivenessSecondary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentSecondaryType);

        return effectivenessPrimary * effectivenessSecondary;
    }

    private void SetupBattle(PokemonParty playerParty, Pokemon enemyPokemon)
    {


        this.playerParty = playerParty;
        this.enemyPokemon = enemyPokemon;
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(enemyPokemon);
        UpdateHudData();
        StartCoroutine(InitialDialog());
    }
    private IEnumerator InitialDialog()
    {
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog("A wild " + enemyUnit.Pokemon.Base.Name + " appeared!");
        MainBattleButtons.Instance.HideMainButtons();
        PlayerAction();
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

    public void TryCatchPokemon(Pokeball pokeballType)
    {
        Debug.Log("TryCatchPokemon called");
        float catchRate = CalculateCatchRate(enemyUnit.Pokemon, pokeballType);
        int shakes = CalculateShakes(catchRate);
        Debug.Log("Catch rate: " + catchRate + ", shakes: " + shakes);
        if(Shakes != 99) shakes = Shakes;
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
    }
    private IEnumerator HandlePokemonFaint()
    {
        MainBattleButtons.Instance.SetPokemonPartyButtons(PokemonParty.Instance.Pokemons);
        yield return new WaitForSeconds(1f);

        OpenPokemonPartyScreen();
        pokemonPartyDialogBox.GetComponent<BattleDialogBox>().TypeDialog("Choose a Pokemon.");
        cancelButton.SetActive(false);
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

            string playerName = "Martin";
            int coinsLost = CalculateCoinsLost();

            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerName} is out of usable Pokemon!");
            yield return StartCoroutine(WaitForDialogueBox());
            yield return new WaitForSeconds(1f);

            dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"{playerName} gave {coinsLost} coins to the foe!");
            yield return StartCoroutine(WaitForDialogueBox());

            yield return new WaitForSeconds(1f);

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
        state = BattleState.PlayerAction;
        UpdateTurnCounter();
        StartCoroutine(EnablePlayerActions());
    }

    private void UpdateTurnCounter()
    {
        turnCounter.GetComponent<TextMeshProUGUI>().text = "Turn " + turn;
        turn++;
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

    private IEnumerator EnemyMoveRoutine()
    {
        yield return new WaitForSeconds(1f);

        int enemyMoveIndex = UnityEngine.Random.Range(0, enemyUnit.Pokemon.Moves.Count);
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
