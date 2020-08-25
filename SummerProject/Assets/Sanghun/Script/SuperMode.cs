using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMode : MonoBehaviour
{
    private Coroutine coroutine;

    [SerializeField]
    private float SuperModeTimer = 3.0f;

    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateSuperMode());
    }

    private IEnumerator UpdateSuperMode()
    {
        float previousTimer = 0.0f;
        player.GetComponent<CapsuleCollider>().isTrigger = true;
        while (previousTimer <= SuperModeTimer)
        {
            // TODO : 파티클이나 사운드 빠방하게 넣을수 있음!
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }
        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.SetPlayerMode(PlayerMode.NORMAL);
    }
}