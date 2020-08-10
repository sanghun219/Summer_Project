using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    private TestPlayer testPlayer;
    private bool IsRunning = false;

    // Start is called before the first frame update
    private void Awake()
    {
        // TODO : 나중에 진짜 Player로 대체됨
        testPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
    }

    // Update is called once per frame
    private void Update()
    {
        Spawner.GetInstance.UpdateSpawnerPosition(testPlayer.transform.position);
    }
}