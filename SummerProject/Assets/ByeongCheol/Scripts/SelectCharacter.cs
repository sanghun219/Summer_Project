using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{

    public Character character;

    void Start()
    {

    }

    private void OnMouseUpAsButton()
    {
        DataMgr.instance.currentCharacter = character;
    }

}
