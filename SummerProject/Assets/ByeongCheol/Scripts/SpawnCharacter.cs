using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{

    public GameObject[] charPrefebs;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(charPrefebs[(int)DataMgr.instance.currentCharacter]);
        player.transform.position = transform.position;
    }

}
