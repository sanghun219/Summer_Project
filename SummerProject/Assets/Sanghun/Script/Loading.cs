using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    private string playerID;

    public string GetPlayerID()
    { return playerID; }

    private void SetPlayerID(string id)
    { this.playerID = id; }

    private static Loading Instance = null;

    public static Loading GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = FindObjectOfType(typeof(Loading)) as Loading;
                if (Instance == null)
                {
                    return null;
                }
            }
            return Instance;
        }
    }

    private void Start()
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