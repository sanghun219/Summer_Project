using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        fade.FadeOut(1.0f);

        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
           (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(SelectCharacter.GetInstance.character)
            .gameObject.GetComponent<Player>();
        player.ReStart();
    }

    public void ReStart()
    {
        if (player.isGameOver == false) return;

        SelectCharacter.GetInstance.Restart();
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
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
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