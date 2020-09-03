using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public GameObject InGameUI;

    //Player 클래스의 게임오버이벤트를 스태틱으로 생성하여 해당 클래스의 함수로 바로 접근
    //private Player player;

    private void OnEnable()
    {
        Debug.Log("인에이블");
        Player.GameOverEvent += this.GameOverEvent;
    }

    private void OnDisable()
    {
        Debug.Log("디스에이블");
        Player.GameOverEvent -= this.GameOverEvent;
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
        Debug.Log("게임오버 함수 실행");
        if (this.InGameUI.activeSelf == true)
        {
            //this.InGameUI.SetActive(false);
            if (GameObject.Find("UI").transform.Find("GameOverUI"))
            {
                Debug.Log("게임오버 UI 찾기 성공");
                GameObject.Find("UI").transform.Find("GameOverUI").gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("게임오버 UI 찾기 실패");
            }
        }
    }
}