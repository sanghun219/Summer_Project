using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObj : MonoBehaviour
{
    private Rigidbody rigidy;
    private SphereCollider collider;
    private Player player;
    private Coroutine coroutine;

    [SerializeField]
    private float MaxColliderRadius = 4000.0f;

    [SerializeField]
    private float pushTimer = 10.0f;

    [SerializeField]
    private ParticleSystem particle;

    private void Awake()
    {
        rigidy = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        particle = Instantiate<ParticleSystem>(particle);
    }

    private void OnDisable()
    {
        player.SetPlayerMode(PlayerMode.PUSH, PlayerMode.NORMAL);
        collider.radius = 1.0f;
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(PushObjUpdate());
    }

    private IEnumerator PushObjUpdate()
    {
        float previousTimer = 0.0f;
        particle.Play(true);
        while (pushTimer >= previousTimer)
        {
            yield return new WaitForFixedUpdate();

            previousTimer += Time.fixedDeltaTime;
            if (collider.radius <= MaxColliderRadius)
            {
                //TODO : 나중에 파티클 집어넣어서 효과도 빠방하게 할 수 있음
                collider.radius += Time.fixedDeltaTime * 1500;
            }
        }

        player.SetPlayerMode(PlayerMode.PUSH, PlayerMode.NORMAL);
        collider.radius = 1.0f;
        previousTimer = 0.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            collision.gameObject.GetComponent<Obstacle>().DestroyObs(1.0f);
        }
    }
}