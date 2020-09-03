using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager Instance;

    private int score;

    private int resultScore;

    private Player player;

    public Text scoreUI;

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        StartCoroutine(NormalCalculateScore());
        StartCoroutine(ScoreUIUpdate());
    }

    // UI가 Score를 받아서 사용
    public int GetScore() { return score; }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public IEnumerator NormalCalculateScore()
    {
        while (player.isGameOver == false)
        {
            yield return new WaitForSeconds(0.5f);
            // TODO : 그럴듯한 스코어 계산법이 생각나면 사용

            score += (int)player.VelocityZ / 10 + 3;
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
        scoreUI.text = score.ToString();
        StartCoroutine(NormalCalculateScore());
        StartCoroutine(ScoreUIUpdate());
    }
}