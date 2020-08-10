using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OBSTACLE_TYPE
{
    OBS1,
    OBS2,
    END,
}

public class Obstacle : MonoBehaviour, ISpawned
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _destroyTime;

    public float speed { get { return _speed; } set { _speed = value; } }

    public float destroyTime { get { return _destroyTime; } set { _destroyTime = value; } }

    public OBSTACLE_TYPE Obs_type;

    private void FixedUpdate()
    {
        transform.position += transform.right * speed;
    }

    public void Awake()
    {
        Invoke("DestroyObs", destroyTime);
    }

    private void DestroyObs()
    {
        Spawner.GetInstance.ReturnObj(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO : 충돌시 타입에 따른 다른 처리를 하자
            switch (Obs_type)
            {
                case OBSTACLE_TYPE.OBS1:

                    break;

                case OBSTACLE_TYPE.OBS2:
                    break;

                default:
                    break;
            }
        }
    }
}