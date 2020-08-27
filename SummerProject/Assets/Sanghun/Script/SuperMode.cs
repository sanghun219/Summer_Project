using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMode : MonoBehaviour
{
    private Coroutine coroutine;

    [SerializeField]
    private float SuperModeTimer = 3.0f;

    private Player player;

    [SerializeField]
    private ParticleSystem particle;

    private ParticleSystem tempParticle = null;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (tempParticle == null)
            tempParticle = Instantiate<ParticleSystem>(particle);
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
        tempParticle.Play();
        player.VelocityZ = 1200;
        while (previousTimer <= SuperModeTimer)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }

        tempParticle.Stop();
        player.VelocityZ = originSpeed;
        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.SetPlayerMode(PlayerMode.SUPER, PlayerMode.NORMAL);
    }
}