using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class WebServer : MonoBehaviour
{
    private bool UseWebServer = true;
    public void SetUserWebServer(bool useServer)
    {
        UseWebServer = useServer;
    }
    private ScoreData[] scoreDatas;
    private void Start()
    {
        InsertScoreInRank("sanghun2191", 101);
    }
    public void Login(string userID, string userPass)
    {
        StartCoroutine(ILogin(userID, userPass));
    }
    public ScoreData[] GetRank()
    {
        StartCoroutine(IGetRank());
        return scoreDatas;
    }
    public void InsertScoreInRank(string userID, int userScore)
    {
        StartCoroutine(InsertScore(userID, userScore));
    }

    IEnumerator ILogin(string userID, string userPass)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", userID);
        form.AddField("loginPass", userPass);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/logininfo/GetLoginInfo.php",form))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
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

    IEnumerator IGetRank()
    {
        WWWForm form = new WWWForm();
        form.AddField("dataType", "LoadRank");
        using(UnityWebRequest www = UnityWebRequest.Post("http://localhost/rankinginfo/GetRankingInfo.php",form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {     
                string jsonArray = www.downloadHandler.text;
                if(jsonArray != "error")
                {
                    scoreDatas = JsonHelper.FromJson<ScoreData>("{\"items\":" + jsonArray + "}");
                }
                       
            }
        }
    } 

    IEnumerator InsertScore(string playerID, int playerScore)
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
