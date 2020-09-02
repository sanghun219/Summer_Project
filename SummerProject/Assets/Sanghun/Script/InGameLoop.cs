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

    public void ReStart()
    {
        if (player.isGameOver == false) return;
        StopCoroutine(IFixedUpdate());
        StopCoroutine(IUpdate());

        player.ReStart();
        Spawner.GetInstance.Restart();
        Spawner.GetInstance.WaitRestart = false;
        StartCoroutine(IFixedUpdate());
        StartCoroutine(IUpdate());

        ScoreManager.GetInstance.InitializeScore();
    }

    private void Update()
    {
        // TODO : 나중에 UI쪽에서 버튼으로 RESTART하게 해야함!
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
        Spawner.GetInstance.WaitRestart = true;
        while (player.isGameOver)
        {
            //TODO : 다시 시작하는 버튼 누를시 조건을 넣어줘야함!
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