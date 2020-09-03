﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIManager : MonoBehaviour
{
    public GameObject rbtnPrefab;

    public RectTransform contents;

    public Sprite[] RankgImages;

    public Text MyScore;

    [SerializeField]
    private WebServer webserver;

    private void OnEnable()
    {
        if (webserver == null)
        {
            webserver = GameObject.Find("GameManager").GetComponent<WebServer>();
        }
        if (contents.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < contents.gameObject.transform.childCount; i++)
            {
                Destroy(contents.GetChild(i).gameObject);
            }
        }
        // webserver.InsertScoreInRank(Loading.GetInstance.GetPlayerID(), ScoreManager.GetInstance.GetScore());
        webserver.InsertScoreInRank("sanghun123", ScoreManager.GetInstance.GetScore());
        webserver.InitGetRank();
    }

    private void Start()
    {
        webserver.InsertedEventHandler += ViewRankContent;
    }

    private void ViewRankContent()
    {
        int rankNum = 1;
        MyScore.text += ScoreManager.GetInstance.GetScore();
        foreach (var rank in webserver.GetRank())
        {
            this.CreateRankContent(rank.ID, rank.Date, rank.Score, rankNum);
            Debug.Log(rank.ID);
            rankNum++;
        }
    }

    public void CreateRankContent(string id, string date, int score, int rank)
    {
        if (rank <= 3)
        {
            GameObject tempPrefab = Instantiate(this.rbtnPrefab);

            Image rankImage = tempPrefab.transform.Find("RankImage").GetComponent<Image>();
            Text rankDateData = tempPrefab.transform.Find("DateData").GetComponent<Text>();
            Text rankID = tempPrefab.transform.Find("NameData").GetComponent<Text>();
            Text rankScore = tempPrefab.transform.Find("ScoreData").GetComponent<Text>();
            Text rankTextData = tempPrefab.transform.Find("RankTextData").GetComponent<Text>();

            rankDateData.text = date;
            rankID.text = id.ToString();
            rankScore.text = score.ToString();
            rankTextData.text = rank.ToString();
            rankImage.sprite = RankgImages[rank - 1];
            tempPrefab.transform.SetParent(contents);
        }
        else
        {
            GameObject tempPrefab = Instantiate(this.rbtnPrefab);

            Image rankImage = tempPrefab.transform.Find("RankImage").GetComponent<Image>();
            Text rankDateData = tempPrefab.transform.Find("DateData").GetComponent<Text>();
            Text rankID = tempPrefab.transform.Find("NameData").GetComponent<Text>();
            Text rankScore = tempPrefab.transform.Find("ScoreData").GetComponent<Text>();
            Text rankTextData = tempPrefab.transform.Find("RankTextData").GetComponent<Text>();

            rankDateData.text = date;
            rankID.text = id.ToString();
            rankScore.text = score.ToString();
            rankTextData.text = rank.ToString();
            rankImage.gameObject.SetActive(false);
            tempPrefab.transform.SetParent(contents);
        }
    }
}