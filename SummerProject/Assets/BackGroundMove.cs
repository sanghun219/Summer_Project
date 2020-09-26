using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    private Player player;
    private Vector3 originPos;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
           (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        originPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (player.isGameOver == false)
        {
            transform.position = player.transform.position + new Vector3(originPos.x
                , originPos.y, 1000);
        }
    }
}