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

    public static PlayerManager GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
                if (Instance == null)
                {
                    GameObject.Find("PlayerManager").AddComponent<PlayerManager>();
                    Instance = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
                }
            }
            return Instance;
        }
    }

    public void PlayerManagerAwake()
    {
        playerid = PlayerPrefs.GetString("Login");
        if (playerid == null)
        {
            isFirstLogin = true;
        }
        else
        {
            isFirstLogin = false;
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