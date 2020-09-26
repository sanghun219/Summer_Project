using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperModeParticle : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
             (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
            (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position + new Vector3(0, 0, 50);
    }
}