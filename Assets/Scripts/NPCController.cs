using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour, Interactable
{
    public GameObject dialogBox;
    public TextAsset dialogueTextFile; 
    public bool isTalking = false;
    public bool isTrainer = false;
    public bool isProfessor = false; 
    public bool isSmallProfessor = false; 
    public GameObject profScreen; 
    private Fader fader;
    [SerializeField] public Pokemon heldItem = null;
    private float skipCooldown = 0f;



    private List<string> dialogues = new List<string>(); 

    private void Start()
    {
        if (dialogBox == null)
        {
            dialogBox = InteractableDialogManager.Instance.dialogBox;
        }
        fader = FindObjectOfType<Fader>();
        ReadDialoguesFromFile();

        if (isProfessor && !CharacterValueManager.Instance.SpokeToProfessor) 
        {
            StartCoroutine(InvokeDialog()); 
        }
    }
    void Update()
    {
        var dialogBoxComponent = dialogBox.GetComponent<BattleDialogBox>();
        if (skipCooldown > 0)
        {
            skipCooldown -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButtonDown("RB"))
            {
                if (isProfessor)
                {
                    StartCoroutine(DestroyProfessor());



                }
            }
            else if (Input.GetButtonDown("Submit") && dialogBoxComponent.isTyping)
            {
                dialogBoxComponent.CompleteDialogText();
                skipCooldown = 0.3f;
            }
        }
    }
    private IEnumerator DestroyProfessor()
    {
        var prof = profScreen ? profScreen : GameObject.FindWithTag("ProfScreen");
        yield return fader.FadeIn(0.5f);
        SceneManager.LoadSceneAsync("House");
        StartCoroutine(fader.FadeOut(0.5f));
        Destroy(prof);
        isTalking = false;
        dialogBox.SetActive(false);
    }










    private void ReadDialoguesFromFile()
    {
        if (dialogueTextFile != null)
        {
            string[] lines = dialogueTextFile.text.Split('\n');
            foreach (string line in lines)
            {
                dialogues.Add(line);
            }
        }
    }

    public void Interact()
    {
        if (dialogBox == null)
        {
            Debug.LogError("Dialog box not found.");
            return;
        }

        if (!isTalking) StartCoroutine(InvokeDialog());
    }

    public bool IsTalking()
    {
        return isTalking;
    }
    public bool IsTrainer()
    {
        return isTrainer;
    }

    private IEnumerator InvokeDialog()
    {
        dialogBox.SetActive(true);
        isTalking = true;

        foreach (string dialogue in dialogues)
        {
            if (dialogue.Contains("This world is widely inhabited by creatures known as Pokemon."))
            {
                
                yield return StartCoroutine(SpecialAction());
            }
            else if (dialogue.Contains("Now, are you ready?"))
            {
                yield return StartCoroutine(SpecialAction2());
            }
            dialogBox.GetComponent<BattleDialogBox>().TypeDialog(dialogue);
            yield return StartCoroutine(WaitForDialogueBox());
            dialogBox.GetComponent<BattleDialogBox>().instantComplete = false;
        }
        if (isProfessor)
        {
            var prof = profScreen ? profScreen : GameObject.FindWithTag("ProfScreen");

            
            yield return fader.FadeIn(0.5f);
            SceneManager.LoadSceneAsync("House");
            
            StartCoroutine(fader.FadeOut(0.5f));

            
            Destroy(prof);

            
            
        }
        if (isSmallProfessor && !CharacterValueManager.Instance.SpokeToProfessor)
        {
            Debug.Log("Added " + heldItem.Base.Name + " to the party.");
            PokemonParty.Instance.AddPokemonV(heldItem);
            PokedexManager.Instance.AddPokemonAsCaught(heldItem.Base.PokedexNumber);
            CharacterValueManager.Instance.SpokeToProfessor = true;

        }
        isTalking = false;
        dialogBox.SetActive(false);
    }
    private IEnumerator SpecialAction()
    {
        
        float moveAmount = 350; 
        
        yield return transform.DOMoveX(transform.position.x + moveAmount, 1f);
        
        yield return new WaitForSeconds(1f);
        yield return GameObject.FindWithTag("ProfessorBall").GetComponent<Image>().DOFade(1f, 0);
        yield return new WaitForSeconds(0.5f);
        yield return GameObject.FindWithTag("ProfessorBall").GetComponent<Image>().DOFade(0, 1f);
        yield return GameObject.FindWithTag("Professor").GetComponent<Image>().DOFade(1f, 1f);

        
        

        
        yield return new WaitForSeconds(1f); 
    }
    private IEnumerator SpecialAction2()
    {
        
        float moveAmount = 350; 
        
        yield return GameObject.FindWithTag("Professor").GetComponent<Image>().DOFade(0f, 1f);
        yield return new WaitForSeconds(1f);
        yield return transform.DOMoveX(transform.position.x - moveAmount, 1f);
        

        
        

        
        yield return new WaitForSeconds(1f); 
    }

    private IEnumerator WaitForDialogueBox()
    {
        BattleDialogBox battleDialogBox = dialogBox.GetComponent<BattleDialogBox>();

        yield return new WaitUntil(() => !battleDialogBox.isTyping);

        yield return new WaitUntil(() => skipCooldown <= 0 && (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Submit")));
    }




}
