using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public enum SPAWN_OBJ
{
    ITEM,
    OBSTACLE,
    END,
}

public class Spawner : MonoBehaviour
{
    // Player와 Spawner 사이의 거리

    public float distSpawnerToPlayer;

    // Spawn이 시작되는 시간

    public float startSpawnTime;

    // Spawn 되는 시간 간격

    public float elaspedSpawn;

    // Spawn될 요소를 미리 만들어두는 풀
    private Queue<GameObject>[] spawningPool = new Queue<GameObject>[2];

    // spawningPool의 크기 (큐가 가변 배열같은거니 딱히 필요 없을지도..)

    public int numOfSpawnedObj;

    // Spawner의 대각선 길이

    public float lengOfSpawner = 300.0f;

    [Range(0, 1)]
    public float ratioOfObstacle;

    // Spawner 대각선의 양 끝점 (여기서부터 일정 간격으로 스폰 됨)
    private Vector3[] EndOfSpawnPoint;

    // 아이템 종류를 리스트로 받음
    [SerializeField]
    private List<Item> InstOfItems = new List<Item>();

    // 장애물 종류를 리스트로 받음
    [SerializeField]
    private List<Obstacle> InstOfObstacles = new List<Obstacle>();

    // Spawn 되는 간격에 필요한 변수 (왜 static이 안되는지 몰겠음)
    private float deltaSpawnTime = 0.0f;

    // SINGLETON PATTERN
    private static Spawner Instance = null;

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

    private void Start()
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
        InitSpawningPool();
        EndOfSpawnPoint = new Vector3[2];

