using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperModeEngine : MonoBehaviour
{
    private ParticleSystem.MainModule main;
    private float originSize;
    private Player player;
    private bool OptimizeKey = false;

    [SerializeField]
    private float multiSize = 15.0f;

    private void Start()
    {
        main = gameObject.GetComponent<ParticleSystem>().main;
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
            (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
        originSize = main.startSize.constant;
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
            (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        main = gameObject.GetComponent<ParticleSystem>().main;
        originSize = main.startSize.constant;
    }

    private void FixedUpdate()
    {
        if ((player.GetPlayerMode() & PlayerMode.SUPER) != 0 && OptimizeKey == false)
        {
            main.startSize = originSize * multiSize;
            OptimizeKey = true;
        }
        else if ((player.GetPlayerMode() & PlayerMode.SUPER) == 0)
        {
            main.startSize = originSize;
            OptimizeKey = false;
        }
    }
}