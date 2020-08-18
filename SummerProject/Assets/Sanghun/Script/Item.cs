﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ITEM_TYPE
{
    ITEM1,
    ITEM2,
    END,
}

public class Item : MonoBehaviour, ISpawned
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _destroyTime;

    [SerializeField]
    private ITEM_TYPE itemType;

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
            transform.position += transform.forward * speed;
    }

    private void DestroyItem()
    {
        Spawner.GetInstance.ReturnObj(gameObject, SPAWN_OBJ.ITEM);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO : 타입에 따라 다른 처리
            switch (itemType)
            {
                case ITEM_TYPE.ITEM1:
                    break;

                case ITEM_TYPE.ITEM2:
                    break;

                default:
                    break;
            }
        }
    }
}