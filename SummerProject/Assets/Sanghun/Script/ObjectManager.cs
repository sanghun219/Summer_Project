﻿using System.Collections;
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
    DOWNSPEED,
    PUSHALLOBJ,
    POINTUP_100,
    POINTUP_500,
    POINTUP_1000,
    DOUBLEPOINT,

    // 무적
    SUPERMODE,

    MAGNET,
    END,
}

[Flags]
public enum SHOOT_OPT
{
    NONE = 0,

    // 플레이어 근접시 여러개로 나뉨
    CLONE = 1 << 0,

    // 방향을 꺾어 회전하며 나아감
    GL_ROTATE = 1 << 1,

    // 자기자리에서 회전
    LC_ROTATE = 1 << 2,

    // 방향이 일반적인 직진이 아니라 플레이어를 향해 날라옴
    GUIDE = 1 << 3,

    // 아이템을 먹어 마그넷 기능이 탑재.. 이건 아이템에만 적용될 수 있음
    MAGNET = 1 << 4,

    ALL = int.MaxValue,
}

[Flags]
public enum COLLIDE_OPT
{
    NONE = 0,

    // 장애물에만 적용시키게 하자
    EXPLODE = 1 << 0,

    // 공통 (먹으면 바로 사라짐, 보통은 천천히 사라짐)
    VANISH = 1 << 1,

