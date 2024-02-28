using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class PlayerMovement : MonoBehaviour
{
    public VideoPlayer video;
    public GameObject transition;
    public float moveSpeed = 5f;
    public float tileSize = 1f;
    public float walkingSpeed = 4f;
    public float runningSpeed = 8f;
    public float bikingSpeed = 10f;
    public Rigidbody2D rb;
    public Animator animator;
    private Vector2 input;
    bool isMoving = false;
    public float currentSpeed = 0f;
    public float lastHorizontal = 0f;
    public float lastVertical = -1f;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;
    public LayerMask portalLayer;
    public bool isTransitioning = false;
    public AudioClip song;
    public bool isCutscenePlaying = false;
    public bool isNPCTalking = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    public GameObject text;
    private Vector2 lastInput;
    public float PosX = 0;
    public float PosY = 0;
    private bool isRunning = false;
    private bool isBiking = false;
    public bool isPaused = false;
    public static PlayerMovement Instance { get; private set; }

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
    private void Start()
    {
        LoadCharacterPosition();
    }
    private string[] buttonNames = {
        "joystick button 0",
        "joystick button 1",
        "joystick button 2",
        "joystick button 3",
        "joystick button 4",
        "joystick button 5",
        "joystick button 6",
        "joystick button 7",
        "joystick button 8",
        "joystick button 9",
        "joystick button 10",
        "joystick button 11",
        "joystick button 12",
        "joystick button 13",
        "joystick button 14",
        "joystick button 15",
        "joystick button 16",
        "joystick button 17",
        "joystick button 18",
        "joystick button 19"
    };
    void Update()
    {
        foreach (var buttonName in buttonNames)
        {
            if (Input.GetKeyDown(buttonName))
            {
                Debug.Log("Pressed: " + buttonName);
            }
        }
        HandleMovement();
        HandleCutscene();
        UpdateDebugText();
        HandleToggleSpeed();
        HandleSpaceInput();
        HandleInteraction();
    }
    private void HandleToggleSpeed()
    {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Cancel"))
        {
            isRunning = !isRunning;
            isBiking = false; 
        }
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Jump"))
        {
            isBiking = !isBiking;
            isRunning = false; 
        }
    }
    void Interact()
    {
        Debug.Log("Interacting");
        Vector3 facingDir = new Vector3(lastHorizontal, lastVertical);
        Vector3 interactPos = transform.position + facingDir;
        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
            isNPCTalking = true;
            StartCoroutine(WaitForDialogFinished(collider));
        }

    }
    private IEnumerator WaitForDialogFinished(Collider2D collider)
    {
        var npc = collider.GetComponent<Interactable>();
        
        while (npc.IsTalking())
        {
            yield return null;
        }
        
        isNPCTalking = false;
        if (npc.IsTrainer())
        {
            transition = CharacterValueManager.Instance.transition;
            video = CharacterValueManager.Instance.video;
            PokemonParty.Instance.IsWildBattle = false;
            Debug.Log("Encountered a wild trainer");
            lastHorizontal = input.x;
            lastVertical = input.y;
            animator.SetFloat("LastHorizontal", lastHorizontal);
            animator.SetFloat("LastVertical", lastVertical);
            BGmusic.instance.audioSource.Stop();
            BGmusic.instance.audioSource.clip = song;
            BGmusic.instance.audioSource.Play();
            isTransitioning = true;
            animator.SetFloat("Speed", 0f);
            SaveCharacterPosition();
            transition.SetActive(true);
            video.Play();
            video.loopPointReached += CheckOver;
        }
            
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F2) || Input.GetButtonDown("Submit"))
        {
            Interact();
        }
    }
    private void HandleMovement()
    {
        if (!isPaused && !isMoving && !isCutscenePlaying && !isNPCTalking && !PauseMenu.Instance.GameIsPaused && !isTransitioning)
        {

            //Debug.Log("Can move");
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            
            input = ConvertToDigitalInput(input);

            ProcessMovement(input);
        }
    }

    private Vector2 ConvertToDigitalInput(Vector2 input)
    {
        float deadzone = 0.5f; 
        Vector2 digitalInput = Vector2.zero;

        
        if (Mathf.Abs(input.x) > deadzone)
        {
            digitalInput.x = Mathf.Sign(input.x);
        }

        
        if (Mathf.Abs(input.y) > deadzone)
        {
            digitalInput.y = Mathf.Sign(input.y);
        }

        
        if (digitalInput.x != 0) digitalInput.y = 0;

        return digitalInput;
    }

    private void HandleCutscene()
    {
        if (isCutscenePlaying && !isMoving)
        {
            Debug.Log("Playing cutscene");
            if (pathQueue.Count > 0)
            {
                Vector3 nextTarget = pathQueue.Peek(); 
                UpdateAnimatorForCutsceneMovement(nextTarget);
                if (!isMoving) StartCoroutine(Move(nextTarget));
            }
            else
            {
                Debug.Log("Cutscene ended");
                EndCutscene();
            }
        }
    }

    private void UpdateAnimatorForCutsceneMovement(Vector3 nextTarget)
    {
        Vector2 movementDirection = (nextTarget - transform.position).normalized;
        animator.SetFloat("Horizontal", movementDirection.x);
        animator.SetFloat("Vertical", movementDirection.y);
        animator.SetFloat("Speed", movementDirection.magnitude);
        lastHorizontal = movementDirection.x;
        lastVertical = movementDirection.y;
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
            yield return null;
        }
        if (pathQueue.Count > 0) pathQueue.Dequeue(); 
        isMoving = false;
        CheckForEncounters();
        CheckForPortals();
    }


    private void EndCutscene()
    {
        UpdateAnimatorForIdle();
        isCutscenePlaying = false; 
    }

    private void UpdateDebugText()
    {
        if (text != null)
        {
            text.GetComponent<TextMeshProUGUI>().text = $"cutscene: {isCutscenePlaying}, isMoving: {isMoving}";
        }
    }

    private void HandleSpaceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            StartCutsceneToTarget(new Vector2(PosX, PosY));
        }
    }


    private void ProcessMovement(Vector2 input)
    {
        if (input.x != 0) input.y = 0;

        if (input != Vector2.zero)
        {
            
            currentSpeed = isPaused ? 0 : DetermineSpeed();
            Debug.Log($"Current speed: {currentSpeed}");
            UpdateAnimatorForMovement(input);
            var targetPos = transform.position + new Vector3(input.x, input.y, 0);
            if (IsWalkable(targetPos))
            {
                StartCoroutine(Move(targetPos));
            }
        }
        else
        {
            
            UpdateAnimatorForIdle();
        }
    }



    private float DetermineSpeed()
    {
        if (isRunning) return runningSpeed;
        if (isBiking) return bikingSpeed;
        return walkingSpeed;
    }


    private void UpdateAnimatorForMovement(Vector2 input)
    {
        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetFloat("Speed", input.magnitude);
        lastHorizontal = input.x;
        lastVertical = input.y;
    }

    private void UpdateAnimatorForIdle()
    {
        animator.SetFloat("LastHorizontal", lastHorizontal);
        animator.SetFloat("LastVertical", lastVertical);
        animator.SetFloat("Speed", 0f);
    }
    public void StartCutscene(Vector3[] path)
    {
        foreach (var point in path)
        {
            pathQueue.Enqueue(point);
        }

        isCutscenePlaying = true;
    }
    void StartCutsceneToTarget(Vector2 targetPos)
    {
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        Debug.Log("Start: " + startPos + " Target: " + targetPos);
        Pathfinding pathfinding = new Pathfinding();
        pathfinding.InitializeGridFromMap();
        List<Node> path = pathfinding.FindPath(startPos, targetPos);
        if (path != null && path.Count > 0)
        {
            Debug.Log("Path found with length: " + path.Count);
            foreach (var node in path)
            {
                Debug.Log($"Node at: {node.worldPosition}");
            }
        }
        else
        {
            Debug.Log("No path found or path is empty");
        }

        if (path != null)
        {
            Vector3[] vectorPath = path.Select(node => new Vector3(node.worldPosition.x, node.worldPosition.y, 0)).ToArray();
            Debug.Log($"Path: {string.Join(", ", vectorPath)}");
            StartCutscene(vectorPath);
        }
        else
        {
            Debug.Log("No path found");
        }
    }


    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.3f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                PokemonParty.Instance.IsWildBattle = true;
                Debug.Log("Encountered a wild pokemon");
                lastHorizontal = input.x;
                lastVertical = input.y;
                animator.SetFloat("LastHorizontal", lastHorizontal);
                animator.SetFloat("LastVertical", lastVertical);
                BGmusic.instance.audioSource.Stop();
                BGmusic.instance.audioSource.clip = song;
                BGmusic.instance.audioSource.Play();
                isTransitioning = true;
                animator.SetFloat("Speed", 0f);
                SaveCharacterPosition();
                if (transition == null)
                {
                    transition = GameObject.FindWithTag("Transition");
                    Debug.Log(transition == null ? "Transition object not found." : "Transition object found.");
                }

                if (transition != null)
                {
                    transition.SetActive(true);
                }
                else
                {
                    Debug.LogError("Transition object is null when trying to activate.");
                }

                video.Play();
                video.loopPointReached += CheckOver;
            }
        }
    }
    private void CheckForPortals()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, portalLayer);
        if (collider != null)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
            }
        }
    }
    private void SaveCharacterPosition()
    {
        CharacterValueManager.Instance.posX = transform.position.x;
        CharacterValueManager.Instance.posY = transform.position.y;
        CharacterValueManager.Instance.directionHorizontal = lastHorizontal;
        CharacterValueManager.Instance.directionVertical = lastVertical;
    }
    private void LoadCharacterPosition()
    {
        transform.position = new Vector3(CharacterValueManager.Instance.posX, CharacterValueManager.Instance.posY, -0.55f);
        lastHorizontal = CharacterValueManager.Instance.directionHorizontal;
        lastVertical = CharacterValueManager.Instance.directionVertical;
        animator.SetFloat("LastHorizontal", lastHorizontal);
        animator.SetFloat("LastVertical", lastVertical);
    }
    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene("BattleScene");
    }
    private bool IsWalkable(Vector2 targetPosition)
    {
        return Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer | interactableLayer) == null;
    }
}
