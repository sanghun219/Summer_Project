using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void SelectCharacterBtn(int i)
    {
        SelectCharacter.GetInstance.OnMouseUpAsButton(i);
    }
}