    ALL = int.MaxValue,
}

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager Instance = null;

    public Dictionary<OBSTACLE_TYPE, Action<Obstacle, SHOOT_OPT>> ObsType_Update = new Dictionary<OBSTACLE_TYPE, Action<Obstacle, SHOOT_OPT>>();

    public Dictionary<ITEM_TYPE, Action<Item, SHOOT_OPT>> ItemType_Update = new Dictionary<ITEM_TYPE, Action<Item, SHOOT_OPT>>();

    public Dictionary<OBSTACLE_TYPE, Action<Obstacle, Collision, COLLIDE_OPT>> ObsType_Collide = new Dictionary<OBSTACLE_TYPE, Action<Obstacle, Collision, COLLIDE_OPT>>();

    public Dictionary<ITEM_TYPE, Action<Item, Collider, COLLIDE_OPT>> ItemType_Collide = new Dictionary<ITEM_TYPE, Action<Item, Collider, COLLIDE_OPT>>();

    private Player player;

    // 날아오는 모든 아이템 모두에 공통적으로 적용시키고 싶을 때
    public SHOOT_OPT GlobalItem_opt;

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // TODO : 장애물/아이템 추가시 Key/value 형식으로 집어넣으면 됨
        ObsType_Update[OBSTACLE_TYPE.OBS1] = Obs1_Update;
        ObsType_Update[OBSTACLE_TYPE.OBS2] = Obs2_Update;
        ObsType_Collide[OBSTACLE_TYPE.OBS1] = Obs1_Collision;
        ObsType_Collide[OBSTACLE_TYPE.OBS2] = Obs2_Collision;

        ItemType_Update[ITEM_TYPE.DOWNSPEED] = downSpeedItem_Update;
        ItemType_Update[ITEM_TYPE.PUSHALLOBJ] = PushAllObjItem_Update;
        ItemType_Update[ITEM_TYPE.POINTUP_100] = PointUPItem_100_Update;
        ItemType_Update[ITEM_TYPE.POINTUP_500] = PointUPItem_500_Update;
        ItemType_Update[ITEM_TYPE.POINTUP_1000] = PointUPItem_1000_Update;
        ItemType_Update[ITEM_TYPE.DOUBLEPOINT] = DoublePointItem_Update;
        ItemType_Update[ITEM_TYPE.MAGNET] = MagnetItem_Update;
        ItemType_Update[ITEM_TYPE.SUPERMODE] = SuperModeItem_Update;
        ItemType_Collide[ITEM_TYPE.SUPERMODE] = SuperModeItem_Collision;
        ItemType_Collide[ITEM_TYPE.MAGNET] = MagnetItem_Collision;
        ItemType_Collide[ITEM_TYPE.DOUBLEPOINT] = DoublePointItem_Collision;
        ItemType_Collide[ITEM_TYPE.DOWNSPEED] = downSpeedItem_Collision;
        ItemType_Collide[ITEM_TYPE.PUSHALLOBJ] = PushAllObjItem_Collision;
        ItemType_Collide[ITEM_TYPE.POINTUP_100] = PointUPItem_100_Collision;
        ItemType_Collide[ITEM_TYPE.POINTUP_500] = PointUPItem_500_Collision;
        ItemType_Collide[ITEM_TYPE.POINTUP_1000] = PointUPItem_1000_Collision;
    }

    private void CommonItemUpdate(Item item, SHOOT_OPT shootOpt)
    {
        float itemSpeed = item.speed;
        shootOpt |= GlobalItem_opt;
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
            item.transform.Rotate(new Vector3(0, 1, 0) * 30 * Time.deltaTime);
        }

        if ((shootOpt & SHOOT_OPT.MAGNET) != 0)
        {
            item.transform.LookAt(player.transform);
            item.transform.position += item.transform.forward * itemSpeed * 3;
        }
        if ((shootOpt & SHOOT_OPT.MAGNET) == 0)
            item.transform.position += -1 * Vector3.forward * itemSpeed;
    }

    private void SuperModeItem_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void MagnetItem_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    #region POINTUP

    private void PointUPItem_100_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void PointUPItem_500_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void PointUPItem_1000_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void PointUPItem_100_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }

            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }
            if (player.GetPlayerMode() == PlayerMode.DOUBLE_POINT)
                ScoreManager.GetInstance.AddScore(200);
            else
                ScoreManager.GetInstance.AddScore(100);
        }
    }

    private void PointUPItem_500_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }

            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }

            if (player.GetPlayerMode() == PlayerMode.DOUBLE_POINT)
                ScoreManager.GetInstance.AddScore(1000);
            else
                ScoreManager.GetInstance.AddScore(500);
        }
    }

    private void PointUPItem_1000_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }

            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }

            if (player.GetPlayerMode() == PlayerMode.DOUBLE_POINT)
                ScoreManager.GetInstance.AddScore(2000);
            else
                ScoreManager.GetInstance.AddScore(1000);
        }
    }

    #endregion POINTUP

    private void DoublePointItem_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void DoublePointItem_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            // 아이템 연출 효과
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }
            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }
            player.SetPlayerMode(PlayerMode.DOUBLE_POINT);
        }
    }

    private void SuperModeItem_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            // 아이템 연출 효과
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }
            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }
            player.SetPlayerMode(PlayerMode.SUPER);
        }
    }

    private void MagnetItem_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            // 아이템 연출 효과
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }
            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }

            player.SetPlayerMode(PlayerMode.MAGNET);
        }
    }

    private void downSpeedItem_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
    }

    private void PushAllObjItem_Update(Item item, SHOOT_OPT shootOpt)
    {
        CommonItemUpdate(item, shootOpt);
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
            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }
            // speed를 기존 foward speed로 리셋시킴.. 추가효과를 주려면 주어라
            player.VelocityZ = player.forwardSpeed;
        }
    }

    private void PushAllObjItem_Collision(Item item, Collider col, COLLIDE_OPT col_opt)
    {
        if (player.isGameOver == false)
        {
            // 아이템 연출 효과
            if (col_opt == COLLIDE_OPT.VANISH)
            {
                item.gameObject.SetActive(false);
            }
            if (col_opt == COLLIDE_OPT.EXPLODE)
            {
            }

            player.SetPlayerMode(PlayerMode.PUSH);
        }
    }

    private void CommonObstacleUpdate(Obstacle spawned, SHOOT_OPT shootOpt)
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
            spawned.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        }
    }

    private void Obs1_Update(Obstacle spawned, SHOOT_OPT shootOpt)
    {
        CommonObstacleUpdate(spawned, shootOpt);
    }

    private void Obs2_Update(Obstacle spawned, SHOOT_OPT shootOpt)
    {
        CommonObstacleUpdate(spawned, shootOpt);
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