using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float tileSize = 1f;
    public float walkingSpeed = 4f;
    public float runningSpeed = 8f;
    public float bikingSpeed = 10f;
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 targetPosition;
    bool isMoving = false;
    float currentSpeed = 0f;
    float lastHorizontal = 0f;
    float lastVertical = -1f;

    void Update()
    {
        if (!isMoving && !PauseMenu.GameIsPaused)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

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

            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                vertical = 0f;
            }
            else
            {
                horizontal = 0f;
            }

            if (horizontal != 0 || vertical != 0)
            {
                targetPosition = rb.position + new Vector2(horizontal, vertical) * tileSize;
                animator.SetFloat("Horizontal", horizontal);
                animator.SetFloat("Vertical", vertical);
                animator.SetFloat("Speed", new Vector2(horizontal, vertical).magnitude);

                lastHorizontal = horizontal;
                lastVertical = vertical;

                isMoving = true;
            }
            else
            {
                animator.SetFloat("LastHorizontal", lastHorizontal);
                animator.SetFloat("LastVertical", lastVertical);
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !PauseMenu.GameIsPaused)
        {
            float step = currentSpeed * Time.fixedDeltaTime;
            rb.position = Vector2.MoveTowards(rb.position, targetPosition, step);

            if (Vector2.Distance(rb.position, targetPosition) < float.Epsilon)
            {
                rb.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
