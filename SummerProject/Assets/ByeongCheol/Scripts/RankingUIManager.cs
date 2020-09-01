using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIManager : MonoBehaviour
{
    public GameObject rbtnPrefab;

    public RectTransform contents;

    public Image rankImg;
    public Text rankTextData;
    public Text nameData;
    public Text scoreData;

    string[] rankImageName = new string[]
    {
        "Rank1",
        "Rank2",
        "Rank3"
    };

    void Start()
    {
            Debug.Log("sdfgsdfgsdfg");
        //rbtnPrefab = Resources.Load<GameObject>("RankPref");
        if(rbtnPrefab == null) { Debug.Log("랭크 프리팹을 로드하지 못했습니다."); }

        for (int i = 0; i < 5; i++)
        {
            this.CreateRankContent();
        }
    }

    public void CreateRankContent()
    {
        Init("asdf", 5, "asdfasdf", 123123);
        GameObject rankContent = Instantiate(this.rbtnPrefab);
        rankContent.transform.SetParent(contents);
    }

    public void Init(string rankImgName, int rank, string name, int score)
    {
        if (rank <= 3)
        {
            this.transform.Find("RankImage").gameObject.SetActive(true);
            this.transform.Find("RankTextData").gameObject.SetActive(false);
            this.rankImg.sprite = Resources.Load<Sprite>("Rank" + rank.ToString());
        }
        else
        {
            rbtnPrefab.transform.Find("RankImage").gameObject.SetActive(false);
            rbtnPrefab.transform.Find("RankText").gameObject.SetActive(true);
            //this.rankTextData.text = rank.ToString();
        }
        //this.nameData.text = name;
        //this.scoreData.text = score.ToString();

    }
}
