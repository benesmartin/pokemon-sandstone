using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] GameObject playerHud;
    [SerializeField] GameObject enemyHud;
    [SerializeField] GameObject dialogBox;
    private void Start()
    {
        SetupBattle();
    }
    private void SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.GetComponent<StatGUI>().SetData(playerUnit.Pokemon);
        enemyHud.GetComponent<StatGUI>().SetData(enemyUnit.Pokemon);
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog("A wild " + enemyUnit.Pokemon.Base.Name + " appeared!");
    }

}
