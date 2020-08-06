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
    private static Spawner Instance = null;

    // Player와 Spawner 사이의 거리
    private float distSpawnerToPlayer;

    // Spawn이 시작되는 시간
    private float startSpawnTime;

    // Spawn 되는 간격
    private float elaspedSpawn;

    // Spawn될 요소를 미리 만들어두는 풀
    private Queue<GameObject> spawningPool;

    // Spawn될 요소의 개수
    private int numOfSpawnedObj;

    // Spawner의 대각선 길이
    private float distSpawner = 300.0f;

    // numOfspawnPoint로 일정 거리마다 스폰되는 지점이 정해짐
    private int numOfspawnPoint;

    // Spawner 대각선의 양 끝점 (여기서부터 일정 간격으로 스폰 됨)
    private Vector2[] EndOfSpawnPoint;

    [SerializeField]
    private List<GameObject> InstOfItems = new List<GameObject>();

    [SerializeField]
    private List<GameObject> InstOfObstacles = new List<GameObject>();

    private float StartSpawn = 0.0f;

    public static Spawner GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = FindObjectOfType(typeof(Spawner)) as Spawner;
                if (Instance == null)
                {
                    Debug.Log("no singleton obj");
                }
            }
            return Instance;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Init(Vector3 startPlayerPos, float startSpawnTime,
        int numOfSpawnedObj, float distSpawnerToPlayer, float elaspedSpawn, int numOfSpawnPoint)
    {
        this.startSpawnTime = startSpawnTime;
        this.numOfSpawnedObj = numOfSpawnedObj;
        this.distSpawnerToPlayer = distSpawnerToPlayer;
        this.elaspedSpawn = elaspedSpawn;
        this.numOfspawnPoint = numOfSpawnPoint;
        EndOfSpawnPoint = new Vector2[2];
        UpdateSpawnerPosition(new Vector3(0, 0, 0));
        InitSpawningPool();
    }

    public void UpdateSpawnerPosition(Vector3 playerPos)
    {
        // 대각선 (player 기준 45도 (1시방향)에 지정한 거리만큼 떨어진 곳에 spawner 생성
        Vector2 center = new Vector2(playerPos.x + distSpawnerToPlayer * Mathf.Cos(45),
            playerPos.y + distSpawnerToPlayer * Mathf.Sin(45));

        transform.position = new Vector3(center.x, center.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, -135);
        // Spawner 대각선의 양 끝점을 지정
        EndOfSpawnPoint[0].x = center.x - (Mathf.Sqrt(2) / 2) * distSpawner / 2;
        EndOfSpawnPoint[0].y = center.y + (Mathf.Sqrt(2) / 2) * distSpawner / 2;
        EndOfSpawnPoint[1].x = center.x + (Mathf.Sqrt(2) / 2) * distSpawner / 2;
        EndOfSpawnPoint[1].y = center.y - (Mathf.Sqrt(2) / 2) * distSpawner / 2;

        startSpawnTime += Time.deltaTime;
        if (startSpawnTime >= elaspedSpawn)
        {
            startSpawnTime = 0.0f;
            SpawnObjects();
        }
    }

    // 매 주기마다 스폰이 됨
    private void SpawnObjects()
    {
        // 한 wave에 생길수 있는 장애물 비율/빈 공간 비율/ 아이템 비율 (장애물 비율은 50%이상)
        int numOfEnemy = (int)(Random.Range((int)numOfspawnPoint / 2, numOfspawnPoint));
        int numOfEmpty = (int)(Random.Range(0, numOfspawnPoint - numOfEnemy));
        int numOfItem = (int)(Random.Range(0, numOfspawnPoint - numOfEnemy - numOfEmpty));
        Debug.Log(numOfEnemy + "," + numOfEmpty + "," + numOfItem);
        List<GameObject> line = new List<GameObject>();
        for (int i = 0; i < numOfEnemy; i++)
        {
            GameObject spawnObj = GetSpawnedObj();
            line.Add(spawnObj);
        }
        for (int i = 0; i < numOfItem; i++)
        {
            GameObject spawnObj = GetSpawnedObj();
            line.Add(spawnObj);
        }
        // 안에 요소들을 다 섞음.
        GetShuffledSpawnedObj(ref line);

        // 안에 요소들 포지션이 정해짐 (내분)
        for (int i = 0; i < line.Count; i++)
        {
            float px = ((i * EndOfSpawnPoint[1].x) + (line.Count - i) * EndOfSpawnPoint[0].x) / (line.Count);
            float py = ((i * EndOfSpawnPoint[1].y) + (line.Count - i) * EndOfSpawnPoint[0].y) / (line.Count);
            float randY = Random.Range(py - 20, py + 20);

            line[i].transform.position = new Vector3(px, randY, 0);
            line[i].SetActive(true);
        }
    }

    private void GetShuffledSpawnedObj(ref List<GameObject> spawnObjs)
    {
        int maxValue = spawnObjs.Count;
        int tmpValue;
        GameObject swapValue;

        for (int i = 0; i < maxValue; i++)
        {
            tmpValue = Random.Range(0, maxValue);
            swapValue = spawnObjs[i];
            spawnObjs[i] = spawnObjs[tmpValue];
            spawnObjs[tmpValue] = swapValue;
        }
    }

    private void InitSpawningPool()
    {
        spawningPool = new Queue<GameObject>(numOfSpawnedObj);

        for (int i = 0; i < numOfSpawnedObj * 2; i++)
        {
            GameObject spawned;
            if (i % 2 == 0)
            {
                spawned = Instantiate(InstOfObstacles[Random.Range(0, InstOfObstacles.Count)]
                    , transform.position, transform.rotation);
            }
            else
            {
                spawned = Instantiate(InstOfItems[Random.Range(0, InstOfItems.Count)]
                    , transform.position, transform.rotation);
            }

            spawned.SetActive(false);
            spawningPool.Enqueue(spawned);
        }
    }

    private GameObject GetSpawnedObj()
    {
        if (spawningPool.Count > 0)
        {
            var spawned = spawningPool.Dequeue();
            spawned.SetActive(false);
            return spawned;
        }
        else
        {
            GameObject spawned = new GameObject();
            if (Random.Range(0, 1) == 0)
            {
                spawned = Instantiate(InstOfObstacles[Random.Range(0, InstOfObstacles.Count)]
                    , transform.position, transform.rotation);
            }
            else
            {
                spawned = Instantiate(InstOfItems[Random.Range(0, InstOfItems.Count)]
                    , transform.position, transform.rotation);
            }

            spawned.SetActive(false);

            return spawned;
        }
    }

    public void ReturnObj(SpawnedObj obj)
    {
        obj.gameObject.SetActive(false);
        spawningPool.Enqueue(obj.gameObject);
    }
}