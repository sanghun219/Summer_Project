using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public enum OBSTACLE_TYPE
{
    OBS1,
    OBS2,
    END,
}

public enum ITEM_TYPE
{
    ITEM1,
    ITEM2,
    END,
}

[Flags]
public enum SHOOT_OPT
{
    // 플레이어 근접시 여러개로 나뉨
    CLONE = 0x00000002,

    // 방향을 꺾어 회전하며 나아감
    GL_ROTATE = 0x00000004,

    // 자기자리에서 회전
    LC_ROTATE = 0x00000008,

    // 방향이 일반적인 직진이 아니라 플레이어를 향해 날라옴
    GUIDE = 0x00000010,
}

[Flags]
public enum COLLIDE_OPT
{
    // 장애물에만 적용시키게 하자
    EXPLODE = 0x00000001,

    // 공통 (먹으면 바로 사라짐, 보통은 천천히 사라짐)
    VANISH = 0x00000002,
}

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager Instance = null;

    public Dictionary<OBSTACLE_TYPE, Action<Obstacle, SHOOT_OPT>> ObsType_Update = new Dictionary<OBSTACLE_TYPE, Action<Obstacle, SHOOT_OPT>>();

    public Dictionary<ITEM_TYPE, Action<Item, SHOOT_OPT>> ItemType_Update = new Dictionary<ITEM_TYPE, Action<Item, SHOOT_OPT>>();

    public Dictionary<OBSTACLE_TYPE, Action<Obstacle, Collision, COLLIDE_OPT>> ObsType_Collide = new Dictionary<OBSTACLE_TYPE, Action<Obstacle, Collision, COLLIDE_OPT>>();

    public Dictionary<ITEM_TYPE, Action<Item, Collider, COLLIDE_OPT>> ItemType_Collide = new Dictionary<ITEM_TYPE, Action<Item, Collider, COLLIDE_OPT>>();

    private Player player;

    public static ObjectManager GetInstance
    {
        get
        {
            if (!Instance)
            {
                Instance = GameObject.Find("GameManager").GetComponent<ObjectManager>();
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // TODO : 장애물/아이템 추가시 Key/value 형식으로 집어넣으면 됨
        ObsType_Update[OBSTACLE_TYPE.OBS1] = Obs1_Update;
        ObsType_Update[OBSTACLE_TYPE.OBS2] = Obs2_Update;
        ObsType_Collide[OBSTACLE_TYPE.OBS1] = Obs1_Collision;
        ObsType_Collide[OBSTACLE_TYPE.OBS2] = Obs2_Collision;

        ItemType_Update[ITEM_TYPE.ITEM1] = Item1_Update;
        ItemType_Update[ITEM_TYPE.ITEM2] = Item2_Update;
        ItemType_Collide[ITEM_TYPE.ITEM1] = downSpeedItem_Collision;
        ItemType_Collide[ITEM_TYPE.ITEM2] = Item2_Collision;
    }

    private void Item1_Update(Item item, SHOOT_OPT shootOpt)
    {
        item.transform.position += item.transform.forward * item.speed;

        if ((shootOpt & SHOOT_OPT.GL_ROTATE) != 0)
        {
            item.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(-1.0f, 1.0f), 0) * 30 * Time.deltaTime);
        }

        if ((shootOpt & SHOOT_OPT.GUIDE) != 0)
        {
            item.transform.LookAt(player.transform);
            // 단 한번만 LookAt 하도록 LookAt 된 이후에 옵션을 제거해 준다
            // 안해주면 계속 따라감 유도 미사일처럼
            shootOpt &= ~SHOOT_OPT.GUIDE;
        }

        if ((shootOpt & SHOOT_OPT.LC_ROTATE) != 0)
        {
            //spawned.transform.position += Vector3.forward * spawned.speed;
            item.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }
    }

    private void Item2_Update(Item item, SHOOT_OPT shootOpt)
    {
        item.transform.position += item.transform.forward * item.speed;

        if ((shootOpt & SHOOT_OPT.GL_ROTATE) != 0)
        {
            item.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(-1.0f, 1.0f), 0) * 120 * Time.deltaTime);
        }

        if ((shootOpt & SHOOT_OPT.GUIDE) != 0)
        {
            item.transform.LookAt(player.transform);
            // 단 한번만 LookAt 하도록 LookAt 된 이후에 옵션을 제거해 준다
            // 안해주면 계속 따라감 유도 미사일처럼
            shootOpt &= ~SHOOT_OPT.GUIDE;
        }

        if ((shootOpt & SHOOT_OPT.LC_ROTATE) != 0)
        {
            //spawned.transform.position += Vector3.forward * spawned.speed;
            item.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }
    }

    private void downSpeedItem_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            // 아이템 연출 효과
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }

            // item 효과 : 속도 늦추기, 점수, 무적상태,
        }
    }

    private void Item2_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
    }

    private void Obs1_Update(Obstacle spawned, SHOOT_OPT shootOpt)
    {
        spawned.transform.position += spawned.transform.forward * spawned.speed;

        if ((shootOpt & SHOOT_OPT.GL_ROTATE) != 0)
        {
            spawned.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(-1.0f, 1.0f), 0) * 30 * Time.deltaTime);
        }

        if ((shootOpt & SHOOT_OPT.GUIDE) != 0)
        {
            spawned.transform.LookAt(player.transform);
            // 단 한번만 LookAt 하도록 LookAt 된 이후에 옵션을 제거해 준다
            // 안해주면 계속 따라감 유도 미사일처럼
            shootOpt &= ~SHOOT_OPT.GUIDE;
        }

        if ((shootOpt & SHOOT_OPT.LC_ROTATE) != 0)
        {
            //spawned.transform.position += Vector3.forward * spawned.speed;
            spawned.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }
    }

    private void Obs2_Update(Obstacle spawned, SHOOT_OPT shootOpt)
    {
        spawned.transform.position += -1 * Vector3.forward * spawned.speed;

        if ((shootOpt & SHOOT_OPT.GL_ROTATE) != 0)
        {
            spawned.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }

        if ((shootOpt & SHOOT_OPT.GUIDE) != 0)
        {
            spawned.transform.LookAt(player.transform);
            // 단 한번만 LookAt 하도록 LookAt 된 이후에 옵션을 제거해 준다
            // 안해주면 계속 따라감 유도 미사일처럼
            shootOpt &= ~SHOOT_OPT.GUIDE;
        }

        if ((shootOpt & SHOOT_OPT.LC_ROTATE) != 0)
        {
            //spawned.transform.position += Vector3.forward * spawned.speed;
            spawned.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }
    }

    private void Obs1_Collision(Obstacle obs, Collision col, COLLIDE_OPT col_opt)
    {
        // 다른 기능을 추가할 수 있음
        if (player.isGameOver == false)
        {
            obs.isCollide = true;
            player.GameOver();

            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }

            if (col_opt == COLLIDE_OPT.VANISH)
            {
                obs.gameObject.SetActive(false);
            }
        }
    }

    private void Obs2_Collision(Obstacle obs, Collision col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            obs.isCollide = true;
            player.GameOver();
        }
    }
}