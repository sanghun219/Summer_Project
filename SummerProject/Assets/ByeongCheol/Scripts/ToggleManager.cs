using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public Toggle toggle;
    void Start()
    {

        toggle.onValueChanged.AddListener((value) =>
     {
         MyListener(value);
     });
        //Do this in Start() for example 
    }

    public void MyListener(bool value)
    {
        if (value)
        {
            GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().enabled = true;
            //do the stuff when the toggle is on 
        }
        else
        {
            GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>().enabled = false;
            //do the stuff when the toggle is off 
        }

    }

    public void OnClick()
    {
        Debug.Log("Clicked");
    }
}
