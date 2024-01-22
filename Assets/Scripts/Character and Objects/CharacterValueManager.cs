using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CharacterValueManager : MonoBehaviour
{
    public static CharacterValueManager Instance { get; private set; }

    public float posX = 1.5f;
    public float posY = 1f;
    public float directionHorizontal = 0;
    public float directionVertical = 0;
    public GameObject transition;
    public VideoPlayer video;
    public bool SpokeToProfessor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
