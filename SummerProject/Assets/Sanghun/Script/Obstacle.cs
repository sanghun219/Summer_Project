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

    private IEnumerator IUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (gameObject.activeSelf)
            {
                if (transform.position.z < 0 && Mathf.Abs(player.transform.position.z - transform.position.z) >= distOffset)
                    DestroyObs();
                transform.position += transform.forward * speed;
            }
        }
    }

    private static int i = 0;

    // Start와 Awake 차이는 Awake가 순서가 빠르며, active가 꺼진 상태에도 작동되지만
    // Start는 active가 꺼진 오브젝트에 대해 실행되지 않음.
    //
    private TestPlayer player;

    private float distOffset;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
        distOffset = Spawner.GetInstance.distSpawnerToPlayer * 2;
        Invoke("DestroyObs", destroyTime);
        StartCoroutine(IUpdate());
    }

    private void DestroyObs()
    {
        if (gameObject.activeSelf)
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