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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInMoves)
            {
                GameObject.Find("Move Buttons").SetActive(false);
                mainButtons.SetActive(true);
                EventSystem.current.SetSelectedGameObject(GameObject.Find("Battle Button"));
                isInMoves = false;
            }
        }
    }
    public int escapeAttempts = 0;
    public void SelectMoveButtons()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Move 1 Button"));
        isInMoves = true;
    }
    public void SelectMainButtons()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Battle Button"));
    }
    public void OnFightButton()
    {
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Choose a move!");
    }
    public void OnRunButton()
    {
        int F = (((playerUnit.Pokemon.Speed * 128) / enemyUnit.Pokemon.Speed) + 30 * escapeAttempts) % 256;
        if (Random.Range(0, 256) < F)
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Got away safely!");
            StartCoroutine(WaitForDialogBox());
        }
        else
        {
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog("Can't escape!");
            escapeAttempts++;
        }

    }
    IEnumerator WaitForDialogBox()
    {
        while (dialogBox.GetComponent<BattleDialogBox>().isTyping)
        {
            yield return null;
        }
        SceneManager.LoadScene("Game");
    }
}
