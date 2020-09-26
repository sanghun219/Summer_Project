using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Threading;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Slider loadingBar;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Text touchToStart;

    [SerializeField]
    private Fade fade;

    [SerializeField]
    private Text loadText;

    public static int nextScenenum;

    private bool isMoveNextScene = false;

    public void SetMoveNextScene(bool bo)
    {
        if (PlayerManager.GetInstance.isFirstLogin)
            PlayerManager.GetInstance.SetPlayerID(inputField.text);
        fade.FadeIn(0.5f, () => { isMoveNextScene = bo; });
    }

    private void Awake()
    {
        PlayerManager.GetInstance.PlayerManagerAwake();
    }

    private void Start()
    {
        if (PlayerManager.GetInstance.isFirstLogin == false)
        {
            inputField.gameObject.SetActive(false);
        }

        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;

            loadText.text = Math.Truncate(loadingBar.value * 100).ToString() + "%";

            if (loadingBar.value < 0.9f)
            {
                loadingBar.value = Mathf.MoveTowards(loadingBar.value, 0.9f, Time.deltaTime);
            }
            else if (op.progress >= 0.9f)
            {
                loadingBar.value = Mathf.MoveTowards(loadingBar.value, 1f, Time.deltaTime);
            }

            if (loadingBar.value >= 1.0f)
            {
                if (PlayerManager.GetInstance.isFirstLogin)
                {
                    if (inputField.text.Length > 0)
                    {
                        touchToStart.GetComponent<Button>().gameObject.SetActive(true);
                        touchToStart.GetComponent<Button>().enabled = true;
                    }
                }
                else
                {
                    touchToStart.GetComponent<Button>().gameObject.SetActive(true);
                    touchToStart.GetComponent<Button>().enabled = true;
                }
            }

            if (PlayerManager.GetInstance.isFirstLogin)
            {
                if (isMoveNextScene && loadingBar.value >= 1.0f && inputField.text.Length > 0)
                {
                    op.allowSceneActivation = true;
                }
            }
            else
            {
                if (isMoveNextScene && loadingBar.value >= 1.0f)
                {
                    op.allowSceneActivation = true;
                }
            }
        }
    }
}