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
        Debug.Log(playerid);
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
        //PlayerPrefs.DeleteAll();
        playerid = PlayerPrefs.GetString("Login");
        if (playerid == null || playerid.Length <= 0)
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