using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public Toggle toggle;
    public Toggle vibrateToggle;
    public static bool isVibrate = true;

    public static bool GetIsVibrate()
    {
        return isVibrate;
    }

    public void ChangeVibrate()
    {
        if (vibrateToggle.isOn)
        {
            isVibrate = true;
        }
        else
        {
            isVibrate = false;
        }
    }

    public static void SetVibrate(bool _isvibe)
    {
        isVibrate = _isvibe;
    }

    private void Start()
    {
        toggle.onValueChanged.AddListener((value) => { ToggleListener(value); });
    }

    public void ToggleListener(bool value)
    {
        if (value)
        {
            GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().enabled = true;
            SoundManager.instance.ToggleSound();
        }
        else
        {
            GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().enabled = false;
            SoundManager.instance.ToggleSound();
        }
    }
}