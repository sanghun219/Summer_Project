using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperModeParticle : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position + new Vector3(0, 0, 50);
    }
}