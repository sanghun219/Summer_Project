using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLoop : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnerPrefab;

    [SerializeField]
    private float StartSpawnTime;

    [SerializeField]
    private float distSpawnerToPlayer;

    [SerializeField]
    private int numOfSpawnedObj;

    [SerializeField]
    private float elaspedSpawn;

    [SerializeField]
    private int numOfSpawnPoint;

    private TestPlayer testPlayer;
    private bool IsRunning = false;

    // Start is called before the first frame update
    private void Awake()
    {
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
            numOfSpawnedObj, distSpawnerToPlayer, elaspedSpawn, numOfSpawnPoint);
        // 회전.. 135도 회전하면 플레이어 쪽을 바라보게 만들어짐
        GameObject Go = Instantiate(spawnerPrefab, Spawner.GetInstance.transform.position, Spawner.GetInstance.transform.rotation);
    }
}