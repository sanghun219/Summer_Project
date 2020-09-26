using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public GameObject InGameUI;

    private Player player;

    //Player 클래스의 게임오버이벤트를 스태틱으로 생성하여 해당 클래스의 함수로 바로 접근
    //private Player player;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
           (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        //player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        player.GameOverEvent += this.GameOverEvent;
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
    }

    private void OnDisable()
    {
        SelectCharacter.GetInstance.ChangeCharacterHandler -= ChangePlayer;
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
           (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        //player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        player.GameOverEvent += this.GameOverEvent;
    }

    private void RankingEvent()
    {
        if (this.InGameUI.activeSelf == true)
        {
            //this.InGameUI.SetActive(false);
            if (GameObject.Find("UI").transform.Find("RankingUI"))
            {
                GameObject.Find("UI").transform.Find("RankingUI").gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("게임오버 UI 찾기 실패");
            }
        }
    }

    private void GameOverEvent()
    {
        if (this.InGameUI.activeSelf == true)
        {
            //this.InGameUI.SetActive(false);
            if (GameObject.Find("UI").transform.Find("GameOverUI"))
            {
                GameObject.Find("UI").transform.Find("GameOverUI").gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("게임오버 UI 찾기 실패");
            }
        }
    }
}