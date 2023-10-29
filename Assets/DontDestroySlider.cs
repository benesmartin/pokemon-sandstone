using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroySlider : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
