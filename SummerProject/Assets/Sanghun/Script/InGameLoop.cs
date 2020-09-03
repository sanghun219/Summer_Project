using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private Fade fade;

    // 단 한번만 게임 시작할 때 true가 됨
    public static bool isGameStart = false;

    public void SetGameStart(bool isStart)
    {
        isGameStart = isStart;
    }

    private void Awake()
    {
        fade.FadeOut(0.5f);
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
            if (isGameStart)
                player.PlayerUpdate();
        }
    }

    // Update is called once per frame
    private IEnumerator IFixedUpdate()
    {
        while (player.isGameOver == false)
        {
            yield return new WaitForFixedUpdate();
            if (isGameStart)
            {
                player.PlayerFixedUpdate();
                Spawner.GetInstance.UpdateSpawnerPosition(player.transform.position);
            }
        }
        Spawner.GetInstance.WaitRestart = true;
        isGameStart = false;
        while (player.isGameOver)
        {
            yield return new WaitForFixedUpdate();
            // player를 극적으로 날려보낼수 있음
            player.transform.Rotate(new Vector3(1, 1, 1) * 120 * Time.deltaTime);
            player.transform.position += transform.forward * 30 * Time.deltaTime;
        }
    }
}