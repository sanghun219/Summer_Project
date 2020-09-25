using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIManager : MonoBehaviour
{
    public GameObject rbtnPrefab;

    public RectTransform contents;

    public Sprite[] RankgImages;

    private Player player;

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
        webserver.InitGetRank();
    }

    private void Awake()
    {
        webserver.InsertedEventHandler += ViewRankContent;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        player.GameOverEvent += InsertData;
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        player.GameOverEvent += InsertData;
    }

    private void InsertData()
    {
        if (contents.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < contents.gameObject.transform.childCount; i++)
            {
                Destroy(contents.GetChild(i).gameObject);
            }
        }
        webserver.InitGetRank();
        webserver.InsertScoreInRank(PlayerManager.GetInstance.GetPlayerID(), ScoreManager.GetInstance.GetScore());
    }

    private void ViewRankContent()
    {
        int rankNum = 1;
        foreach (var rank in webserver.GetRank())
        {
            this.CreateRankContent(rank.ID, rank.Date, rank.Score, rankNum);
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
            // Text rankTextData = tempPrefab.transform.Find("RankTextData").GetComponent<Text>();

            rankDateData.text = date;
            rankID.text = id.ToString();
            rankScore.text = score.ToString();
            // rankTextData.text = rank.ToString();
            rankImage.sprite = RankgImages[rank - 1];
            rankImage.rectTransform.anchoredPosition = new Vector2(rankImage.rectTransform.anchoredPosition.x + 150,
                rankImage.rectTransform.anchoredPosition.y);
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