using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    private TestPlayer testPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        // TODO : 나중에 진짜 Player로 대체됨
        testPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();

        StartCoroutine(IUpdate());
    }

    // Update is called once per frame
    private IEnumerator IUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Spawner.GetInstance.UpdateSpawnerPosition(testPlayer.transform.position);
        }
    }
}