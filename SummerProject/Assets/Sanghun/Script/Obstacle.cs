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

    public SHOOT_OPT shootOpt;

    [SerializeField]
    private COLLIDE_OPT colideOpt;

    public float speed { get { return _speed; } set { _speed = value; } }

    public float destroyTime { get { return _destroyTime; } set { _destroyTime = value; } }

    public OBSTACLE_TYPE Obs_type;

    private bool isRestart = false;

    // 게임 재시작시
    private bool isNotReturningObjPool = false;

    private void FixedUpdate()
    {
        if (isRestart)
        {
            DestroyObs();
            return;
        }
        if (gameObject.activeSelf)
            ObjectManager.GetInstance.ObsType_Update[Obs_type](this, shootOpt);
    }

    private void Awake()
    {
        Spawner.GetInstance.RestartEvent += Restart;
    }

    private void Restart()
    {
        isRestart = true;
    }

    // Start와 Awake 차이는 Awake가 순서가 빠르며, Awake는 active가 꺼진 상태에도 작동되지만
    // Start는 active가 꺼진 오브젝트에 대해 실행되지 않음.
    // OnEnable은 setactive가 true가 될 때마다 동작함

    private void OnEnable()
    {
        Invoke("DestroyObs", destroyTime);
        isRestart = false;
    }

    private void OnDisable()
    {
        DestroyObs();
    }

    private void DestroyObs()
    {
        CancelInvoke("DestroyObs");
        isRestart = false;
        if (Spawner.GetInstance == null) { Debug.Log("게임 종료시 Spawner가 먼저 힙에서 사라짐 문제없음"); return; }
        Spawner.GetInstance.ReturnObj(gameObject, SPAWN_OBJ.OBSTACLE);
    }

    public void DestroyObs(float later)
    {
        StartCoroutine(IDestroyObs(later));
    }

    private IEnumerator IDestroyObs(float later)
    {
        float timer = 0.0f;
        while (timer <= later)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        DestroyObs();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjectManager.GetInstance.ObsType_Collide[Obs_type](this, collision, colideOpt);
        }
    }
}