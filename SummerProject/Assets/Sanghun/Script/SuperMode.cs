using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMode : MonoBehaviour
{
    private Coroutine coroutine;

    public float SuperModeTimer = 3.0f;

    private Player player;

    [SerializeField]
    private ParticleSystem particle;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        particle = Instantiate<ParticleSystem>(particle);
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateSuperMode());
    }

    private IEnumerator UpdateSuperMode()
    {
        float previousTimer = 0.0f;
        float originSpeed = player.VelocityZ;
        player.GetComponent<CapsuleCollider>().isTrigger = true;
        particle.Play();
        player.VelocityZ = 1200;
        while (previousTimer <= SuperModeTimer)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }

        particle.Stop();
        player.VelocityZ = originSpeed;
        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.SetPlayerMode(PlayerMode.SUPER, PlayerMode.NORMAL);
    }
}