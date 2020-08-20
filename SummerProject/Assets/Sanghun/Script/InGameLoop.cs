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
        // TODO : 나중에 진짜 Player로 대체됨
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        StartCoroutine(IUpdate());
    }

    // Update is called once per frame
    private IEnumerator IUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Spawner.GetInstance.UpdateSpawnerPosition(player.transform.position);
        }
    }
}