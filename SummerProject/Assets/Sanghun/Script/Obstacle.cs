using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, ISpawned
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _destroyTime;

    [SerializeField]
    private SHOOT_OPT shootOpt;

    [SerializeField]
    private COLLIDE_OPT colideOpt;

    public float speed { get { return _speed; } set { _speed = value; } }

    public float destroyTime { get { return _destroyTime; } set { _destroyTime = value; } }

    public OBSTACLE_TYPE Obs_type;

    public bool isCollide = false;

    // Collide로 bool 값을 바로 끄면 물리적 효과가 나타나지 않음

    private void FixedUpdate()
    {
        ObjectManager.GetInstance.ObsType_Update[Obs_type](this, shootOpt);
    }

    // Start와 Awake 차이는 Awake가 순서가 빠르며, Awake는 active가 꺼진 상태에도 작동되지만
    // Start는 active가 꺼진 오브젝트에 대해 실행되지 않음.
    // OnEnable은 setactive가 true가 될 때마다 동작함

    private void OnEnable()
    {
        Invoke("DestroyObs", destroyTime);
    }

    private void DestroyObs()
    {
        if (gameObject.activeSelf)
        {
            Spawner.GetInstance.ReturnObj(gameObject, SPAWN_OBJ.OBSTACLE);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjectManager.GetInstance.ObsType_Collide[Obs_type](this, collision, colideOpt);
        }
    }
}