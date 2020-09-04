using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public static int nextScenenum;

    private bool isMoveNextScene = false;

    public void SetMoveNextScene(bool bo)
    {
        if (PlayerManager.GetInstance.isFirstLogin)
            PlayerManager.GetInstance.SetPlayerID(inputField.text);
        fade.FadeIn(0.5f, () => { isMoveNextScene = bo; });
    }

    private void Start()
    {
        PlayerManager.GetInstance.PlayerManagerAwake();

        if (PlayerManager.GetInstance.isFirstLogin == false)
        {
            inputField.gameObject.SetActive(false);
        }

        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("Scenes/TESTEST");
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;

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
                    yield break;
                }
            }
            else
            {
                if (isMoveNextScene && loadingBar.value >= 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}