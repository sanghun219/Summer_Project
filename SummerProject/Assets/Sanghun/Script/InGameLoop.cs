using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.AwakePlayer();
    }

    private void ReStart()
    {
        StopCoroutine(IFixedUpdate());
        StopCoroutine(IUpdate());

        Spawner.GetInstance.gameObject.SetActive(true);
        Spawner.GetInstance.Restart();
        player.ReStart();
        StartCoroutine(IFixedUpdate());
        StartCoroutine(IUpdate());
    }

    private void Update()
    {
        // TODO : 나중에 UI쪽에서 RESTART하게 해야함!
        if (Input.GetKey(KeyCode.Space) && player.isGameOver)
        {
            ReStart();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(IFixedUpdate());
        StartCoroutine(IUpdate());
        player.StartPlayer();
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
        Spawner.GetInstance.gameObject.SetActive(false);
        while (true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                break;
            }
            yield return new WaitForFixedUpdate();
            // player를 극적으로 날려보낼수 있음
            player.transform.Rotate(new Vector3(1, 1, 1) * 120 * Time.deltaTime);
            player.transform.position += transform.forward * 30 * Time.deltaTime;
        }
    }
}