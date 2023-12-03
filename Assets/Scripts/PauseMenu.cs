using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Pokédex"));
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        ResumeGame();
    }
    public void PlaySFX(AudioClip clip)
    {
        BGmusic.instance.audioSource.PlayOneShot(clip);
    }
    public void PlaySong(AudioClip song)
    {
        BGmusic.instance.audioSource.Stop();
        BGmusic.instance.audioSource.clip = song;
        BGmusic.instance.audioSource.Play();
    }
}
