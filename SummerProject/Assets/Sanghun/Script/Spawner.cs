using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    public int NumOfMaxInitObject;

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

    public event Action RestartEvent;

    public bool isInitSpawner = false;

    public bool WaitRestart { get; set; }

    public void Restart()
    {
        if (RestartEvent != null)
        {
            RestartEvent();
        }
    }

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
                    return null;
                }
            }
            return Instance;
        }
    }

    public void Awake()
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
        SceneManager.sceneLoaded += StartInitPooling;
        EndOfSpawnPoint = new Vector3[2];
        WaitRestart = false;
    }

    public void StartInitPooling(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitSpawningPool());
    }

    public void UpdateSpawnerPosition(Vector3 playerPos)
    {
        if (WaitRestart) return;

        deltaSpawnTime += Time.deltaTime;
        if (deltaSpawnTime >= elaspedSpawn)
        {
            deltaSpawnTime = 0.0f;
            SpawnObjects(playerPos);
        }
        Vector3 center = new Vector3(playerPos.x, playerPos.y, playerPos.z + distSpawnerToPlayer);
        transform.position = new Vector3(center.x, center.y, center.z);

        // Spawner 대각선의 양 끝점을 지정
        EndOfSpawnPoint[0].x = center.x - lengOfSpawner / 2;
        EndOfSpawnPoint[0].z = center.z;
        EndOfSpawnPoint[1].x = center.x + lengOfSpawner / 2;
        EndOfSpawnPoint[1].z = center.z;
    }

    // 매 주기마다 스폰이 됨
    private void SpawnObjects(Vector3 playerPos)
    {
        // 한 wave에 생길수 있는 장애물 비율/빈 공간 비율/ 아이템 비율 (장애물 비율은 50%이상)
        int numOfObstacle = (int)((numOfSpawnedObj + 1) * ratioOfObstacle);
        int numOfEmpty = UnityEngine.Random.Range(0, numOfObstacle);
        int numOfItem = numOfSpawnedObj - numOfEmpty - numOfObstacle;
        List<GameObject> wave = new List<GameObject>();

        // TODO : 개수 지정 구간1 ( 여기서 비율 조정 가능)
        for (int i = 0; i < numOfItem + 3; i++)
        {
            wave.Add(GetSpawnedObj(SPAWN_OBJ.ITEM));
        }
        for (int i = 0; i < numOfObstacle; i++)
        {
            wave.Add(GetSpawnedObj(SPAWN_OBJ.OBSTACLE));
        }

        for (int i = 0; i < numOfEmpty; i++)
        {
            wave.Add(null);
        }

        GetShuffledSpawnedObj(ref wave);

        //안에 요소들 포지션이 정해짐 (내분)
        for (int i = 0; i < wave.Count; i++)
        {
            float px = ((i * EndOfSpawnPoint[1].x) + (wave.Count - i) * EndOfSpawnPoint[0].x) / (wave.Count);
            float pz = ((i * EndOfSpawnPoint[1].z) + (wave.Count - i) * EndOfSpawnPoint[0].z) / (wave.Count);
            float randZ;
            float randX;
            if (i == 0) // 플레이어 앞쪽으로 아이템이든 장애물이든 하나는 오게하자
            {
                randX = playerPos.x;
                randZ = pz;
            }
            else
            {
                randZ = UnityEngine.Random.Range(pz + 250, pz + 100);
                randX = UnityEngine.Random.Range(px - 250, px + 250);
            }
            var obj = wave[i];
            // 빈공간
            if (obj == null) continue;
            if (!obj.activeSelf)
            {
                obj.transform.position = new Vector3(randX, obj.transform.position.y + playerPos.y, randZ);
                obj.SetActive(true);
            }
        }
    }

    private void GetShuffledSpawnedObj(ref List<GameObject> spawnObjs)
    {
        int maxValue = spawnObjs.Count;
        int tmpValue;
        GameObject swapValue = null;

        for (int i = 0; i < maxValue; i++)
        {
            tmpValue = UnityEngine.Random.Range(0, maxValue);
            swapValue = spawnObjs[i];
            spawnObjs[i] = spawnObjs[tmpValue];
            spawnObjs[tmpValue] = swapValue;
        }
    }

    public IEnumerator InitSpawningPool()
    {
        for (int i = 0; i < 2; i++)
        {
            spawningPool[i] = new Queue<GameObject>(NumOfMaxInitObject);
        }

        for (int i = 0; i < NumOfMaxInitObject * 2; i++)
        {
            yield return new WaitForEndOfFrame();
            if (i % 2 == 0)
            {
                // 장애물 비율 적당히 조절해서 instantiate
                GameObject tempGO = InstOfObstacles[UnityEngine.Random.Range(0, InstOfObstacles.Count)].gameObject;
                GameObject spawned = Instantiate(tempGO
                    , tempGO.transform.position + transform.position, transform.rotation);

                spawned.SetActive(false);
                spawningPool[(int)SPAWN_OBJ.OBSTACLE].Enqueue(spawned);
            }
            else
            {
                // 100 코인 500 코인 1000 코인 확률 합쳐서 80% 이상
                // 나머지 20%
                GameObject spawned = null;

                //TODO : 비율 조정 구간2 (아이템 사이 확률 80% 코인, 20% 아이템)
                if (UnityEngine.Random.Range(0, 1.0f) <= 0.8)
                {
                    GameObject tempGO = InstOfItems[UnityEngine.Random.Range(0, 3)].gameObject;
                    spawned = Instantiate(tempGO
                    , tempGO.transform.position + transform.position, transform.rotation);
                }
                else
                {
                    GameObject tempGO = InstOfItems[UnityEngine.Random.Range(3, InstOfItems.Count)].gameObject;
                    spawned = Instantiate(tempGO
                    , tempGO.transform.position + transform.position, transform.rotation);
                }

                spawned.SetActive(false);
                spawningPool[(int)SPAWN_OBJ.ITEM].Enqueue(spawned);
            }
        }
        isInitSpawner = true;
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