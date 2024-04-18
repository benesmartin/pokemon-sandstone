using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private string location = "Petalwind Town";
    private string pokedex = "0";
    private string time = "0:00";
    private string player = "Red";
    private string badges = "0";
    private string scene = "Game";
    public string Location { get => location; set => location = value; }
    public string Pokedex { get => pokedex; set => pokedex = value; }
    public string Time { get => time; set => time = value; }
    public string Player { get => player; set => player = value; }
    public string Badges { get => badges; set => badges = value; }
    public string Scene { get => scene; set => scene = value; }
    public void UpdateStats()
    {
        Location = "Petalwind Town";
        Pokedex = PokedexManager.Instance.GetPokedexCount().ToString();
        Time = "0:00";
        Player = "Player";
        Badges = GetGymBadges();
        Scene = SceneManager.GetActiveScene().name;
    }
    private Dictionary<string, bool> gyms = new Dictionary<string, bool>
    {
        { "Terran", false },
        { "Emberlyn", false },
        { "Shade", false },
        { "Floralie", false },
        { "Marina", false },
        { "Lumina", false },
        { "Ferris", false },
        { "Draken", false }
    };
    public void CheckGymBadge(string trainerName)
    {
        gyms[trainerName] = true;
    }
    public string GetGymBadges()
    {
        int count = 0;
        foreach (var gym in gyms)
        {
            if (gym.Value)
            {
                count++;
            }
        }
        return count.ToString();
    }
}
