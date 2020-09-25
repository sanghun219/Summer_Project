using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObj : MonoBehaviour
{
    private Rigidbody rigidy;
    private SphereCollider spCollider;
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
        spCollider = gameObject.GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        particle = Instantiate<ParticleSystem>(particle);
    }

    private void OnDisable()
    {
        player.SetPlayerMode(PlayerMode.PUSH, PlayerMode.NORMAL);
        spCollider.radius = 1.0f;
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(PushObjUpdate());
    }

    private IEnumerator PushObjUpdate()
    {
        float previousTimer = 0.0f;
        particle.Play(true);
        Debug.Log("실행되니?");
        while (pushTimer >= previousTimer)
        {
            yield return new WaitForFixedUpdate();

            previousTimer += Time.fixedDeltaTime;
            if (spCollider.radius <= MaxColliderRadius)
            {
                spCollider.radius += Time.fixedDeltaTime * 1500;
            }
            Debug.Log("pushObj : " + previousTimer);
        }

        player.SetPlayerMode(PlayerMode.PUSH, PlayerMode.NORMAL);
        spCollider.radius = 1.0f;
        previousTimer = 0.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (collision.gameObject.activeSelf == false)
                collision.gameObject.GetComponent<Collider>().enabled = false;
            collision.gameObject.GetComponent<Obstacle>().DestroyObs(1.0f, () =>
            { collision.gameObject.GetComponent<Collider>().enabled = true; });
        }
    }
}