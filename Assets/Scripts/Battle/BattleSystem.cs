using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
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
    public int turn;
    private BattleState state;
    public AudioClip song;

    private void Start()
    {
        SetupBattle();
    }
    float GetMoveEffectiveness(Move move, PokemonType opponentPrimaryType, PokemonType opponentSecondaryType)
    {
        float effectivenessPrimary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentPrimaryType);
        float effectivenessSecondary = PokemonTypeEffectivenessChart.Instance.GetEffectiveness(move.Base.Type, opponentSecondaryType);

        return effectivenessPrimary * effectivenessSecondary;
    }

    private void SetupBattle()
    {
        PlayerAction();
        playerUnit.Setup();
        enemyUnit.Setup();
        UpdateHudData();
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
    }
    private void UpdateHudData()
    {
        playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon);
        enemyHud.GetComponent<StatGUI>().SetData(enemyUnit.Pokemon);
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
        string resultMessage = playerWins ? "You won!" : "You lost!";
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog(resultMessage);
        yield return StartCoroutine(WaitForDialogueBox());

        yield return new WaitForSeconds(1f);
        PlaySong(song);
        SceneManager.LoadScene("Game");
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
        state = BattleState.PlayerAction;
        StartCoroutine(EnablePlayerActions());
    }

    private IEnumerator EnablePlayerActions()
    {
        yield return StartCoroutine(WaitForDialogueBox());
        yield return new WaitForSeconds(1f);
        SetMoveButtons(playerUnit.Pokemon.Moves);
        MainBattleButtons.Instance.ReturnToMenu();
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
        
        StartCoroutine(FlashSprite(playerUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f));
        
        playerUnit.Pokemon.TakeDamage(move, effectiveness);
        UpdateHud(playerUnit);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, playerUnit.Pokemon.Base.Name));

        if (playerUnit.Pokemon.HP <= 0)
        {
            StartCoroutine(EndBattle(false));
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
        
        StartCoroutine(FlashSprite(enemyUnit.GetComponent<UnityEngine.UI.Image>(), 3, 0.1f));

       
        enemyUnit.Pokemon.TakeDamage(move, effectiveness);
        UpdateHud(enemyUnit);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowEffectivenessMessage(effectiveness, playerUnit.Pokemon.Base.Name));

        if (enemyUnit.Pokemon.HP <= 0)
        {
            StartCoroutine(EndBattle(true));
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
