using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, ISpawned
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _destroyTime;

    [SerializeField]
    private ITEM_TYPE itemType;

    public SHOOT_OPT shootOpt;

    [SerializeField]
    private COLLIDE_OPT colideOpt;

    public float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float destroyTime { get { return _destroyTime; } set { _destroyTime = value; } }

    private bool isRestart = false;

    private void Awake()
    {
        Spawner.GetInstance.RestartEvent += Restart;
    }

    private void Restart()
    {
        isRestart = true;
    }

    private void OnEnable()
    {
        Invoke("DestroyItem", destroyTime);
        isRestart = false;
    }

    private void FixedUpdate()
    {
        if (isRestart) { DestroyItem(); return; }
        if (gameObject.activeSelf)
        {
            ObjectManager.GetInstance.ItemType_Update[itemType](this, shootOpt);
        }
    }

    private void OnDisable()
    {
        DestroyItem();
    }

    private void DestroyItem()
    {
        if (isRestart == false) return;
        if (IsInvoking("DestroyItem"))
            CancelInvoke("DestroyItem");
        isRestart = false;
        shootOpt &= ~SHOOT_OPT.MAGNET;
        if (Spawner.GetInstance == null) { Debug.Log("게임 종료시 Spawner가 먼저 힙에서 사라짐 문제없음"); return; }

        Spawner.GetInstance.ReturnObj(gameObject, SPAWN_OBJ.ITEM);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectManager.GetInstance.ItemType_Collide[itemType](this, other, colideOpt);
        }
    }
}