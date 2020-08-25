using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public GameObject InGameUI;

    private Player player;

    private void OnEnable()
    {
        Debug.Log("인에이블");
        player.GameOverEvent += this.GameOverEvent;
    }

    private void OnDisable()
    {
        Debug.Log("디스에이블");
        player.GameOverEvent -= this.GameOverEvent;
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