        // 스폰되는 지점보다는 한 번에 스폰되는 오브젝트 개수가 많아야 함
    }

    public void ClearSpawner()
    {
        // spawn된 놈들 전부 제거하고 회수
    }

    public void UpdateSpawnerPosition(Vector3 playerPos)
    {
        deltaSpawnTime += Time.deltaTime;
        if (deltaSpawnTime >= elaspedSpawn)
        {
            deltaSpawnTime = 0.0f;
            SpawnObjects();
        }
        Vector3 center = new Vector3(playerPos.x, 0, playerPos.z + distSpawnerToPlayer);
        transform.position = new Vector3(center.x, center.y, center.z);

        // Spawner 대각선의 양 끝점을 지정
        EndOfSpawnPoint[0].x = center.x - lengOfSpawner / 2;
        EndOfSpawnPoint[0].z = center.z;
        EndOfSpawnPoint[1].x = center.x + lengOfSpawner / 2;
        EndOfSpawnPoint[1].z = center.z;
    }

    // 매 주기마다 스폰이 됨
    private void SpawnObjects()
    {
        // 한 wave에 생길수 있는 장애물 비율/빈 공간 비율/ 아이템 비율 (장애물 비율은 50%이상)
        int numOfObstacle = (int)((numOfSpawnedObj + 1) * ratioOfObstacle);
        int numOfEmpty = Random.Range(0, numOfObstacle);
        int numOfItem = numOfSpawnedObj - numOfEmpty - numOfObstacle;
        Queue<GameObject> wave = new Queue<GameObject>();
        for (int i = 0; i < numOfItem; i++)
        {
            wave.Enqueue(GetSpawnedObj(SPAWN_OBJ.ITEM));
        }
        for (int i = 0; i < numOfObstacle; i++)
        {
            wave.Enqueue(GetSpawnedObj(SPAWN_OBJ.OBSTACLE));
        }

        for (int i = 0; i < numOfEmpty; i++)
        {
            wave.Enqueue(null);
        }

        GetShuffledSpawnedObj(ref wave);

        //안에 요소들 포지션이 정해짐 (내분)
        for (int i = 0; i < wave.Count; i++)
        {
            float px = ((i * EndOfSpawnPoint[1].x) + (wave.Count - i) * EndOfSpawnPoint[0].x) / (wave.Count);
            float pz = ((i * EndOfSpawnPoint[1].z) + (wave.Count - i) * EndOfSpawnPoint[0].z) / (wave.Count);
            float randZ = UnityEngine.Random.Range(pz + 250, pz + 100);
            float randX = UnityEngine.Random.Range(px - 100, px + 100);
            var obj = wave.Dequeue();
            // 빈공간
            if (obj == null) continue;
            if (!obj.activeSelf)
            {
                obj.transform.position = new Vector3(randX, 0, randZ);
                obj.SetActive(true);
            }
        }
        //wave.Clear();
    }

    private void GetShuffledSpawnedObj(ref Queue<GameObject> spawnObjs)
    {
        int maxValue = spawnObjs.Count;
        int tmpValue;
        GameObject swapValue = null;

        for (int i = 0; i < maxValue; i++)
        {
            tmpValue = UnityEngine.Random.Range(0, maxValue);
            swapValue = spawnObjs.ToArray()[i];
            spawnObjs.ToArray()[i] = spawnObjs.ToArray()[tmpValue];
            spawnObjs.ToArray()[tmpValue] = swapValue;
        }
    }

    private void InitSpawningPool()
    {
        for (int i = 0; i < 2; i++)
        {
            spawningPool[i] = new Queue<GameObject>(numOfSpawnedObj);
        }

        for (int i = 0; i < numOfSpawnedObj * 2; i++)
        {
            if (i % 2 == 0)
            {
                // 장애물 비율 적당히 조절해서 instantiate
                GameObject spawned = Instantiate(InstOfObstacles[UnityEngine.Random.Range(0, InstOfObstacles.Count)].gameObject
                    , transform.position, transform.rotation);

                spawned.SetActive(false);
                spawningPool[(int)SPAWN_OBJ.OBSTACLE].Enqueue(spawned);
            }
            else
            {
                GameObject spawned = Instantiate(InstOfItems[UnityEngine.Random.Range(0, InstOfItems.Count)].gameObject
                    , transform.position, transform.rotation);
                spawned.SetActive(false);
                spawningPool[(int)SPAWN_OBJ.ITEM].Enqueue(spawned);
            }
        }
    }

    private GameObject GetSpawnedObj(SPAWN_OBJ spawnedObj)
    {
        if (spawnedObj == SPAWN_OBJ.ITEM)
        {
            if (spawningPool[(int)SPAWN_OBJ.ITEM].Count > 0)
            {
                Item spawned = spawningPool[(int)SPAWN_OBJ.ITEM].Dequeue().GetComponent<Item>();
                spawned.gameObject.SetActive(false);
                return spawned.gameObject;
            }
            else
            {
                Item spawned = Instantiate(InstOfItems[UnityEngine.Random.Range(0, InstOfItems.Count)],
                    transform.position, transform.rotation);

                spawned.gameObject.SetActive(false);
                return spawned.gameObject;
            }
        }
        else if (spawnedObj == SPAWN_OBJ.OBSTACLE)
        {
            if (spawningPool[(int)SPAWN_OBJ.OBSTACLE].Count > 0)
            {
                Obstacle spawned = spawningPool[(int)SPAWN_OBJ.OBSTACLE].Dequeue().GetComponent<Obstacle>();
                spawned.gameObject.SetActive(false);
                return spawned.gameObject;
            }
            else
            {
                Obstacle spawned = Instantiate(InstOfObstacles[UnityEngine.Random.Range(0, InstOfObstacles.Count)]
                    , transform.position, transform.rotation);

                spawned.gameObject.SetActive(false);
                return spawned.gameObject;
            }
        }
        else return null;
    }

    // SpawnedObj가 Destroy될 때 다시 회수함( 실제로는 사본이 넘겨지는 것)
    public void ReturnObj(GameObject obj, SPAWN_OBJ spawnObj)
    {
        if (spawnObj == SPAWN_OBJ.ITEM)
        {
            obj.gameObject.SetActive(false);
            spawningPool[(int)SPAWN_OBJ.ITEM].Enqueue(obj);
        }
        else if (spawnObj == SPAWN_OBJ.OBSTACLE)
        {
            obj.gameObject.SetActive(false);
            spawningPool[(int)SPAWN_OBJ.OBSTACLE].Enqueue(obj);
        }
    }
}