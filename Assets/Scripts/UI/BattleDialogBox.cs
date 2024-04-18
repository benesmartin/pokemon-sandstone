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
    private string dialog;
    public bool instantComplete = false;

    public void CompleteDialogText()
    {
        if (isTyping)
        {
            if (typeDialogCoroutine != null)
            {
                StopCoroutine(typeDialogCoroutine);
            }

            dialogText.text = dialog;
            isTyping = false;
            instantComplete = true;
        }
    }

    public void TypeDialog(string dialog, bool allowSkipping = true)
    {
        this.dialog = dialog;
        Debug.Log("TypeDialog called with dialog: " + dialog);
        if (isTyping && allowSkipping)
        {
            Debug.Log("Typing in progress, stopping and setting text instantly.");
            StopCoroutine(typeDialogCoroutine);
            dialogText.text = dialog;
            isTyping = false;
            DialogCompleted?.Invoke();
        }
        else if (!isTyping)
        {
            Debug.Log("Starting new typing coroutine.");
            typeDialogCoroutine = StartCoroutine(TypeDialogCoroutine(dialog));
        }
    }
    private IEnumerator TypeDialogCoroutine(string dialog)
    {
        isTyping = true;
        int tagOpenIndex = 0;
        bool isTag = false;

        dialogText.text = "";

        for (int i = 0; i < dialog.Length; i++)
        {
            if (dialog[i] == '<')
            {
                isTag = true;
                tagOpenIndex = i;
            }
            if (dialog[i] == '>' && isTag)
            {
                isTag = false;
                dialogText.text += dialog.Substring(tagOpenIndex, i - tagOpenIndex + 1);
                continue;
            }

            if (!isTag)
            {
                dialogText.text += dialog[i];
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }
        }

        isTyping = false;
        DialogCompleted?.Invoke();
    }

    public void SetDialogText(string dialog)
    {
        dialogText.text = dialog;
    }
}


