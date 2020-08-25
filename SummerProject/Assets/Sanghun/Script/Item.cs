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

    [SerializeField]
    public SHOOT_OPT shootOpt;

    [SerializeField]
    private COLLIDE_OPT colideOpt;

    public float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float destroyTime { get { return _destroyTime; } set { _destroyTime = value; } }

    private void OnEnable()
    {
        Invoke("DestroyItem", destroyTime);
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            ObjectManager.GetInstance.ItemType_Update[itemType](this, shootOpt);
        }
    }

    private void OnDisable()
    {
        if (gameObject.activeSelf == false) return;
        shootOpt &= ~SHOOT_OPT.MAGNET;
        Spawner.GetInstance.ReturnObj(gameObject, SPAWN_OBJ.ITEM);
    }

    private void DestroyItem()
    {
        if (gameObject.activeSelf == false) return;
        shootOpt &= ~SHOOT_OPT.MAGNET;
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