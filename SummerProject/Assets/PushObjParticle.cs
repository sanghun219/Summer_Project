using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObjParticle : MonoBehaviour
{
    private Player player;
    private ParticleSystem.MainModule main;
    private ParticleSystem particleSys;
    private float originRadius;
    private float delta;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        originRadius = main.startSize.constant;
        particleSys = gameObject.GetComponent<ParticleSystem>();
        main = particleSys.main;
        delta = 10000;
    }

    private float timer = 0.0f;
    private bool locking = false;

    private void FixedUpdate()
    {
        transform.position = player.transform.position;

        if (particleSys.isStopped)
        {
            main.startSize = originRadius;
        }
        else
        {
            main.startSize = main.startSize.constant + originRadius * delta * Time.deltaTime;
        }
    }
}