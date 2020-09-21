using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public Toggle toggle;
    void Start()
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
