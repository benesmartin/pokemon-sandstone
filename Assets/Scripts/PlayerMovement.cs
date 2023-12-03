using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    bool isMoving;
    float currentSpeed = 0f;
    float lastHorizontal = 0f;
    float lastVertical = -1f;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public bool isTransitioning = false;
    public AudioClip song;
    private void Start()
    {
        LoadCharacterPosition();
    }
    void Update()
    {
        if (!isMoving && !PauseMenu.GameIsPaused && !isTransitioning)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(KeyCode.B))
            {
                currentSpeed = runningSpeed;
                //todo
                //animator.SetBool("Running", true);
            }
            else if (Input.GetKey(KeyCode.C))
            {
                currentSpeed = bikingSpeed;
                //todo
                //animator.SetBool("Biking", true);
            }
            else
            {
                currentSpeed = walkingSpeed;
                //todo
                //animator.SetBool("Running", false);
                //animator.SetBool("Biking", false);
            }
            if (input.x != 0) input.y = 0;
            if (input != Vector2.zero)
            { 
                var targetpos = transform.position;
                targetpos.x += input.x;
                targetpos.y += input.y;
                if (IsWalkable(targetpos)) StartCoroutine(Move(targetpos));
                animator.SetFloat("Horizontal", input.x);
                animator.SetFloat("Vertical", input.y);
                animator.SetFloat("Speed", new Vector2(input.x, input.y).magnitude);
                lastHorizontal = input.x;
                lastVertical = input.y;
            }
            else
            {
                animator.SetFloat("LastHorizontal", lastHorizontal);
                animator.SetFloat("LastVertical", lastVertical);
                animator.SetFloat("Speed", 0f);
            }   
        }
    }
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }
    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.3f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                
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
                transition.SetActive(true);
                video.Play();
                video.loopPointReached += CheckOver;
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
        transform.position = new Vector3(CharacterValueManager.Instance.posX, CharacterValueManager.Instance.posY, 1);
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
        return Physics2D.OverlapCircle(targetPosition, 0.3f, solidObjectsLayer) == null;
    }
}
