using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObj : MonoBehaviour
{
    public float Speed = 20.0f;
    public float DestroyTime = 20.0f;

    // Update is called once per frame
    private void Start()
    {
        Invoke("DestroyObj", DestroyTime);
    }

    private void Update()
    {
        transform.position += transform.right * Speed * Time.deltaTime;
    }

    private void DestroyObj()
    {
        Spawner.GetInstance.ReturnObj(this);
        DestroyImmediate(this);
    }
}