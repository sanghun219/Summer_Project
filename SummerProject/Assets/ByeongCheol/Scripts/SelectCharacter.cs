using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectCharacter : MonoBehaviour
{
    public int character = 0;
    private GameObject player;
    public GameObject[] childPlayers;
    private bool changed = false;
    public string testtext;
    private int tempCharacter = 0;

    public event Action ChangeCharacterHandler;

    private static SelectCharacter Instance = null;

    private static GameObject container;

    public static SelectCharacter GetInstance
    {
        get
        {
            if (!Instance)
            {
                container = new GameObject();
                container.name = "SelectManager";
                Instance = container.AddComponent(typeof(SelectCharacter)) as SelectCharacter;
            }
            return Instance;
        }
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        childPlayers = new GameObject[player.transform.childCount];

        for (int i = 0; i < player.transform.childCount; i++)
        {
            childPlayers[i] = player.transform.GetChild(i).gameObject;
            childPlayers[i].SetActive(true);
            childPlayers[i].GetComponent<Player>().AwakePlayer();
            childPlayers[i].GetComponent<Player>().StartPlayer();

            if (i != character)
            {
                childPlayers[i].SetActive(false);
            }
        }
        ChangeCharacter();
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
            int selectedCharacter = tempCharacter;
            character = tempCharacter;
            childPlayers[selectedCharacter].SetActive(true);
            childPlayers[selectedCharacter].GetComponent<Player>().AwakePlayer();
            childPlayers[selectedCharacter].GetComponent<Player>().StartPlayer();
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
        character = selectedCharacter;

        if (GameObject.FindGameObjectWithTag("Player").transform.GetChild(tempCharacter)
            .gameObject.GetComponent<Player>().isGameOver)
        {
            tempCharacter = character;
            changed = true;
        }
        else
        {
            childPlayers[selectedCharacter].SetActive(true);
            childPlayers[selectedCharacter].GetComponent<Player>().AwakePlayer();
            childPlayers[selectedCharacter].GetComponent<Player>().StartPlayer();
            tempCharacter = selectedCharacter;
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