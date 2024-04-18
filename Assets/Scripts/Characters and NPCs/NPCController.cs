using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;

[System.Serializable]
public class TradeOffer
{
    public Pokemon wantedPokemon;
    public Pokemon offeredPokemon;
}

public class NPCController : MonoBehaviour, Interactable
{
    public static NPCController Instance { get; private set; }
    public InteractionType interactionType;
    public TradeOffer tradeOffer;
    public bool canTrade = false;
    public GameObject dialogBox;
    public TextAsset dialogueTextFile;
    public TextAsset tradeSuccessDialogueFile; // New dialogue file for successful trade
    public TextAsset tradeFailDialogueFile; // New dialogue file for failed trade
    public bool tradeCompletedSuccessfully = false;
    public bool isTalking = false;
    public bool isTrainer = false;
    public bool isProfessor = false; 
    public bool isSmallProfessor = false; 
    public GameObject profScreen;
    public static bool CanInteract = true;
    public bool videoFinished = false;
    private Fader fader;
    [SerializeField] public Pokemon heldItem = null;
    [SerializeField] public Sprite trainerSprite;
    [SerializeField] public string trainerName, sceneName;
    [SerializeField] public List<Pokemon> pokemons;
    private float skipCooldown = 0f;



    private List<string> dialogues = new List<string>();
    private void Awake()
    {
        if (dialogBox == null)
        {
            dialogBox = InteractableDialogManager.Instance.dialogBox;
        }
        fader = FindObjectOfType<Fader>();
        ChooseDialogueBasedOnType();
    }
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
        ChooseDialogueBasedOnType();
    }

    private void ChooseDialogueBasedOnType()
    {
        switch (interactionType)
        {
            case InteractionType.Dialogue:
                Debug.Log("Trade completed successfully: " + tradeCompletedSuccessfully);
                if (tradeCompletedSuccessfully)
                {
                    ReadDialoguesFromFile(tradeSuccessDialogueFile);
                }
                else
                {
                    ReadDialoguesFromFile(dialogueTextFile);

                }
                break;
            case InteractionType.Battle:

                Debug.Log("Battle outcome: " + EnemySpriteManager.Instance.GetBattleOutcome());
                if (EnemySpriteManager.Instance.IsTrainerDefeated(trainerName))
                {
                    ReadDialoguesFromFile(tradeSuccessDialogueFile);
                    if (EnemySpriteManager.Instance.GetName().Contains("Gym")) SaveDataManager.Instance.CheckGymBadge(EnemySpriteManager.Instance.GetName());
                    isTrainer = false;
                    return;
                }
                else
                {
                    ReadDialoguesFromFile(dialogueTextFile);
                }
                break;
            case InteractionType.Trade:
                if (tradeCompletedSuccessfully)
                {
                    ReadDialoguesFromFile(tradeSuccessDialogueFile); 
                }
                else
                {
                    //InitiateTrade();
                }
                break;
            default:
                ReadDialoguesFromFile(dialogueTextFile); 
                break;
        }
    }


    public void Interact()
    {
        if (!CanInteract) return;
        if (dialogBox == null)
        {
            Debug.LogError("Dialog box not found.");
            return;
        }
        Debug.Log($"[NPCController] Interaction type: {interactionType.ToString().SetBold()}");

        switch (interactionType)
        {
            case InteractionType.Dialogue:
                if (!isTalking) StartCoroutine(InvokeDialog());
                break;
            case InteractionType.Trade:
                if (isTalking) return;
                if (!tradeCompletedSuccessfully)
                {
                    PlayerMovement.Instance.shouldStopPlayer = true;
                    PlayerMovement.Instance.ignoreNextInput = false;
                    ModalManager.Instance.IsModalActive = true;
                    
                    ModalManager.Instance.modalPanel.Show(UIFocusManager.Instance.eventSystem, "Would you like to trade your " + tradeOffer.wantedPokemon.Base.Name.SetColor("#D4AF37") + " for my " + tradeOffer.offeredPokemon.Base.Name.SetColor("#C3B1E1") + "?",
                        () => {
                            ModalManager.Instance.IsModalActive = false;
                            PlayerMovement.Instance.ignoreNextInput = true;
                            ModalManager.Instance.modalPanel.Hide();
                            CheckPlayerHasPokemon();
                        },
                        () => {
                            ModalManager.Instance.IsModalActive = false;
                            PlayerMovement.Instance.ignoreNextInput = true;
                            ModalManager.Instance.modalPanel.Hide();
                            PlayerMovement.Instance.shouldStopPlayer = false;
                            StartCoroutine(DisplayTradeRefusalDialog("Oh, that's a pity."));
                        });
                }
                else
                {
                    ReadDialoguesFromFile(tradeSuccessDialogueFile);
                    StartCoroutine(InvokeDialog());
                }
                break;
            case InteractionType.Battle:
                if (!isTalking)
                {
                    isTalking = true;
                    Debug.Log($"Trainer name: {trainerName}, EnemySpriteManager name: {EnemySpriteManager.Instance.GetName()}");
                    EnemySpriteManager.Instance.SetSprite(trainerSprite);
                    EnemySpriteManager.Instance.SetName(trainerName);
                    EnemySpriteManager.Instance.SetPokemons(pokemons);
                    EnemySpriteManager.Instance.SetSceneName(sceneName);
                    StartCoroutine(InvokeDialog());
                }
                break;
            default:
                Debug.LogError("Unhandled interaction type.");
                break;
        }

    }
    private IEnumerator InitiateBattleAfterDialogue()
    {
        if (isTalking)
        {
            yield return new WaitUntil(() => !isTalking); 
        }
        Debug.Log("Starting battle...");
    }

    IEnumerator TradeAndSaveSequence()
    {
        yield return StartCoroutine(TradeAnimation());
        PokemonParty.Instance.AddPokemonV(tradeOffer.offeredPokemon);
        PokemonParty.Instance.RemovePokemonByName(tradeOffer.wantedPokemon.Base.Name);
        PokedexManager.Instance.AddPokemonAsCaught(tradeOffer.offeredPokemon.Base.PokedexNumber);
        tradeCompletedSuccessfully = true;
        ReadDialoguesFromFile(tradeSuccessDialogueFile);
        yield return StartCoroutine(DisplayTradeRefusalDialog($"{tradeOffer.offeredPokemon.Base.Name}'s data was added to the Pokedex!"));
    }
    IEnumerator TradeAnimation()
    {
        yield return fader.FadeIn(0.5f);
        yield return new WaitForSeconds(1f);
        var transition = TradingManager.Instance.GetTransitionScreen();
        var video = TradingManager.Instance.GetTransitionPlayer().GetComponent<VideoPlayer>();
        if (transition == null)
        {
            Debug.Log($"2Transition: {transition}, Video: {video}");
            transition = TradingManager.Instance.GetTransitionScreen();
            Debug.Log($"3Transition: {transition}, Video: {video}");
            Debug.Log(transition == null ? "Transition object not found." : "Transition object found.");
        }

        if (transition != null)
        {
            Debug.Log($"Transition object is active: {transition.activeInHierarchy}");
            transition.SetActive(true);
        }
        else
        {
            Debug.LogError("Transition object is null when trying to activate.");
        }
        if (video == null) video = TradingManager.Instance.GetTransitionPlayer().GetComponent<VideoPlayer>();

        video.Play();
        video.loopPointReached += CheckOver;
        while (!videoFinished)
        {
            yield return null;
        }
        transition.SetActive(false);
        yield return fader.FadeOut(0.5f);
    }
    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        videoFinished = true;
        Debug.Log("Video finished.");
    }
    private void CheckPlayerHasPokemon()
    {
        if (PokemonParty.Instance.HasPokemon(tradeOffer.wantedPokemon.Base.Name))
        {

            StartCoroutine(TradeAndSaveSequence());
        }
        else
        {
            StartCoroutine(DisplayTradeRefusalDialog($"I appreciate it, but you don't have {tradeOffer.wantedPokemon.Base.Name.SetColor("#D4AF37")}!"));
        }
    }

    IEnumerator DisplayTradeRefusalDialog(string message)
    {
        dialogBox.SetActive(true);
        dialogBox.GetComponent<BattleDialogBox>().SetDialogText("");
        isTalking = true;
        yield return new WaitForSeconds(0.01f);
        dialogBox.GetComponent<BattleDialogBox>().instantComplete = false;
        dialogBox.GetComponent<BattleDialogBox>().TypeDialog(message);
        yield return StartCoroutine(WaitForDialogueBox());
        isTalking = false;
        dialogBox.SetActive(false);
    }

    public void StartProfessor()
    {
        if (isProfessor && !CharacterValueManager.Instance.SpokeToProfessor)
        {
            StartCoroutine(InvokeDialog());
        }
    }

    void Update()
    {
        var dialogBoxComponent = InteractableDialogManager.Instance.dialogBox.GetComponent<BattleDialogBox>();

        if (skipCooldown > 0)
        {
            skipCooldown -= Time.deltaTime;
        }

        if (skipCooldown <= 0)
        {
            if (Input.GetButtonDown("RB"))
            {
                if (isProfessor)
                {
                    Debug.Log("RB pressed, starting DestroyProfessor coroutine.");
                    StartCoroutine(DestroyProfessor());
                }
            }
            else if (Input.GetButtonDown("Submit"))
            {
                Debug.Log($"Submit pressed. Is typing: {dialogBoxComponent.isTyping}");
                if (!dialogBoxComponent.isTyping)
                {
                    Debug.Log("Submit pressed while not typing.");
                }
                if (dialogBoxComponent.isTyping)
                {
                    Debug.Log("Submit pressed while typing.");
                    dialogBoxComponent.CompleteDialogText();
                    skipCooldown = 0.3f;
                }
            }
        }
    }

    private IEnumerator DestroyProfessor()
    {
        CharacterValueManager.Instance.SpokeToProfessor = true;
        ProfessorManager.Instance.isProfessorScreenActive = false;
        var prof = profScreen ? profScreen : GameObject.FindWithTag("ProfScreen");
        yield return fader.FadeIn(0.5f);
        SceneManager.LoadSceneAsync("House");
        StartCoroutine(fader.FadeOut(0.5f));
        isTalking = false;
        dialogBox.SetActive(false);
        prof.SetActive(false);
    }










    private void ReadDialoguesFromFile(TextAsset file)
    {
        Debug.Log($"Reading dialogues from file: {file.name}");
        dialogues.Clear();

        if (file != null)
        {
            string[] lines = file.text.Split('\n');
            foreach (string line in lines)
            {
                dialogues.Add(line);
            }
        }
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
        dialogBox.GetComponent<BattleDialogBox>().SetDialogText("");
        yield return new WaitForSeconds(0.01f);
        isTalking = true;
        List<string> tempDialogues = new List<string>(dialogues);

        Debug.Log($"list: {string.Join(", ", tempDialogues)}");
        foreach (string dialogue in tempDialogues)
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

            CharacterValueManager.Instance.SpokeToProfessor = true;
            yield return fader.FadeIn(0.5f);
            SceneManager.LoadSceneAsync("House");
            
            StartCoroutine(fader.FadeOut(0.5f));

            
            prof.SetActive(false);

            
            
        }
        if (isSmallProfessor && !CharacterValueManager.Instance.SpokeToSmallProfessor)
        {
            Debug.Log("Added " + heldItem.Base.Name + " to the party.");
            PokemonParty.Instance.AddPokemonV(heldItem);
            PokedexManager.Instance.AddPokemonAsCaught(heldItem.Base.PokedexNumber);
            CharacterValueManager.Instance.SpokeToSmallProfessor = true;
            ReadDialoguesFromFile(tradeSuccessDialogueFile);
            tradeCompletedSuccessfully = true;
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
        PlayerMovement.Instance.shouldStopPlayer = true;    
        BattleDialogBox battleDialogBox = dialogBox.GetComponent<BattleDialogBox>();

        yield return new WaitUntil(() => !battleDialogBox.isTyping);

        yield return new WaitForSeconds(0.3f);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Submit"));

        skipCooldown = 0.3f;
        PlayerMovement.Instance.shouldStopPlayer = false;
    }


    public enum InteractionType
    {
        Dialogue,
        Trade,
        Battle
    }
}
