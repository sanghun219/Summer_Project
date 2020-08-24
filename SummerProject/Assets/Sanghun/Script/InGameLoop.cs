using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        StartCoroutine(IFixedUpdate());
        StartCoroutine(IUpdate());
    }

    private IEnumerator IUpdate()
    {
        while (player.isGameOver == false)
        {
            yield return null;
            player.PlayerUpdate();
        }
    }

    // Update is called once per frame
    private IEnumerator IFixedUpdate()
    {
        while (player.isGameOver == false)
        {
            yield return new WaitForFixedUpdate();
            player.PlayerFixedUpdate();
            Spawner.GetInstance.UpdateSpawnerPosition(player.transform.position);
        }

        while (true)
        {
            yield return new WaitForFixedUpdate();
            // player를 극적으로 날려보낼수 있음
            player.transform.Rotate(new Vector3(1, 1, 1) * 120 * Time.deltaTime);
            player.transform.position += transform.forward * 30 * Time.deltaTime;
        }
    }
}