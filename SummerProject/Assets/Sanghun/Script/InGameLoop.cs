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

    private Spawner spawner;
    private TestPlayer testPlayer;
    private bool IsRunning = false;

    // Start is called before the first frame update
    private void Awake()
    {
        testPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
        spawner = spawnerPrefab.GetComponent<Spawner>();

        CreateSpawner();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsRunning)
        {
            spawner.UpdateSpawnerPosition(testPlayer.transform.position);
        }
    }

    private void CreateSpawner()
    {
        spawner.Init(testPlayer.transform.position, StartSpawnTime,
            numOfSpawnedObj, distSpawnerToPlayer, elaspedSpawn, numOfSpawnPoint);
        // 회전.. 135도 회전하면 플레이어 쪽을 바라보게 만들어짐
        Quaternion quaternion = Quaternion.Euler(new Vector3(0, 0, 135));
        GameObject Go = Instantiate(spawnerPrefab, spawner.transform.position, quaternion);
        IsRunning = true;
    }
}