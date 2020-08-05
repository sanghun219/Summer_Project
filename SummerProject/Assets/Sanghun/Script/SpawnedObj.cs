using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObj : MonoBehaviour
{
    public float Speed = 2.0f;
    public float DestroyTime = 20.0f;

    // Update is called once per frame
    private void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    private void Update()
    {
        transform.position += transform.forward * Speed;
    }
}