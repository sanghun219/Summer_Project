using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    public float StartSpawnTime;

    public float distSpawnerToPlayer;

    public int numOfSpawnedObj;

    public float elaspedSpawn;

    public int numOfSpawnPoint;

    public float lengOfSpawner;

    private TestPlayer testPlayer;
    private bool IsRunning = false;

    // Start is called before the first frame update
    private void Awake()
    {
        // TODO : 나중에 진짜 Player로 대체됨
        testPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
        CreateSpawner();
        IsRunning = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsRunning)
            Spawner.GetInstance.UpdateSpawnerPosition(testPlayer.transform.position);
    }

    private void CreateSpawner()
    {
        Spawner.GetInstance.Init(testPlayer.transform.position, StartSpawnTime,
            numOfSpawnedObj, distSpawnerToPlayer, elaspedSpawn, numOfSpawnPoint, lengOfSpawner);
    }
}