using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    // Score 관련 다 띄워보자
    private static ScoreManager Instance;

    private int score;

    private int resultScore;

    private Player player;

    public Text scoreUI;

    public Text speedUI;

    public Text curItemUI;

    public bool isGameOver;

    public static ScoreManager GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = GameObject.Find("GameManager").GetComponent<ScoreManager>();
                if (Instance == null)
                {
                    GameObject.Find("GameManager").AddComponent<ScoreManager>();
                    Instance = GameObject.Find("GameManager").GetComponent<ScoreManager>();
                }
            }
            return Instance;
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
        StartCoroutine(NormalCalculateScore());
        StartCoroutine(ScoreUIUpdate());
        StartCoroutine(UpdateSpeedUI());
        StartCoroutine(UpdateCurItem());
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        StopAllCoroutines();
        StartCoroutine(NormalCalculateScore());
        StartCoroutine(ScoreUIUpdate());
        StartCoroutine(UpdateSpeedUI());
        StartCoroutine(UpdateCurItem());
    }

    // UI가 Score를 받아서 사용
    public int GetScore() { return score; }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public IEnumerator UpdateCurItem()
    {
        while (player.isGameOver == false)
        {
            yield return null;
            switch (player.GetPlayerMode())
            {
                case PlayerMode.DOUBLE_POINT:
                    curItemUI.text = "Double Point !";
                    break;

                case PlayerMode.DOWN_SPEED:
                    curItemUI.text = "Down Speed !";
                    break;

                case PlayerMode.MAGNET:
                    curItemUI.text = "Magnet !";
                    break;

                case PlayerMode.PUSH:
                    curItemUI.text = "Push Obstacle !";
                    break;

                case PlayerMode.SUPER:
                    curItemUI.text = "Super Mode !";
                    break;

                default:
                    curItemUI.text = "";
                    break;
            }
        }
    }

    public IEnumerator UpdateSpeedUI()
    {
        while (player.isGameOver == false)
        {
            yield return null;
            if (InGameLoop.isGameStart == true)
            {
                int speed = (int)player.VelocityZ;
                if (speed <= 0) speed = 0;
                speedUI.text = speed.ToString();
            }
        }
    }

    public IEnumerator NormalCalculateScore()
    {
        while (player.isGameOver == false)
        {
            yield return new WaitForSeconds(0.2f);
            // TODO : 그럴듯한 스코어 계산법이 생각나면 사용
            if ((player.GetPlayerMode() & PlayerMode.SUPER) == 0)
                score += (int)player.VelocityZ / 2 + 3;
            else
                score += (int)player.VelocityZ / 10;
            resultScore = score;
        }
    }

    // UI상 스코어 업데이트
    public IEnumerator ScoreUIUpdate()
    {
        while (player.isGameOver == false)
        {
            yield return new WaitForSeconds(0);
            if (InGameLoop.isGameStart == true)
                scoreUI.text = GetScore().ToString();
        }
    }

    public void InitializeScore()
    {
        score = 0;
        speedUI.text = "";
        scoreUI.text = score.ToString();
        StartCoroutine(NormalCalculateScore());
        StartCoroutine(ScoreUIUpdate());
        StartCoroutine(UpdateSpeedUI());
        StartCoroutine(UpdateCurItem());
    }
}