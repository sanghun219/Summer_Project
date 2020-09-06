using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeID : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Button btn;

    [SerializeField]
    private GameObject mainUI;

    private void OnEnable()
    {
        btn.interactable = false;
        StartCoroutine(EnableClick());
    }

    private void OnDisable()
    {
        inputField.text = "";
    }

    private IEnumerator EnableClick()
    {
        while (true)
        {
            yield return null;
            if (inputField.text.Length > 0)
            {
                btn.interactable = true;
            }
            else
            {
                btn.interactable = false;
            }
        }
    }

    public void Click()
    {
        if (inputField.text.Length > 0)
        {
            PlayerManager.GetInstance.SetPlayerID(inputField.text);
            gameObject.SetActive(false);
            mainUI.gameObject.SetActive(true);
        }
    }
}