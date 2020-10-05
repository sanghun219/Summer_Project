using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager Instance;

    private static string playerid;

    public void SetPlayerID(string id)
    {
        playerid = id;
        PlayerPrefs.SetString("Login", playerid);
    }

    public string GetPlayerID()
    {
        return playerid;
    }

    public bool isFirstLogin { get; set; }

    private static GameObject container;

    public static PlayerManager GetInstance
    {
        get
        {
            if (!Instance)
            {
                container = new GameObject();
                container.name = "PlayerManager";
                Instance = container.AddComponent(typeof(PlayerManager)) as PlayerManager;
            }

            return Instance;
        }
    }

    public void PlayerManagerAwake()
    {
        PlayerPrefs.DeleteAll();
        playerid = PlayerPrefs.GetString("Login");
        if (PlayerPrefs.HasKey("Login"))
        {
            isFirstLogin = false;
        }
        else
        {
            isFirstLogin = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}