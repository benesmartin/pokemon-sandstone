using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private int lettersPerSecond;

    public bool isTyping;

    public event Action DialogCompleted;

    private Coroutine typeDialogCoroutine;

    public void TypeDialog(string dialog, bool allowSkipping = true)
    {
        if (isTyping && allowSkipping)
        {
            StopCoroutine(typeDialogCoroutine);
            dialogText.text = dialog;
            isTyping = false;
            DialogCompleted?.Invoke();
        }
        else if (!isTyping)
        {
            typeDialogCoroutine = StartCoroutine(TypeDialogCoroutine(dialog));
        }
    }

    private IEnumerator TypeDialogCoroutine(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
        DialogCompleted?.Invoke();
    }
}


