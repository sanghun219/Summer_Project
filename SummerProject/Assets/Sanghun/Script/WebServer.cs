using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class WebServer : MonoBehaviour
{
    private bool UseWebServer = false;

    public bool isInsertedYourData = false;

    public event Action InsertedEventHandler;

    private void InsertedEvent()
    {
        if (InsertedEventHandler != null)
        {
            InsertedEventHandler();
        }
    }

    public void SetUserWebServer(bool useServer)
    {
        UseWebServer = useServer;
    }

    private ScoreData[] scoreDatas;

    private void Start()
    {
        //if (UseWebServer)
        StartCoroutine(IGetRank());
    }

    public void Login(string userID, string userPass)
    {
        StartCoroutine(ILogin(userID, userPass));
    }

    // TODO : 랭킹 보여줄 때 필요한 TOP 10 자료
    public void InitGetRank()
    {
        StartCoroutine(IGetRank());
    }

    public ScoreData[] GetRank()
    {
        return scoreDatas;
    }

    // TODO : 죽었을시에 랭킹을 올려줌 (추후에 필요없다 싶으면 탑10 안에 드는 정보만 테이블로 유지하자!)
    public void InsertScoreInRank(string userID, int userScore)
    {
        StartCoroutine(InsertScore(userID, userScore));
    }

    private IEnumerator ILogin(string userID, string userPass)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", userID);
        form.AddField("loginPass", userPass);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/logininfo/GetLoginInfo.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                byte[] results = www.downloadHandler.data;
            }
        }
    }

    private IEnumerator IGetRank()
    {
        isInsertedYourData = false;
        WWWForm form = new WWWForm();
        form.AddField("dataType", "LoadRank");
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/rankinginfo/GetRankingInfo.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;
                if (jsonArray != "error")
                {
                    scoreDatas = JsonHelper.FromJson<ScoreData>("{\"items\":" + jsonArray + "}");
                    InsertedEvent();
                }
            }
        }
    }

    private IEnumerator InsertScore(string playerID, int playerScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("UserID", playerID);
        form.AddField("UserScore", playerScore);
        form.AddField("dataType", "InsertScore");
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/rankinginfo/GetRankingInfo.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}