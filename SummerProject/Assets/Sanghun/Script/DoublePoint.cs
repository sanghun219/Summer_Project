﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DoublePoint : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private float DoublePointTimer = 3.0f;

    private Coroutine coroutine;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateDoublePoint());
    }

    private IEnumerator UpdateDoublePoint()
    {
        float previousTimer = 0.0f;
        while (previousTimer <= DoublePointTimer)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }

        player.SetPlayerMode(PlayerMode.NORMAL);
    }
}