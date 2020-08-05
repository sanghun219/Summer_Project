using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

internal enum SPAWN_OBJ_TYPE
{
    NONE,
    ITEM,
    OBSTACLE,
    EMPTY,
    END,
}

internal class SpawnObj
{
    public SPAWN_OBJ_TYPE ObjType;
    public GameObject Obj;
}

public class Spawner : MonoBehaviour
{
    // Player와 Spawner 사이의 거리
    private float distSpawnerToPlayer;

    // Spawn이 시작되는 시간
    private float startSpawnTime;

    // Spawn 되는 간격
    private float elaspedSpawn;

    // Spawn될 요소를 미리 만들어두는 풀
    private Queue<SpawnObj> spawningPool;

    // Spawn될 요소의 개수
    private int numOfSpawnedObj;

    // Spawner의 대각선 길이
    private float distSpawner;

    // numOfspawnPoint로 일정 거리마다 스폰되는 지점이 정해짐
    private int numOfspawnPoint;

    // Spawner 대각선의 양 끝점 (여기서부터 일정 간격으로 스폰 됨)
    private Vector2[] EndOfSpawnPoint;

    [SerializeField]
    private GameObject[] InstOfItems;

    [SerializeField]
    private GameObject[] InstOfObstacles;

    public void Init(Vector3 startPlayerPos, float startSpawnTime,
        int numOfSpawnedObj, float distSpawnerToPlayer, float elaspedSpawn, int numOfSpawnPoint)
    {
        this.startSpawnTime = startSpawnTime;
        this.numOfSpawnedObj = numOfSpawnedObj;
        this.distSpawnerToPlayer = distSpawnerToPlayer;
        this.elaspedSpawn = elaspedSpawn;
        this.numOfspawnPoint = numOfSpawnPoint;
        EndOfSpawnPoint = new Vector2[2];
        InitSpawningPool();
    }

    public void UpdateSpawnerPosition(Vector3 playerPos)
    {
        // 대각선 (player 기준 45도 (1시방향)에 지정한 거리만큼 떨어진 곳에 spawner 생성
        Vector2 center = new Vector2(playerPos.x + distSpawnerToPlayer * Mathf.Cos(45),
            playerPos.y + distSpawnerToPlayer * Mathf.Sin(45));

        transform.position = new Vector3(center.x, center.y, 0);

        // Spawner 대각선의 양 끝점을 지정
        EndOfSpawnPoint[0].x = center.x - Mathf.Sqrt(2) * distSpawner / 2;
        EndOfSpawnPoint[0].y = center.y + Mathf.Sqrt(2) * distSpawner / 2;
        EndOfSpawnPoint[1].x = center.x + Mathf.Sqrt(2) * distSpawner / 2;
        EndOfSpawnPoint[1].y = center.y - Mathf.Sqrt(2) * distSpawner / 2;
        Invoke("SpawnObjects", elaspedSpawn);
    }

    // 매 주기마다 스폰이 됨
    private void SpawnObjects()
    {
        // 한 wave에 생길수 있는 장애물 비율/빈 공간 비율/ 아이템 비율 (장애물 비율은 50%이상)
        int numOfEnemy = (int)(Random.Range((int)numOfspawnPoint / 2, numOfspawnPoint));
        int numOfEmpty = (int)(Random.Range(0, numOfspawnPoint - numOfEnemy));
        int numOfItem = (int)(Random.Range(0, numOfspawnPoint - numOfEnemy - numOfEmpty));
        Debug.Log(numOfEnemy + "," + numOfEmpty + "," + numOfItem);
        List<SpawnObj> line = new List<SpawnObj>();
        for (int i = 0; i < numOfEnemy; i++)
        {
            SpawnObj spawnObj = GetSpawnedObj();
            if (spawnObj == null)
            {
                Debug.LogError("Spawn 되는 Object 개수가 총 Object 개수를 못따라감");
                return;
            }
            spawnObj.ObjType = SPAWN_OBJ_TYPE.OBSTACLE;
            line.Add(spawnObj);
        }
        for (int i = 0; i < numOfItem; i++)
        {
            SpawnObj spawnObj = GetSpawnedObj();
            if (spawnObj == null)
            {
                Debug.LogError("Spawn 되는 Object 개수가 총 Object 개수를 못따라감");
                return;
            }
            spawnObj.ObjType = SPAWN_OBJ_TYPE.ITEM;
            line.Add(spawnObj);
        }
        // 안에 요소들을 다 섞음.
        GetShuffledSpawnedObj(ref line);

        // 안에 요소들 포지션이 정해짐 (내분)
        for (int i = 0; i < line.Count; i++)
        {
            float px = (EndOfSpawnPoint[0].x + EndOfSpawnPoint[1].x) * (i + 1) / line.Count;
            float py = (EndOfSpawnPoint[0].y + EndOfSpawnPoint[1].y) * (i + 1) / line.Count;
            float randY = Random.Range(py - 50, py + 50);
            line[i].Obj.transform.position = new Vector3(px, randY, 0);
        }

        // Instantiate , 다시 spawningpool이 수거해감.

        for (int i = 0; i < line.Count; i++)
        {
            GameObject Go = Instantiate(line[i].Obj, line[i].Obj.transform);
            line[i].Obj.SetActive(false);
            line[i].ObjType = SPAWN_OBJ_TYPE.NONE;
            spawningPool.Enqueue(line[i]);
        }
    }

    private void GetShuffledSpawnedObj(ref List<SpawnObj> spawnObjs)
    {
        int maxValue = spawnObjs.Count;
        int tmpValue;
        SpawnObj swapValue = new SpawnObj();

        for (int i = 0; i < maxValue; i++)
        {
            tmpValue = Random.Range(0, maxValue - 1);
            swapValue = spawnObjs[i];
            spawnObjs[i] = spawnObjs[tmpValue];
            spawnObjs[tmpValue] = swapValue;
        }
    }

    private void InitSpawningPool()
    {
        spawningPool = new Queue<SpawnObj>(numOfSpawnedObj);

        for (int i = 0; i < numOfSpawnedObj; i++)
        {
            SpawnObj spawned = new SpawnObj();
            spawned.Obj = new GameObject();
            // 생성만 해두고 Sceen에서 안보이게 만듬

            spawned.Obj.SetActive(false);
            spawned.ObjType = SPAWN_OBJ_TYPE.NONE;

            // 처음엔 Spawner의 중앙에 위치시킴, 회전각도 같이 줘서 플레이어쪽으로 보게 함.
            spawned.Obj.transform.position = transform.position;
            spawned.Obj.transform.rotation = transform.rotation;
            spawningPool.Enqueue(spawned);
        }
    }

    private SpawnObj GetSpawnedObj()
    {
        while (spawningPool.Count > 0)
        {
            var spawned = spawningPool.Dequeue();
            if (spawned.Obj.activeSelf == false)
            {
                spawned.Obj.SetActive(true);
                return spawned;
            }
            else
            {
                spawningPool.Enqueue(spawned);
            }
        }
        return null;
    }
}