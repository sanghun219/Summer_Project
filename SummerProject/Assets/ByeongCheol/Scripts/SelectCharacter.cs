using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectCharacter : MonoBehaviour
{
    public int character;
    private GameObject player;
    private GameObject[] childPlayers;
    private bool changed = false;

    public event Action ChangeCharacterHandler;

    private static SelectCharacter Instance = null;

    public static SelectCharacter GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = GameObject.Find("GameManager").GetComponent<SelectCharacter>();
                if (Instance == null)
                {
                    Debug.Log("no singleton obj");
                }
            }
            return Instance;
        }
    }

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else if (Instance != null)
        //{
        //    Destroy(gameObject);
        //}
        //DontDestroyOnLoad(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
        childPlayers = new GameObject[player.transform.childCount];

        for (int i = 0; i < player.transform.childCount; i++)
        {
            childPlayers[i] = player.transform.GetChild(i).gameObject;

            childPlayers[i].GetComponent<Player>().AwakePlayer();
            childPlayers[i].GetComponent<Player>().StartPlayer();
            if (i != 0)
                childPlayers[i].SetActive(false);
        }
    }

    public void ChangeCharacter()
    {
        if (ChangeCharacterHandler != null)
        {
            ChangeCharacterHandler();
        }
    }

    public void Restart()
    {
        if (changed)
        {
            int selectedCharacter = character;
            childPlayers[selectedCharacter].SetActive(true);

            for (int i = 0; i < childPlayers.Length; i++)
            {
                if (i != selectedCharacter)
                {
                    childPlayers[i].SetActive(false);
                }
            }
            changed = false;
            ChangeCharacter();
        }
    }

    public void OnMouseUpAsButton(int selectedCharacter)
    {
        //DataMgr.instance.currentCharacter = (Character)selectedCharacter;

        if (GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>().isGameOver)
        {
            character = selectedCharacter;
            changed = true;
        }
        else
        {
            childPlayers[selectedCharacter].SetActive(true);

            for (int i = 0; i < childPlayers.Length; i++)
            {
                if (i != selectedCharacter)
                {
                    childPlayers[i].SetActive(false);
                }
            }
            ChangeCharacter();
        }
    }
}