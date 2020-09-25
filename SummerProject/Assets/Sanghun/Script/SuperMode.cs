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
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        particle = Instantiate<ParticleSystem>(particle);
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangePlayer;
    }

    private void ChangePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
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
        player.VelocityZ = 1500;
        while (previousTimer <= SuperModeTimer && player.isGameOver == false)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
            Debug.Log("SupderMode : " + previousTimer);
        }

        particle.Stop();
        player.VelocityZ = originSpeed;
        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.SetPlayerMode(PlayerMode.SUPER, PlayerMode.NORMAL);
    }
}