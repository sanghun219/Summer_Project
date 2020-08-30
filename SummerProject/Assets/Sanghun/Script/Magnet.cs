using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Magnet : MonoBehaviour
{
    private Coroutine coroutine;

    [SerializeField]
    private float MagnetTimer;

    [SerializeField]
    private float MagnetRadius;

    private Player player;

    private bool isTriggerOn = false;

    private Collider[] collide = null;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.GameOverEvent += StopUpdate;
    }

    public void StopUpdate()
    {
        StopCoroutine(coroutine);
        foreach (var col in collide)
        {
            if (col.CompareTag("Item"))
                col.gameObject.GetComponent<Item>().shootOpt &= ~SHOOT_OPT.MAGNET;
        }
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateMagnet());
    }

    private IEnumerator UpdateMagnet()
    {
        float previousTimer = 0.0f;

        while (previousTimer <= MagnetTimer && (player.GetPlayerMode() & PlayerMode.SUPER) == 0)
        {
            // TODO : 파티클이나 사운드 빠방하게 넣을수 있음!
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
            collide = Physics.OverlapSphere(player.transform.position, MagnetRadius);
            foreach (var col in collide)
            {
                if (col.CompareTag("Item"))
                    col.gameObject.GetComponent<Item>().shootOpt |= SHOOT_OPT.MAGNET;
            }
        }

        foreach (var col in collide)
        {
            if (col.CompareTag("Item"))
                col.gameObject.GetComponent<Item>().shootOpt &= ~SHOOT_OPT.MAGNET;
        }

        player.SetPlayerMode(PlayerMode.MAGNET, PlayerMode.NORMAL);
    }
}