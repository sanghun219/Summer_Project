using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperModeEngine : MonoBehaviour
{
    private ParticleSystem.MainModule main;
    private float originSize;
    private Player player;
    private bool OptimizeKey = false;

    private void Awake()
    {
        main = gameObject.GetComponent<ParticleSystem>().main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        originSize = main.startSize.constant;
    }

    private void FixedUpdate()
    {
        if (player.GetPlayerMode() == PlayerMode.SUPER && OptimizeKey == false)
        {
            main.startSize = originSize * 15;
            OptimizeKey = true;
        }
        else if ((player.GetPlayerMode() & PlayerMode.SUPER) != 0)
        {
            main.startSize = originSize;
            OptimizeKey = false;
        }
    }
}